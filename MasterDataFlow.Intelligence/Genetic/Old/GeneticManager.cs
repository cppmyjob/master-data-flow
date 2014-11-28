using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Xml.Linq;

namespace MasterDataFlow.Intelligence.Genetic.Old
{
    public abstract class GeneticManager
    {

		//http://stackoverflow.com/questions/2510593/how-can-i-set-processor-affinity-in-net

		/// <summary>
		/// Gets and sets the processor affinity of the current thread.
		/// </summary>
		public static class ProcessorAffinity
		{
			static class Win32Native
			{
				//GetCurrentThread() returns only a pseudo handle. No need for a SafeHandle here.
				[DllImport("kernel32.dll")]
				public static extern IntPtr GetCurrentThread();

				[HostProtection(SelfAffectingThreading = true)]
				[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
				public static extern UIntPtr SetThreadAffinityMask(IntPtr handle, UIntPtr mask);

			}

			public struct ProcessorAffinityHelper : IDisposable
			{
				UIntPtr lastaffinity;

				internal ProcessorAffinityHelper(UIntPtr lastaffinity)
				{
					this.lastaffinity = lastaffinity;
				}

				#region IDisposable Members

				public void Dispose()
				{
					if (lastaffinity != UIntPtr.Zero)
					{
						Win32Native.SetThreadAffinityMask(Win32Native.GetCurrentThread(), lastaffinity);
						lastaffinity = UIntPtr.Zero;
					}
				}

				#endregion
			}

			static ulong maskfromids(params int[] ids)
			{
				ulong mask = 0;
				foreach (int id in ids)
				{
					if (id < 0 || id >= Environment.ProcessorCount)
						throw new ArgumentOutOfRangeException("CPUId", id.ToString());
					mask |= 1UL << id;
				}
				return mask;
			}

			/// <summary>
			/// Sets a processor affinity mask for the current thread.
			/// </summary>
			/// <param name="mask">A thread affinity mask where each bit set to 1 specifies a logical processor on which this thread is allowed to run. 
			/// <remarks>Note: a thread cannot specify a broader set of CPUs than those specified in the process affinity mask.</remarks> 
			/// </param>
			/// <returns>The previous affinity mask for the current thread.</returns>
			public static UIntPtr SetThreadAffinityMask(UIntPtr mask)
			{
				UIntPtr lastaffinity = Win32Native.SetThreadAffinityMask(Win32Native.GetCurrentThread(), mask);
				if (lastaffinity == UIntPtr.Zero)
					throw new Win32Exception(Marshal.GetLastWin32Error());
				return lastaffinity;
			}

			/// <summary>
			/// Sets the logical CPUs that the current thread is allowed to execute on.
			/// </summary>
			/// <param name="CPUIds">One or more logical processor identifier(s) the current thread is allowed to run on.<remarks>Note: numbering starts from 0.</remarks></param>
			/// <returns>The previous affinity mask for the current thread.</returns>
			public static UIntPtr SetThreadAffinity(params int[] CPUIds)
			{
				return SetThreadAffinityMask(((UIntPtr)maskfromids(CPUIds)));
			}

			/// <summary>
			/// Restrict a code block to run on the specified logical CPUs in conjuction with 
			/// the <code>using</code> statement.
			/// </summary>
			/// <param name="CPUIds">One or more logical processor identifier(s) the current thread is allowed to run on.<remarks>Note: numbering starts from 0.</remarks></param>
			/// <returns>A helper structure that will reset the affinity when its Dispose() method is called at the end of the using block.</returns>
			public static ProcessorAffinityHelper BeginAffinity(params int[] CPUIds)
			{
				return new ProcessorAffinityHelper(SetThreadAffinityMask(((UIntPtr)maskfromids(CPUIds))));
			}

		}


		public class FitnessThread : IDisposable
		{
			private Thread _geneticThread;
			private GeneticItem[] _itemsArray;
			private ManualResetEvent _event = new ManualResetEvent(false);
			private GeneticManager _manager;
			private int _processorNumber = 0;

			public ManualResetEvent Event
			{
				get { return _event; }
			}

			private static int _gProcessorNumber = 0;

			public FitnessThread(GeneticManager manager, GeneticItem[] itemsArray)
			{
				_manager = manager;
				_itemsArray = itemsArray;
				_processorNumber = _gProcessorNumber % _manager.ProcessorCount;
				++_gProcessorNumber;

				_geneticThread = new Thread(ThreadProc);
				_geneticThread.Start();
			}


			private void ThreadProc()
			{
				using (ProcessorAffinity.BeginAffinity(_processorNumber))
				{
					for (int i = 0; i < _itemsArray.Length; i++)
					{
						GeneticItem item = _itemsArray[i];
//						if (item.Fitness == 0.0)
                        item.Fitness = _manager.InternalCalculateFitness(item, _processorNumber);
					}
					_event.Set();
				}

			}



			public void Stop()
			{
				if (_geneticThread != null)
				{
					if (_geneticThread.IsAlive)
						_geneticThread.Abort();
					_geneticThread = null;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				Stop();
			}

			#endregion
		}


        private int _itemsCount;
        protected int _valuesCount;
        private int _surviveCount;
		private int _processorCount = 0;
		private int _currentYear = 1;

		public int CurrentYear
		{
			get { return _currentYear; }
		}

        protected GeneticItem[] _itemsArray;

        protected Random _random = new Random((int)DateTime.Now.Ticks);
        //protected FastRandom _random = new FastRandom((int)DateTime.Now.Ticks);
        
        protected GeneticManager(int itemsCount, int surviveCount, int valuesCount)
        {
            _itemsCount = itemsCount;
            _surviveCount = surviveCount;
            _valuesCount = valuesCount;

            Init();

            CreatingPopulation(0);
        }

        private void Init()
        {
            if (_surviveCount > _itemsCount)
                throw new Exception("Число выживших > число особей");
            if (_surviveCount < 1)
                throw new Exception("Число выживших не может быть < 1");
            if (_itemsCount < 2)
                throw new Exception("Число особей в популяции не может быть < 2");

            _itemsArray = new GeneticItem[_itemsCount];
        }

        public int SurviveCount
        {
            get { return _surviveCount; }
        }

        public int ValuesCount
        {
            get { return _valuesCount; }
        }

        public int ItemsCount
        {
            get { return _itemsCount; }
        }

		public int ProcessorCount
		{
			get { return _processorCount; }
			set { _processorCount = value; }
		}

        private void CreatingPopulation(int from)
        {
            for (int i = from; i < _itemsCount; i++)
            {
                GeneticItem item = InternalCreateItem();
				FillValues(item);
                _itemsArray[i] = item;
            }
        }

		protected void FillValues(GeneticItem item)
		{
			double[] values = item.Values;
			for (int j = 0; j < _valuesCount; j++)
			{
				double valueValue = item.CreateValue(_random.NextDouble());
				values[j] = valueValue;
			}
			item.InitOtherValues(_random);
		}

        public void LeaveOnlyBest()
        {
            _random = new Random((int)DateTime.Now.Ticks);
            CreatingPopulation(1);
        }

        public double Process()
        {
            _random = new Random((int)DateTime.Now.Ticks);
            Reproduction();
            Selection();
            ++_currentYear;
            return _itemsArray[0].Fitness;
        }

        protected virtual void SortFitness()
        {
			List<GeneticItem> list = (from item in _itemsArray
									  orderby item.Fitness descending
									  select item).ToList();
			while (list.Count < _itemsCount)
			{
				GeneticItem item = InternalCreateItem();
				FillValues(item);
				list.Add(item);
			}

			_itemsArray = list.ToArray();
        }

        internal void Selection()
        {
			if (_processorCount == 0)
				CalculateWithOneProcessor();
			else
				CalculateWithMoreProcessors();

            SortFitness();
        }

		private void CalculateWithOneProcessor()
		{
			for (int i = 0; i < _itemsCount; i++)
			{
				GeneticItem item = _itemsArray[i];
				if (item.Fitness == 0.0)
					item.Fitness = InternalCalculateFitness(item, 0);
			}
		}

		protected virtual double InternalCalculateFitness(GeneticItem item, int processor)
		{
			return CalculateFitness(item, processor);
		}

		private void CalculateWithMoreProcessors()
		{
            List<GeneticItem> needToCalc = _itemsArray.Where(t => t.Fitness == 0.0).ToList();

            List<List<GeneticItem>> ll = new List<List<GeneticItem>>();
            for (int i = 0; i < _processorCount; i++)
            {
                ll.Add(new List<GeneticItem>());
            }

            int proccessor = 0;
            for (int i = 0; i < needToCalc.Count; i++)
            {
                ll[proccessor].Add(needToCalc[i]);
                ++proccessor;
                if (proccessor == _processorCount)
                    proccessor = 0;
            }

            List<FitnessThread> threads = new List<FitnessThread>();
            for (int i = 0; i < _processorCount; i++)
            {
                threads.Add(new FitnessThread(this, ll[i].ToArray()));
            }

			for (int i = 0; i < threads.Count; i++)
			{
				threads[i].Event.WaitOne();
			}

			for (int i = 0; i < threads.Count; i++)
			{
				threads[i].Dispose();
			}


		}

        internal void Reproduction()
        {
            for (int i = 0; i < _itemsCount-_surviveCount; i++)
            {
                GeneticItem firstParent = _itemsArray[i + _surviveCount];
				if (firstParent.Fitness == 0)
					continue;
                int secondParentIndex;
//                do
//                {
                    secondParentIndex = _random.Next(_surviveCount);
//               ` } while (secondParentIndex == i);
                GeneticItem secondParent = _itemsArray[secondParentIndex];
				if (secondParent.Fitness == 0)
					continue;
                GeneticItem child = CreateChild(firstParent, secondParent);
                _itemsArray[i + _surviveCount] = child;
            }
        }

        protected internal virtual GeneticItem CreateChild(GeneticItem firstParent, GeneticItem secondParent)
        {
            GeneticItem child = InternalCreateItem();
			// Определяем количество частей для обмена
            int partCount = _random.Next(_valuesCount) + 1;
            float coeff = (partCount / (float)_valuesCount);
			// Признак того, кто будет первым при копировании 1 или 2 предок
            bool isFirst = _random.Next(10) < 5;
            int lastPartIndex = 0;
            double[] firstValues = firstParent.Values;
            double[] secondValues = secondParent.Values;
            double[] childValues = child.Values;
			// Это признак того, что-бы копировать одного предка относительно 
			// другого со смещением
            int secondOffset = 0;
            if (_random.NextDouble() > 0.999)
            //if (_random.NextDouble() > 0.9)
                secondOffset = _random.Next(_valuesCount) + 1;
			// 
            for (int i = 0; i < _valuesCount; i++)
            {
                double value;
                if (secondOffset == 0)
                    value = isFirst ? firstValues[i] : secondValues[i];
                else
                {
                    if (isFirst)
                    {
                        value = firstValues[i];
                    } else
                    {
                        if (secondOffset + i < _valuesCount)
                            value = secondValues[secondOffset + i];
                        else
                        {
                            value = child.CreateValue(_random.NextDouble());
                        }
                    }
                }

				// Это мутация значения
                if (_random.NextDouble() > 0.999)
//                if (_random.NextDouble() > 0.9)
                {
                    double valueValue = child.CreateValue(_random.NextDouble());
                    value = valueValue;
                }
                childValues[i] = value;
                int partIndex = (int)(coeff * i);
                if (partIndex != lastPartIndex)
                {
                    isFirst = !isFirst;
                    lastPartIndex = partIndex;
                }
            }
            return child;
        }

        protected GeneticItem InternalCreateItem()
        {
            GeneticInitData initData = new GeneticInitData();
            initData.Count = _valuesCount;
            initData.YearOfBorn = _currentYear;
            GeneticItem child = CreateItem(initData);
            return child;
        }

		public abstract double CalculateFitness(GeneticItem item, int processor);
        protected abstract GeneticItem CreateItem(GeneticInitData initData);

        public List<GeneticItem> GetAll()
        {
            return _itemsArray.ToList();
        }

        public List<GeneticItem> GetSurvived()
        {
            List<GeneticItem> result = new List<GeneticItem>();
            for (int i = 0; i < _surviveCount; i++)
            {
                GeneticItem item = _itemsArray[i];
                result.Add(item);
            }
            return result;
        }


        public void Write(TextWriter writer)
        {
            XElement root = new XElement("Genetic");
            root.Add(new XElement("ItemsCount", _itemsCount));
            root.Add(new XElement("SurviveCount", _surviveCount));
            root.Add(new XElement("ValuesCount", _valuesCount));

            _itemsArray[0].Write(root);

            root.Save(writer);

        }


        public void Read(TextReader reader)
        {
            XElement root = XElement.Load(reader, LoadOptions.None);

            XElement eItemsCount = root.Element("ItemsCount");
            _itemsCount = Int32.Parse(eItemsCount.Value);

            XElement eValuesCount = root.Element("ValuesCount");
            _valuesCount = Int32.Parse(eValuesCount.Value);

            XElement eSurviveCount = root.Element("SurviveCount");
            _surviveCount = Int32.Parse(eSurviveCount.Value);

            Init();

            GeneticInitData initData = new GeneticInitData();
            initData.Count = _valuesCount;
            // TODO
            initData.YearOfBorn = _currentYear; 
            GeneticItem best = CreateItem(initData);

            best.Read(root);
            _itemsArray[0] = best;

            CreatingPopulation(1);

        }
    }
}
