using System.Collections;

namespace MasterDataFlow.Utils
{
    public class AsyncQueue<T>
    {
        private readonly Queue _syncdQ = Queue.Synchronized(new Queue());

        public void Enqueue(T item)
        {
            _syncdQ.Enqueue(item);
        }

        public void EnqueueRange(IEnumerable items)
        {
            foreach (var item in items)
            {
                _syncdQ.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            return (T)(_syncdQ.Dequeue());
        }

        public T Peek()
        {
            return (T)(_syncdQ.Peek());
        }

        public int Count
        {
            get { return _syncdQ.Count; }
        }

        public object SyncRoot
        {
            get { return _syncdQ.SyncRoot; }
        }

    }
}
