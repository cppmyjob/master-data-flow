using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDataFlow.Trading.Genetic
{
    public class FNGeneticManager
    {        // Сколько баров будем проверять.
        public const int BAR_COUNT = 48;
        // Количество периодов для AVG
        public const int MAX_PERIOD = 24;

        // Количество средних линий
        public const int AVG_COUNT = 3;

        // Дополнительные параметры для нейроной сети
        public const int ADD_PARAMETERS = 6;

    }
}
