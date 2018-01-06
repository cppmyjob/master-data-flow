using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AD.Common.DataStructures;
using AD.Common.Helpers;
using ADClientSDK;


namespace MasterDataFlow.Trading.Importer
{
    public class AlfaDirectImporter
    {
        public static void Execute()
        {
            Packer.Initialize();
            AdClient client = new AdClient();
            client.OnTerminalConnectionChanged += status =>
            {
                switch (status)
                {
                    case ConnectionStatus.Connected:
                        Console.WriteLine(
                            String.Format("Версии библиотеки {0}({1}), версии терминала {2}({3}). ",
                                AdClient.Version,
                                AdClient.ProtocolVersion,
                                AdClient.TerminalVersion,
                                AdClient.ProtocolTerminalVersion));
                        break;
                    case ConnectionStatus.Authorized:
                        Console.WriteLine("Авторизация терминала прошла успешно.");
                        break;
                    case ConnectionStatus.Disconnected:
                        Console.WriteLine("Терминал отсоединен.");
                        break;
                    case ConnectionStatus.Connecting:
                        Console.WriteLine("Ожидание подключения терминала.");
                        break;
                }
            };

            while (client.GetConnectionStatus(FrontEndType.AuthAndOperInitServer) != ConnectionStatus.Authorized)
            {
                Thread.Sleep(2000);
                Console.WriteLine("Authorize Waiting");
            }

            Console.WriteLine("Ready to import");


            var manualResetEvent = new ManualResetEvent(false);

            var id = GetIdfi(client, "SBER");
            //var id = GetIdfi(client, "AFLT");

            var archiveIndex = Int32.MinValue;

            client.Archive.OnChartArchive += (entities, i) =>
                                                {
                                                    if (i == archiveIndex)
                                                    {
                                                        CreateCsvFile(entities);
                                                        manualResetEvent.Set();
                                                    }
                                                     
                                                };

            //archiveIndex = client.Archive.RequestChartArchive(CandleType.Standard, id, DateTime.Today,
            //    BaseTimeFrame.Hour, (int)(365 * 1.95));

            archiveIndex = client.Archive.RequestChartArchive(CandleType.Standard, id, DateTime.Today.AddDays(-30),
                BaseTimeFrame.Minute, (int)(30));


            if (!manualResetEvent.WaitOne(new TimeSpan(0,5, 0)))
            {
                Console.Error.WriteLine("Can't load data");
            }
            else
            {
                Console.WriteLine("Import has been finished");
            }
            Console.ReadLine();
        }

        private static void CreateCsvFile(ChartArchiveEntity[] entities)
        {
            var filename = "outputMinutes.csv";
            if (File.Exists(filename))
                File.Delete(filename);
            using (var writer = new StreamWriter(filename))
            {
                // DateTime,Open,High,Low,Close,Volume

                string[] headers = new string[] { "DateTime", "Open", "High", "Low", "Close", "Volume" };
                writer.Write(string.Join(",", headers));
                writer.WriteLine();

                var data = entities.SelectMany(t => t.Candles).Cast<OhlcvDataPointEntity>().OrderBy(t => t.DateTime);

                foreach (var candle in data)
                {
                    writer.Write(candle.DateTime.ToString("o", CultureInfo.InvariantCulture));
                    writer.Write(",");
                    writer.Write(candle.Open.ToString(CultureInfo.InvariantCulture));
                    writer.Write(",");
                    writer.Write(candle.High.ToString(CultureInfo.InvariantCulture));
                    writer.Write(",");
                    writer.Write(candle.Low.ToString(CultureInfo.InvariantCulture));
                    writer.Write(",");
                    writer.Write(candle.Close.ToString(CultureInfo.InvariantCulture));
                    writer.Write(",");
                    writer.Write(candle.Volume.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine();
                }
            }
        }


        private static int GetIdfi(AdClient client, string name)
        {
            var objects = client.Dictionaries.GetObjects().Where(t => t.SymbolObject == name && t.IdObjectType == ObjectType.AO).ToArray();

            var instruments = client.Dictionaries.GetFinInstruments().Where(t => t.IdObject == objects[0].IdObject).ToArray();


            foreach (var fi in instruments)
            {
                var obj = client.Dictionaries.GetMarketBoardByIdFi(fi.IdFi);
                if (obj != null && obj.NameMarketBoard == "МБ ЦК")
                {
                    //Console.WriteLine("IdFi:{0}", fi.IdFi);
                    return fi.IdFi;
                }
            }

            return 0;

        }
    }
}
