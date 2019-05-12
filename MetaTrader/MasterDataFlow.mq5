//+------------------------------------------------------------------+
//|                                               MasterDataFlow.mq5 |
//|                                                   Pavel Tsukanov |
//|                                                                  |
//+------------------------------------------------------------------+
#property copyright "Pavel Tsukanov"
#property link      ""
#property version   "1.00"

//--- inputs for expert
//input string             Symbol                      ="AFLT";


#include <Trade\Trade.mqh>
CTrade trade;

// https://www.mql5.com/ru/articles/249
#import "MasterDataFlow.MetaTrader.dll"
   int AdvisorInit(string &response);
   int AdvisorTick(double& history[], int size);
#import

// КАК САМОСТОЯТЕЛЬНО СОЗДАТЬ И ПРОТЕСТИРОВАТЬ В METATRADER 5 ИНСТРУМЕНТЫ МОСКОВСКОЙ БИРЖИ 
// https://www.mql5.com/ru/articles/5303

// ------- Private values ---------

string symbol;  
ENUM_TIMEFRAMES timeframe;
int historyWidowLength = 27;

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit() {

   symbol = _Symbol;
   timeframe = PERIOD_H1;

   SendInitRequest();
  //SendTickRequest();
  
  return(INIT_SUCCEEDED);
}
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
   
  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
void OnTick() {
   int signal =GetSignal();
}

void SendInitRequest() {
   string str = "                                                     ";
   Print("Response from Init");
   Print(AdvisorInit(str));
   Print(str);
}

int GetSignal() { 

   MqlRates rates[];
   
   int arraySize = CopyRates(symbol, timeframe, 0, 20, rates);
   if (arraySize < 0)
      return 0.0;

   double i1buffer[];
   ArrayResize(i1buffer, historyWidowLength);
   GetRsi14(i1buffer);
   //ArrayReverse(i1buffer);
   
   AdvisorTick(i1buffer, historyWidowLength);

   return 1;
}

// CopyBuffer
// https://www.mql5.com/ru/docs/series/copybuffer

// https://www.mql5.com/ru/docs/indicators/irsi
int GetRsi14(double &buffer[]) {
   return GetRsi(14, buffer);
}

int GetRsi(int period, double &buffer[]) {
   int handle = iRSI(symbol,timeframe,period,PRICE_CLOSE); 
    
   if(handle==INVALID_HANDLE) { 
      //--- сообщим о неудаче и выведем номер ошибки 
      PrintFormat("Не удалось создать хэндл индикатора iRSI для пары %s/%s/%s, код ошибки %d", 
                  symbol, 
                  EnumToString(timeframe), 
                  period, 
                  GetLastError()); 
      //--- работа индикатора завершается досрочно 
      return(INIT_FAILED); 
   }   
   
   CopyBuffer(handle,0, 2, historyWidowLength, buffer);
   
   return(INIT_SUCCEEDED);  
    
}



/*
void SendInitRequest() 
{
   string cookie=NULL,headers; 
   char   post[],result[]; 
   string url="http://vcap.me:9000/api/advisor/init"; 
   ResetLastError(); 
   int res=WebRequest("GET",url,cookie,NULL,500,post,0,result,headers); 
   if(res==-1) { 
      Print("Ошибка в WebRequest. Код ошибки  =",GetLastError()); 
      
      // MessageBox("Необходимо добавить адрес '"+url+"' в список разрешенных URL во вкладке 'Советники'","Ошибка",MB_ICONINFORMATION); 
   } 
   else { 
      if(res==200) { 
         //--- успешная загрузка 
         PrintFormat("Файл успешно загружен, размер %d байт.",ArraySize(result)); 
         string strResult = CharArrayToString(result, 0, WHOLE_ARRAY, CP_UTF8);
         Print(strResult); 
      } 
      else 
         PrintFormat("Ошибка загрузки '%s', код %d",url,res); 
   } 
}

void SendTickRequest() 
{
   string url="http://vcap.me:9000/api/advisor/tick"; 
   string request_headers="Content-Type: application/x-www-form-urlencoded";
   char body[];
   StringToCharArray("value=4444", body);
   string response_headers; 
   char result[];
   ResetLastError(); 
   int res=WebRequest("POST",url,request_headers,500,body,result,response_headers);
   if (res==-1) {
      Print("Ошибка в WebRequest. Код ошибки  =",GetLastError()); 
   } else {
      if(res==200) { 
         string strResult = CharArrayToString(result, 0, WHOLE_ARRAY, CP_UTF8);
         Print(strResult); 
      } else {
         Print("Respose Code =",GetLastError()); 
      // Error
      }
   }
}
*/