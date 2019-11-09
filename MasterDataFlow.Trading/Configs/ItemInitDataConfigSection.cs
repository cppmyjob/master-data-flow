using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Trading.Configs
{
    public class ItemInitDataValidationOptimizerElement : ConfigurationElement
    {
        [ConfigurationProperty("isFilterBadResult", DefaultValue = true)]
        public bool IsFilterBadResult
        {
            get
            {
                return (bool)base["isFilterBadResult"];
            }
        }

        [ConfigurationProperty("isFilterBadResultBuySell", DefaultValue = false)]
        public bool IsFilterBadResultBuySell
        {
            get
            {
                return (bool)base["isFilterBadResultBuySell"];
            }
        }
    }

    public class ItemInitDataTrainingOptimizerElement : ConfigurationElement
    {
        [ConfigurationProperty("isFilterBadResult", DefaultValue = true)]
        public bool IsFilterBadResult
        {
            get
            {
                return (bool)base["isFilterBadResult"];
            }
        }


        [ConfigurationProperty("isFilterBadResultBuySell", DefaultValue = false)]
        public bool IsFilterBadResultBuySell
        {
            get
            {
                return (bool)base["isFilterBadResultBuySell"];
            }
        }

    }

    public class ItemInitDataFintessOptimizerElement : ConfigurationElement
    {
        [ConfigurationProperty("isExpectedValue", DefaultValue = true)]
        public bool IsExpectedValue
        {
            get
            {
                return (bool)base["isExpectedValue"];
            }
        }

        [ConfigurationProperty("isPlusMinusOrdersRatio", DefaultValue = true)]
        public bool IsPlusMinusOrdersRatio
        {
            get
            {
                return (bool)base["isPlusMinusOrdersRatio"];
            }
        }


        [ConfigurationProperty("isPlusMinusEquityRatio", DefaultValue = true)]
        public bool IsPlusMinusEquityRatio
        {
            get
            {
                return (bool)base["isPlusMinusEquityRatio"];
            }
        }

        [ConfigurationProperty("isProfit", DefaultValue = true)]
        public bool IsProfit
        {
            get
            {
                return (bool)base["isProfit"];
            }
        }

        [ConfigurationProperty("isZigZag", DefaultValue = true)]
        public bool IsZigZag
        {
            get
            {
                return (bool)base["isZigZag"];
            }
        }
        [ConfigurationProperty("validationPercent", DefaultValue = 0)]
        public int ValidationPercent
        {
            get
            {
                return (int)base["validationPercent"];
            }
        }


    }


    public class ItemInitDataOptimizerElement : ConfigurationElement
    {

        
        [ConfigurationProperty("validation")]
        public ItemInitDataValidationOptimizerElement Validation
        {
            get
            {
                return (ItemInitDataValidationOptimizerElement)base["validation"];
            }
        }

        [ConfigurationProperty("training")]
        public ItemInitDataTrainingOptimizerElement Training
        {
            get
            {
                return (ItemInitDataTrainingOptimizerElement)base["training"];
            }
        }


        [ConfigurationProperty("fitness")]
        public ItemInitDataFintessOptimizerElement Fitness
        {
            get
            {
                return (ItemInitDataFintessOptimizerElement)base["fitness"];
            }
        }


        [ConfigurationProperty("isValidationPlusMinusRatioLessTraining", DefaultValue = true)]
        public bool IsValidationPlusMinusRatioLessTraining
        {
            get
            {
                return (bool)base["isValidationPlusMinusRatioLessTraining"];
            }
        }
    }


    public class ItemInitDataConfigSection : ConfigurationSection
    {
        public static ItemInitDataConfigSection GetConfig()
        {
            var retval = ConfigurationManager.GetSection("itemInitData") as ItemInitDataConfigSection;

            return retval;
        }

        [ConfigurationProperty("optimizer")]
        public ItemInitDataOptimizerElement Optimizer
        {
            get
            {
                return (ItemInitDataOptimizerElement)base["optimizer"];
            }
        }

    }
}
