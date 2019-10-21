using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Trading.Configs
{
    public class CommandInitDataConfigSection : ConfigurationSection
    {
        public static CommandInitDataConfigSection GetConfig()
        {
            var retval = ConfigurationManager.GetSection("commandInitData") as CommandInitDataConfigSection;

            return retval;
        }

        [ConfigurationProperty("processorCount", DefaultValue = 0)]
        public int ProcessorCount
        {
            get
            {
                return (int)base["processorCount"];
            }
        }


    }
}
