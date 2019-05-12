//+------------------------------------------------------------------+
//|                                                   ExportData.mq5 |
//|                                                   Pavel Tsukanov |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Pavel Tsukanov"
#property link      "https://www.mql5.com"
#property version   "1.00"
//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit() {
//---
  MqlRates rates[];
  
   string terminal_data_path=TerminalInfoString(TERMINAL_DATA_PATH);
   
   Print("Data file path ", terminal_data_path); 
  
   datetime startTime=D'2019.04.24 00:00';
   int count = 10 * 24 * 365;
   int arraySize = CopyRates(_Symbol, PERIOD_H1, startTime, count, rates);
   
   int in =FileOpen(_Symbol+".csv",FILE_WRITE|FILE_ANSI|FILE_CSV);
   if(in==INVALID_HANDLE)
   {
      Alert("Не удалось открыть файл для записи");
      return(INIT_FAILED); 
   }
   
   string Caption ="DateTime,Open,High,Low,Close,Volume";
   FileWrite(in,Caption);


   for(int i=0;i<arraySize; i++) {
   
      MqlRates rate = rates[i];
      string fdate = TimeToString(rate.time);
      double open = DoubleToString(rate.open);
      double high = DoubleToString(rate.high);
      double low = DoubleToString(rate.low);
      double close = DoubleToString(rate.close);
      int vol = IntegerToString(rate.real_volume);
   
      string str =fdate+ ","+open+","+high+","+low+","+close+","+vol;
      FileWrite(in,str);
   }
   
   FileClose(in);
   
//---
   return(INIT_SUCCEEDED);
}
