namespace MasterDataFlow.Trading.Ui.Business.Teacher
{
    public class TradingCommandController : BaseCommandController
    {
        public TradingCommandController(DataProvider dataProvider, int processorCount) : 
            base(dataProvider, processorCount)
        {
        }
    }
}
