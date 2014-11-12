using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MasterDataFlow.Exceptions;
using MasterDataFlow.Interfaces.Network;

namespace MasterDataFlow.Network
{
    public class HubAccumulator : IHubAccumulator
    {
        public class LockStructure
        {
            public LockStructure()
            {
                SyncObject = new object();
                Status = HubAccumulatorStatus.Free;
            }

            public object SyncObject { get; set; }
            public HubAccumulatorStatus Status { get; set; }
        }

        private readonly Dictionary<string, List<IPacket>> _basket = new Dictionary<string, List<IPacket>>();
        private readonly Dictionary<string, LockStructure> _locks = new Dictionary<string, LockStructure>();

        public void Add(string key, IPacket packet)
        {
            lock (_locks)
            {
                List<IPacket> list;
                if (_basket.TryGetValue(key, out list))
                {
                    list.Add(packet);
                    return;
                }
                list = new List<IPacket> {packet};
                _basket.Add(key, list);
            }
        }

        public void SetBusyStatus(string key)
        {
            lock (_locks)
            {
                LockStructure lockStruct;
                if (!_locks.TryGetValue(key, out lockStruct))
                    throw new MasterDataFlowException(String.Format("Key {0} was not been locked.", key));
                lockStruct.Status = HubAccumulatorStatus.Busy;
            }
        }

        public List<IPacket> Extract(string key)
        {
            lock (_locks)
            {
                LockStructure lockStruct;
                if (!_locks.TryGetValue(key, out lockStruct))
                    throw new MasterDataFlowException(String.Format("Key {0} was not been locked.", key));

                List<IPacket> list;
                if (_basket.TryGetValue(key, out list))
                    _basket.Remove(key);

                lockStruct.Status = HubAccumulatorStatus.Extracted;
                return list;
            }            
        }

        public void Lock(string key)
        {
            LockStructure lockStruct;
            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out lockStruct))
                {
                    lockStruct = new LockStructure();
                    _locks.Add(key, lockStruct);
                }
            }

            while (true)
            {
                Monitor.Enter(lockStruct.SyncObject);

                lock (_locks)
                {
                    LockStructure chekingLockStruct;
                    if (!_locks.TryGetValue(key, out chekingLockStruct))
                    {
                        Monitor.Exit(lockStruct.SyncObject);
                        lockStruct = new LockStructure();
                        _locks.Add(key, lockStruct);
                        Monitor.Enter(lockStruct.SyncObject);
                        break;
                    }
                    else
                    {
                        if (chekingLockStruct != lockStruct)
                        {
                            Monitor.Exit(lockStruct.SyncObject);
                            lockStruct = chekingLockStruct;
                        }
                        else
                            break;
                    }
                }
            }

        }

        public void UnLock(string key)
        {
            lock (_locks)
            {
                LockStructure lockStruct;
                if (!_locks.TryGetValue(key, out lockStruct))
                    throw new MasterDataFlowException(String.Format("Key {0} was not been locked.", key));

                if (lockStruct.Status == HubAccumulatorStatus.Extracted)
                {
                    _locks.Remove(key);
                }
                Monitor.Exit(lockStruct.SyncObject);
            }
        }

        public HubAccumulatorStatus GetStatus(string key)
        {
            lock (_locks)
            {
                LockStructure lockStruct;
                if (!_locks.TryGetValue(key, out lockStruct))
                    throw new MasterDataFlowException(String.Format("Key {0} was not been locked.", key));
                return lockStruct.Status;
            }
        }

    }
}
