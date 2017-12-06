using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Trading.Genetic
{
    public class FNGeneticManager
    {
        // Сколько баров будем проверять.
        public const int BAR_COUNT = 48;
        // Количество периодов для AVG
        public const int MAX_PERIOD = 24;

        // Количество средних линий
        public const int AVG_COUNT = 3;
        // ALPHA 
        public const int ALPHA_COUNT = 1;
        //
        public const int STOP_LOSS_COUNT = 1;

        // Дополнительные параметры для нейроной сети
        public const int ADD_PARAMETERS = 6;

        // Число точек после запятой
        public const int DECIMALS = 4;
    }
}
