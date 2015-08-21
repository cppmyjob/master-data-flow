using System;
using System.Xml.Linq;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public abstract class GeneticItem
    {
        protected double[] _values;
        private double _fitness;
        private GeneticInitData _initData;
        private readonly Guid _guid = Guid.NewGuid();
        private double[] _oldValues;

        public GeneticItem(GeneticInitData initData)
        {
            _initData = initData;
            _values = new double[initData.Count];
        }

        public Guid Guid
        {
            get { return _guid; }
        } 

        public GeneticInitData InitData
        {
            get { return _initData; }
        }

        public double Fitness
        {
            get { return _fitness; }
            set
            {
                if (_fitness != 0.0)
                {
                    if (_fitness != value)
                    {
                        throw new Exception("Изменился Fitness");
                    }
                }

                _fitness = value;
            }
        }


        public double[] Values
        {
            get { return _values; }
        }

        protected internal abstract double CreateValue(double random);

		protected internal virtual void InitOtherValues(IRandom random) { }

        protected internal virtual void SaveValues()
        {
            if (_oldValues == null)
                _oldValues = new double[_values.Length];
            Array.Copy(_values, _oldValues, _oldValues.Length);
        }

        protected internal virtual void RestoreValues()
        {
            Array.Copy(_oldValues, _values, _values.Length);
            _oldValues = null;
        }

        public virtual void Write(XElement root)
        {
            XElement best = new XElement("Item");
            root.Add(best);

            best.Add(new XElement("Fitness", Fitness.ToString()));

            XElement values = new XElement("Values");
            best.Add(values);
            foreach (double value in Values)
            {
                values.Add(new XElement("Value", value.ToString()));
            }
        }


        public virtual void Read(XElement root)
        {
            XElement eBest = root.Element("Item");

            XElement eFitness = eBest.Element("Fitness");
            Fitness = Double.Parse(eFitness.Value);

            XElement eValues = eBest.Element("Values");
            int i = 0;
            foreach (XElement eValue in eValues.Elements("Value"))
            {
                Values[i] = Double.Parse(eValue.Value);
                ++i;
            }
        }

    }

}

