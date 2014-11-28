using System;
using System.Xml.Linq;
using MasterDataFlow.Intelligence.Genetic.Old;

namespace MasterDataFlow.Intelligence.Genetic
{
	public enum MoveDirection { None, Up, Down };

	public abstract class MoveGeneticItem : GeneticItem
	{
		private readonly double[] _steps;

		public MoveGeneticItem(GeneticInitData initData)
            : base(initData)
        {
			_steps = new double[initData.Count];
		}

		public double[] Steps
		{
			get { return _steps; }
		}

		protected internal override void InitOtherValues(Random random) 
		{
			for (int i = 0; i < InitData.Count; i++)
			{
				_steps[i] = CreateStep(random.NextDouble());
			}
		}

		internal double CreateStep(double random)
		{
			int power = (int)(random * 20) - 10;
			double result = 1 / Math.Pow(10, Math.Abs(power));
			if (power == 0)
				return 0;
			return power < 0 ? -result : result;
		}

		internal void MoveValues()
		{
			double[] values = Values;
			for (int i = 0; i < InitData.Count; i++)
			{
				double value = values[i] + _steps[i];
				if (value < 0)
					_steps[i] = -_steps[i];
				else
				{
					if (value > 1.0)
						_steps[i] = -_steps[i];
					else
						values[i] = value;
				}
			}
		}

        public override void Write(XElement root)
        {
            base.Write(root);
            XElement item = root.Element("Item");
            XElement values = new XElement("Steps");
            item.Add(values);
            foreach (double step in Steps)
            {
                values.Add(new XElement("Step", step.ToString()));
            }
        }


        public override void Read(XElement root)
        {
            base.Read(root);

            XElement eBest = root.Element("Item");

            XElement eSteps = eBest.Element("Steps");
            if (eSteps == null)
                return; // TODO
            int i = 0;
            foreach (XElement eStep in eSteps.Elements("Step"))
            {
                Steps[i] = Double.Parse(eStep.Value);
                ++i;
            }

        }

	}
}
