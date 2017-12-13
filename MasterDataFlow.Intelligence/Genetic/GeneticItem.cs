using System;
using System.CodeDom;
using System.Xml.Linq;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public abstract class GeneticItem<TValue>
    {
        protected TValue[] _values;
        private double _fitness;
        private GeneticItemInitData _initData;
        private TValue[] _oldValues;
        private GeneticHistory<GeneticItem<TValue>, TValue> _history;
        private Guid _guid = Guid.NewGuid();

        protected GeneticItem()
        {
            
        }

        protected GeneticItem(GeneticItemInitData initData)
        {
            _initData = initData;
            _values = new TValue[initData.Count];
            if (initData.IsAddHistory)
                _history = new GeneticHistory<GeneticItem<TValue>, TValue>();
        }

        public GeneticItemInitData InitData
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

        public virtual GeneticItem<TValue> Clone()
        {
            var result = (GeneticItem<TValue>)Activator.CreateInstance(this.GetType());
            var isCloneInterface = typeof(TValue).IsAssignableFrom(typeof(IValueClone<TValue>));
            result._values = new TValue[_initData.Count];
            for (int i = 0; i < result._values.Length; i++)
            {
                if (!isCloneInterface)
                {
                    result._values[i] = _values[i];
                }
                else
                {
                    result._values[i] = ((IValueClone<TValue>)_values[i]).Clone();
                }
            }
            result._fitness = _fitness;
            result._initData = _initData;
            return result;
        }

        public TValue[] Values
        {
            get { return _values; }
        }

        public GeneticHistory<GeneticItem<TValue>, TValue> History
        {
            get { return _history; }
        }

        public Guid Guid
        {
            get { return _guid; }
        }

        public abstract TValue CreateValue(IRandom random);
        public abstract TValue ParseStringValue(string value);

        protected internal virtual void InitOtherValues(IRandom random) { }

        protected internal virtual void SaveValues()
        {
            if (_oldValues == null)
                _oldValues = new TValue[_values.Length];
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
            best.Add(new XElement("Guid", Guid.ToString()));

            XElement values = new XElement("Values");
            best.Add(values);
            foreach (TValue value in Values)
            {
                values.Add(new XElement("Value", value.ToString()));
            }
        }


        public virtual void Read(XElement root)
        {
            XElement eBest = root.Element("Item");

            XElement eFitness = eBest.Element("Fitness");
            Fitness = Double.Parse(eFitness.Value);

            XElement guid = eBest.Element("Guid");
            if (guid != null)
            {
                _guid = Guid.Parse(guid.Value);
            }

            XElement eValues = eBest.Element("Values");
            int i = 0;
            foreach (XElement eValue in eValues.Elements("Value"))
            {
                Values[i] = ParseStringValue(eValue.Value);
                ++i;
            }
        }

    }

}

