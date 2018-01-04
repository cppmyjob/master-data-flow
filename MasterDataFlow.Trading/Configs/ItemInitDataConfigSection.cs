using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Trading.Configs
{
    public class ItemInitDataConfigSection : ConfigurationSection
    {
        public static ItemInitDataConfigSection GetConfig()
        {
            var retval = ConfigurationManager.GetSection("itemInitData") as ItemInitDataConfigSection;

            return retval;
        }

        //[ConfigurationProperty("maxAttempts")]
        //public int MaxAttempts
        //{
        //    get
        //    {
        //        return (int)base["maxAttempts"];
        //    }
        //}


    }
}
