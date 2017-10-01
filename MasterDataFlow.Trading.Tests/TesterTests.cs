using System;
using System.Collections.Generic;
using MasterDataFlow.Trading.Common;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.Tester;
using MasterDataFlow.Trading.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Trading.Tests
{
    [TestClass]
    public class TesterTests
    {
        private const double START_DEPOSIT = 1000;

        [TestMethod]
        public void FxDirectionProcessBuyPlus()
        {
            // INIT
            List<Bar> bars = new List<Bar>();
            bars.Add(new Bar() { High = 1.0, Open = 1.0, Close = 2.0, Low = 2.0 });
            bars.Add(new Bar() { High = 2.0, Open = 2.0, Close = 6.0, Low = 6.0 });
            bars.Add(new Bar() { High = 6.0, Open = 6.0, Close = 7.0, Low = 7.0 });

            List<FxDirection> dirs = new List<FxDirection>();
            dirs.Add(FxDirection.Up);
            dirs.Add(FxDirection.Up);
            dirs.Add(FxDirection.Up);

            // ACT
            var tester = new TesterMock(dirs.ToArray(), START_DEPOSIT,
                bars.ToArray(), 0, 3);
            tester.Decimals = 1;
            FxTesterResult result = tester.Run();

            // ASSERT
            Assert.AreEqual(1, result.OrderCount);
            Assert.AreEqual(5.7, result.MaxEquity);
            Assert.AreEqual(-0.3, result.MinEquity);
            Assert.AreEqual(0, result.MinusCount);
            Assert.AreEqual(1, result.MinusEquityCount);
            Assert.AreEqual(1, result.PlusCount);
            Assert.AreEqual(10, result.PlusEquityCount);
            Assert.AreEqual(5.7, result.Profit);
        }

        [TestMethod]
        public void FxDirectionProcessBuyMinus()
        {
            // INIT
            List<Bar> bars = new List<Bar>();
            bars.Add(new Bar() { High = 8.0, Open = 8.0, Close = 7.0, Low = 7.0 });
            bars.Add(new Bar() { High = 17.0, Open = 7.0, Close = 5.0, Low = 5.0 });
            bars.Add(new Bar() { High = 2.0, Open = 2.0, Close = 1.0, Low = 1.0 });

            List<FxDirection> dirs = new List<FxDirection>();
            dirs.Add(FxDirection.Up);
            dirs.Add(FxDirection.Up);
            dirs.Add(FxDirection.Up);

            // ACT
            var tester = new TesterMock(dirs.ToArray(), START_DEPOSIT,
                bars.ToArray(), 0, 3);
            tester.Decimals = 1;
            FxTesterResult result = tester.Run();

            // ASSERT
            Assert.AreEqual(1, result.OrderCount);
            Assert.AreEqual(8.7, result.MaxEquity);
            Assert.AreEqual(-7.3, result.MinEquity);
            Assert.AreEqual(1, result.MinusCount);
            Assert.AreEqual(10, result.MinusEquityCount);
            Assert.AreEqual(0, result.PlusCount);
            Assert.AreEqual(1, result.PlusEquityCount);
            Assert.AreEqual(-7.3, result.Profit);
        }



        [TestMethod]
        public void FxDirectionProcessSellPlus()
        {
            // INIT
            List<Bar> bars = new List<Bar>();
            bars.Add(new Bar() { High = 7.0, Open = 7.0, Close = 5.0, Low = 10.0 });
            bars.Add(new Bar() { High = 4.0, Open = 4.0, Close = 3.0, Low = 3.0 });
            bars.Add(new Bar() { High = 2.0, Open = 2.0, Close = 1.0, Low = 1.0 });

            List<FxDirection> dirs = new List<FxDirection>();
            dirs.Add(FxDirection.Down);
            dirs.Add(FxDirection.Down);
            dirs.Add(FxDirection.Down);

            // ACT
            var tester = new TesterMock(dirs.ToArray(), START_DEPOSIT,
                bars.ToArray(), 0, 3);
            tester.Decimals = 1;
            FxTesterResult result = tester.Run();

            // ASSERT
            Assert.AreEqual(1, result.OrderCount);
            Assert.AreEqual(5.7, result.MaxEquity);
            Assert.AreEqual(-3.3, result.MinEquity);
            Assert.AreEqual(0, result.MinusCount);
            Assert.AreEqual(2, result.MinusEquityCount);
            Assert.AreEqual(1, result.PlusCount);
            Assert.AreEqual(9, result.PlusEquityCount);
            Assert.AreEqual(5.7, result.Profit);
        }

        [TestMethod]
        public void FxDirectionProcessSellMinus()
        {
            // INIT
            List<Bar> bars = new List<Bar>();
            bars.Add(new Bar() { High = 1.0, Open = 1.0, Close = 2.0, Low = 2.0 });
            bars.Add(new Bar() { High = 2.0, Open = 2.0, Close = 6.0, Low = 6.0 });
            bars.Add(new Bar() { High = 6.0, Open = 6.0, Close = 7.0, Low = 7.0 });

            List<FxDirection> dirs = new List<FxDirection>();
            dirs.Add(FxDirection.Down);
            dirs.Add(FxDirection.Down);
            dirs.Add(FxDirection.Down);

            // ACT
            var tester = new TesterMock(dirs.ToArray(), START_DEPOSIT,
                bars.ToArray(), 0, 3);
            tester.Decimals = 1;
            FxTesterResult result = tester.Run();

            // ASSERT
            Assert.AreEqual(1, result.OrderCount);
            Assert.AreEqual(Double.MinValue, result.MaxEquity);
            Assert.AreEqual(-6.3, result.MinEquity);
            Assert.AreEqual(1, result.MinusCount);
            Assert.AreEqual(11, result.MinusEquityCount);
            Assert.AreEqual(0, result.PlusCount);
            Assert.AreEqual(0, result.PlusEquityCount);
            Assert.AreEqual(-6.3, result.Profit);
        }



        [TestMethod]
        public void FxDirectionProcessBuySell()
        {
            // INIT
            List<Bar> bars = new List<Bar>();
            bars.Add(new Bar() { High = 1.0, Open = 1.0, Close = -10.0, Low = -10 });
            bars.Add(new Bar() { High = -10.0, Open = -10.0, Close = 3.0, Low = 3.0 });
            bars.Add(new Bar() { High = 3.0, Open = 3.0, Close = 4.0, Low = 4.0 });
            bars.Add(new Bar() { High = 4.0, Open = 4.0, Close = 5.0, Low = 5.0 });
            bars.Add(new Bar() { High = 5.0, Open = 5.0, Close = 6.0, Low = 6.0 });
            bars.Add(new Bar() { High = 6.0, Open = 6.0, Close = 7.0, Low = 7.0 });
            bars.Add(new Bar() { High = 7.0, Open = 7.0, Close = 8.0, Low = 8.0 });

            List<FxDirection> dirs = new List<FxDirection>();
            dirs.Add(FxDirection.Up);
            dirs.Add(FxDirection.Up);
            dirs.Add(FxDirection.Up);
            dirs.Add(FxDirection.None);
            dirs.Add(FxDirection.Down);
            dirs.Add(FxDirection.Down);
            dirs.Add(FxDirection.Down);

            // ACT
            var tester = new TesterMock(dirs.ToArray(), START_DEPOSIT,
                bars.ToArray(), 0, 7);
            tester.Deposit = 1000000000;
            tester.Decimals = 1;
            FxTesterResult result = tester.Run();

            // ASSERT
            Assert.AreEqual(2, result.OrderCount, "OrderCount");
            Assert.AreEqual(2.7, result.MaxEquity, "MaxEquity");
            Assert.AreEqual(-11.3, result.MinEquity, "MinEquity");
            Assert.AreEqual(1, result.MinusCount, "MinusCount");
            //            Assert.AreEqual(3, result.MinusEquityCount, "MinusEquityCount");
            Assert.AreEqual(1, result.PlusCount, "PlusCount");
            //            Assert.AreEqual(1, result.PlusEquityCount, "PlusEquityCount");
            Assert.AreEqual(-0.6, result.Profit, 0.01, "Profit");
        }

    }

}
