using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Interfaces
{
    public interface IValueClone<out TValue>
    {
        TValue Clone();
    }
}
