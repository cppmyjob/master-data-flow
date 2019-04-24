using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Ui.Business.Data;

namespace MasterDataFlow.Trading.Ui.Business.IO
{
    public class Reader
    {
        private readonly InputDataCollection _inputData = new InputDataCollection();

        public TradingItem ReadItem(TradingItemInitData itemInitData)
        {
            if (!File.Exists("genetic.save"))
                return null;

            StreamReader reader = new StreamReader("genetic.save");
            try
            {
                XElement root = XElement.Load(reader, LoadOptions.None);
                var item = CreateItem(itemInitData);
                ReadItemValues(root, item);
                return item;
            }
            finally
            {
                reader.Close();
            }
        }

        public TradingItemInitData ReadItemInitData()
        {
            if (!File.Exists("genetic.save"))
            {
                return new TradingItemInitData();
            }

            StreamReader reader = new StreamReader("genetic.save");
            try
            {
                XElement root = XElement.Load(reader, LoadOptions.None);
                var result = new TradingItemInitData();
                result.Read(root);
                return result;
            }
            finally
            {
                reader.Close();
            }
        }

        private void ReadItemValues(XElement root, TradingItem item)
        {
            XElement itemElement = root.Element("item");

            XElement eFitness = itemElement.Element("fitness");
            item.Fitness = Double.Parse(eFitness.Value);

            XElement efitnessZigZag = itemElement.Element("fitnessZigZag");
            if (efitnessZigZag != null)
                item.FitnessZigZag = Double.Parse(efitnessZigZag.Value);

            XElement efitnessProfit = itemElement.Element("fitnessProfit");
            if (efitnessProfit != null)
                item.FitnessProfit = Double.Parse(efitnessProfit.Value);

            XElement efitnessExpectedValue = itemElement.Element("fitnessExpectedValue");
            if (efitnessExpectedValue != null)
                item.FitnessExpectedValue = Double.Parse(efitnessExpectedValue.Value);

            XElement efitnessPlusMinusOrdersRatio = itemElement.Element("fitnessPlusMinusOrdersRatio");
            if (efitnessPlusMinusOrdersRatio != null)
                item.FitnessPlusMinusOrdersRatio = Double.Parse(efitnessPlusMinusOrdersRatio.Value);

            XElement efitnessPlusMinusEquityRatio = itemElement.Element("fitnessPlusMinusEquityRatio");
            if (efitnessPlusMinusEquityRatio != null)
                item.FitnessPlusMinusEquityRatio = Double.Parse(efitnessPlusMinusEquityRatio.Value);

            XElement guid = itemElement.Element("guid");
            item.Guid = Guid.Parse(guid.Value);

            XElement eValues = itemElement.Element("namedValues");
            int offset = 0;
            foreach (XElement eValue in eValues.Elements("v"))
            {
                var attributes = eValue.Attributes().ToArray();
                if (attributes.Any(t => t.Name.LocalName == "type" && t.Value == "indicator"))
                {
                    var name = attributes.Single(t => t.Name.LocalName == "name");
                    item.Values[offset] = GetIndicatorIndex(name.Value);
                }
                else
                {
                    item.Values[offset] = ParseFloat(eValue.Value);
                }
                ++offset;
            }

            eValues = itemElement.Element("values");
            foreach (XElement eValue in eValues.Elements("v"))
            {
                item.Values[offset] = ParseFloat(eValue.Value);
                ++offset;
            }

        }

        private float GetIndicatorIndex(string name)
        {
            var inputs = _inputData.GetInputs();
            var search = new BaseInput.Search(name);
            var index = inputs.FindIndex(search.Match);
            if (index < 0)
                throw new Exception("Invalid indicator name:" + name);
            return index;
        }

        private float ParseFloat(string value)
        {
            var result = Convert.ToDouble(value);
            return (float)result;
        }

        private TradingItem CreateItem(TradingItemInitData itemInitData)
        {
            TradingItem item = new TradingItem(itemInitData);
            return item;
        }


    }
}
