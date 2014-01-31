using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow
{
    public abstract class BaseCommandItem
    {
        public abstract BaseCommand GetInstance();
    }

}
