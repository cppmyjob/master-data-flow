//+------------------------------------------------------------------+
//|                                               MasterDataFlow.mq5 |
//|                                                   Pavel Tsukanov |
//|                                                                  |
//+------------------------------------------------------------------+
#property copyright "Pavel Tsukanov"
#property link      ""
#property version   "1.00"


#define MACD_MAGIC 785342349

#include <Trade\Trade.mqh>
#include <Trade\SymbolInfo.mqh>
#include <Trade\PositionInfo.mqh>
#include <Trade\AccountInfo.mqh>

// https://www.mql5.com/ru/articles/249
#import "MasterDataFlow.MetaTrader.dll"
   int AdvisorInit(string &response);
   int AdvisorLog(string &message);
   int AdvisorSetCurrentTime(string &message);
   int AdvisorTick(double& history[], int size);
#import


double InpLots          = 1; // Lots

// КАК САМОСТОЯТЕЛЬНО СОЗДАТЬ И ПРОТЕСТИРОВАТЬ В METATRADER 5 ИНСТРУМЕНТЫ МОСКОВСКОЙ БИРЖИ 
// https://www.mql5.com/ru/articles/5303

// -------- Advisor Class 
class MaterDataFlowExpert
  {
private:
   CTrade            m_trade;                      // trading object
   CSymbolInfo       m_symbol;                     // symbol info object
   CPositionInfo     m_position;                   // trade position object
   CAccountInfo      m_account;                    // account info wrapper
   ENUM_TIMEFRAMES   m_timeframe;
   int               m_historyWidowLength;
   
   //--- indicators
   int               m_handle_macd;                // MACD indicator handle
   int               m_handle_rsi_3;
   int               m_handle_rsi_7;
   int               m_handle_rsi_14;
   int               m_handle_rsi_21;
   int               m_handle_rsi_28;
   
   int GetMacd(double &buffer[], int buffer_num);
   bool IsNewBar();
   int GetSignal();   
   int AddIndicatorIntoBuffer(string name, double &buffer[], int offset);
   int GetIndicator(string name, double &buffer[]);
   int GetRsi(int period, double &buffer[]);

   int GetMacdLine(double &buffer[]);
   int GetMacdSignalLine(double &buffer[]);
   int GetMacdHistogram(double &buffer[]);
   bool InitIndicators(void);
   void ProcessSignal(int signal);
   bool ClosePosition();
   bool Buy();
   bool Sell();
   
//protected:
public:
                     MaterDataFlowExpert(void);
                     ~MaterDataFlowExpert(void);
   bool              Init(void);
   void              Deinit(void);
   bool              Processing(void);

  };
  
MaterDataFlowExpert ExtExpert;  
  
//+------------------------------------------------------------------+
//| Constructor                                                      |
//+------------------------------------------------------------------+
MaterDataFlowExpert::MaterDataFlowExpert(void) : 
                                    m_timeframe(PERIOD_H1),
                                    m_historyWidowLength(27),
                                    m_handle_macd(INVALID_HANDLE),
                                    m_handle_rsi_3(INVALID_HANDLE),
                                    m_handle_rsi_7(INVALID_HANDLE),
                                    m_handle_rsi_14(INVALID_HANDLE),
                                    m_handle_rsi_21(INVALID_HANDLE),
                                    m_handle_rsi_28(INVALID_HANDLE)

  {
  }

  
//+------------------------------------------------------------------+
//| Destructor                                                       |
//+------------------------------------------------------------------+
MaterDataFlowExpert::~MaterDataFlowExpert(void)
  {
  }  

bool MaterDataFlowExpert::Init(void)
  {
//--- initialize common information
   m_symbol.Name(Symbol());                  // symbol
   m_trade.SetExpertMagicNumber(MACD_MAGIC); // magic
   m_trade.SetMarginMode();
   m_trade.SetTypeFillingBySymbol(Symbol());

// ------- Send Init to dll

   string str = "                                                     ";
   Print("Response from Init");
   Print(AdvisorInit(str));
   Print(str);
   
   if(!InitIndicators())
      return(false);   
   
   return (true);
  }

bool MaterDataFlowExpert::Processing(void)
   {
//--- refresh rates
      if(!m_symbol.RefreshRates())
         return(false);   
   
      if (IsNewBar()) 
         {
            int signal = GetSignal();
            ProcessSignal(signal);
         }

      return (true);
   }
  
void MaterDataFlowExpert::ProcessSignal(int signal)
   {
      switch(signal) 
         {
      case 0: // Hold
         break;
      case 1: // Close
         if(m_position.Select(m_symbol.Name()))
            {
               if (!ClosePosition())
                  return;
            }
         break;      
      case 2: // Sell
         if(m_position.Select(m_symbol.Name()))
            {
               if(m_position.PositionType() == POSITION_TYPE_SELL)
                  return;
            
               if (!ClosePosition())
                  return;
            }
         break;
      case 3: // Buy
         if(m_position.Select(m_symbol.Name()))
            {
               if(m_position.PositionType() == POSITION_TYPE_BUY)
                  return;
            
               if (!ClosePosition())
                  return;
            }
         break;
         }


      switch(signal) 
         {
      case 0: // Hold
         break;
      case 1: // Close
         break;
      case 2: // Sell
         Sell();
         break;
      case 3: // Buy
         Buy();
         break;
      
         }
   
   }
   
bool MaterDataFlowExpert::Buy() {

   double price=m_symbol.Ask();

   //printf("price=%f, MarginCheck = %f", price, m_account.MarginCheck(m_symbol.Name(),ORDER_TYPE_BUY,InpLots,price));

   //--- check for free money
   if(m_account.FreeMarginCheck(m_symbol.Name(),ORDER_TYPE_BUY,InpLots,price) < 0.0)
      printf("We have no money. Free Margin = %f",m_account.FreeMargin());
   else
     {
      //--- open position
      if(m_trade.PositionOpen(m_symbol.Name(),ORDER_TYPE_BUY,InpLots,price,0.0,0.0))
         printf("Position by %s to be opened",m_symbol.Name());
      else
        {
         printf("Error opening BUY position by %s : '%s'",m_symbol.Name(),m_trade.ResultComment());
         printf("Open parameters : price=%f",price);
        }
     }
   //--- in any case we must exit from expert



   return (true);
}   

bool MaterDataFlowExpert::Sell() {

   double price=m_symbol.Bid();
   
   //printf("price=%f, MarginCheck = %f", price, m_account.MarginCheck(m_symbol.Name(),ORDER_TYPE_SELL,InpLots,price));
   
   //--- check for free money
   if(m_account.FreeMarginCheck(m_symbol.Name(),ORDER_TYPE_SELL,InpLots,price)<0.0)
      printf("We have no money. Free Margin = %f",m_account.FreeMargin());
   else
     {
      //--- open position
      if(m_trade.PositionOpen(m_symbol.Name(),ORDER_TYPE_SELL,InpLots,price,0.0,0.0))
         printf("Position by %s to be opened",m_symbol.Name());
      else
        {
         printf("Error opening SELL position by %s : '%s'",m_symbol.Name(),m_trade.ResultComment());
         printf("Open parameters : price=%f",price);
        }
     }
   //--- in any case we must exit from expert
   

   return (true);
}   

   
bool MaterDataFlowExpert::ClosePosition() {

   //--- close position
   if(m_trade.PositionClose(m_symbol.Name())) 
      {
         printf("Position by %s to be closed",Symbol());
         return (true);
      }
   else 
      {
         printf("Error closing position by %s : '%s'",Symbol(),m_trade.ResultComment());
         return (true);
      }
   //--- processed and cannot be modified   
}   
  
//+------------------------------------------------------------------+
//| Возвращает true, если появился новый бар для пары символ/период  |
//+------------------------------------------------------------------+
bool MaterDataFlowExpert::IsNewBar()
  {
//--- в статической переменной будем помнить время открытия последнего бара
   static datetime last_time=0;
//--- текущее время
   datetime lastbar_time=SeriesInfoInteger(m_symbol.Name(), m_timeframe,SERIES_LASTBAR_DATE);

//--- если это первый вызов функции
   if(last_time==0)
     {
      //--- установим время и выйдем 
      last_time=lastbar_time;
      return(false);
     }

//--- если время отличается
   if(last_time!=lastbar_time)
     {
     
      MqlDateTime mqlDateTime;
      TimeToStruct(TimeCurrent(),mqlDateTime);
      //--- запомним время и вернем true
      if (mqlDateTime.min > 30) {
         return(false);
      }
      
      last_time=lastbar_time;
      return(true);
     }
//--- дошли до этого места - значит бар не новый, вернем false
   return(false);
  }


bool MaterDataFlowExpert::InitIndicators(void)
  {
//--- create MACD indicator
   if(m_handle_macd==INVALID_HANDLE)
      if((m_handle_macd=iMACD(m_symbol.Name(), m_timeframe,12,26,9,PRICE_CLOSE))==INVALID_HANDLE)
        {
         printf("Error creating MACD indicator");
         return(false);
        }
        
   if(m_handle_rsi_3==INVALID_HANDLE)
      if((m_handle_rsi_3=iRSI(m_symbol.Name(), m_timeframe, 3, PRICE_CLOSE))==INVALID_HANDLE)
        {
         printf("Error creating RSI 3 indicator");
         return(false);
        }

   if(m_handle_rsi_7==INVALID_HANDLE)
      if((m_handle_rsi_7=iRSI(m_symbol.Name(), m_timeframe, 7, PRICE_CLOSE))==INVALID_HANDLE)
        {
         printf("Error creating RSI 7 indicator");
         return(false);
        }

   if(m_handle_rsi_14==INVALID_HANDLE)
      if((m_handle_rsi_14=iRSI(m_symbol.Name(), m_timeframe, 14, PRICE_CLOSE))==INVALID_HANDLE)
        {
         printf("Error creating RSI 14 indicator");
         return(false);
        }
        
//--- succeed
   return(true);
  }

int MaterDataFlowExpert::GetSignal() { 

   double buffer[];
   int buffer_length =  5 * m_historyWidowLength;
   ArrayResize(buffer, buffer_length);
   
   AddIndicatorIntoBuffer("MACD Line", buffer, 0 * m_historyWidowLength);
   AddIndicatorIntoBuffer("RSI 3", buffer, 1 * m_historyWidowLength);
   AddIndicatorIntoBuffer("MACD SignalLine", buffer, 2 * m_historyWidowLength);
   //AddIndicatorIntoBuffer("MACD Histogram", buffer, 2 * m_historyWidowLength);
   AddIndicatorIntoBuffer("RSI 14", buffer, 3 * m_historyWidowLength);
   AddIndicatorIntoBuffer("RSI 7", buffer, 4 * m_historyWidowLength);
   
   string current_date = TimeToString(TimeCurrent());   
   //PrintFormat("Current time: %s", current_date);
   AdvisorSetCurrentTime(current_date);
   
   int signal = AdvisorTick(buffer, buffer_length);
   //PrintFormat("Signal : %d", signal);
   return signal;
}

int MaterDataFlowExpert::AddIndicatorIntoBuffer(string name, double &buffer[], int offset) {

   double temp_buffer[];
   ArrayResize(temp_buffer, m_historyWidowLength);
   int handle = GetIndicator(name, temp_buffer);
   if(handle==INVALID_HANDLE) { 
      return INVALID_HANDLE;
   }
   ArrayCopy(buffer, temp_buffer, offset, 0, m_historyWidowLength);
   return handle;
}


int MaterDataFlowExpert::GetIndicator(string name, double &buffer[]) {
   
   if(name == "RSI 3")
      return GetRsi(3, buffer);
   else if(name == "RSI 7")
      return GetRsi(7, buffer);
   else if(name == "RSI 14")
      return GetRsi(14, buffer);
   else if(name == "RSI 21")
      return GetRsi(21, buffer);
   else if(name == "RSI 28")
      return GetRsi(28, buffer);
   else if(name == "MACD Histogram")
      return GetMacdHistogram(buffer);
   else if(name == "MACD SignalLine")
      return GetMacdSignalLine(buffer);
   else if(name == "MACD Line")
      return GetMacdLine(buffer);
      
   PrintFormat("Ошибочное имя индикатора %s", name);
   
   return(INIT_PARAMETERS_INCORRECT);  
}



// CopyBuffer
// https://www.mql5.com/ru/docs/series/copybuffer

// https://www.mql5.com/ru/docs/indicators/irsi

int MaterDataFlowExpert::GetRsi(int period, double &buffer[]) {
   int handle = INVALID_HANDLE; 
   switch(period) {
      case 3:
         handle = m_handle_rsi_3;
      break;
      case 7:
         handle = m_handle_rsi_7;
      break;
      case 14:
         handle = m_handle_rsi_14;
      break;
   }
   if(handle==INVALID_HANDLE) { 
      //--- сообщим о неудаче и выведем номер ошибки 
      PrintFormat("Не удалось создать хэндл индикатора iRSI для пары %s/%s/%s, код ошибки %d", 
                  m_symbol.Name(), 
                  EnumToString(m_timeframe), 
                  period, 
                  GetLastError()); 
      //--- работа индикатора завершается досрочно 
      return(INIT_FAILED); 
   }   
   
   CopyBuffer(handle, 0, 1, m_historyWidowLength, buffer);
   
   return(INIT_SUCCEEDED);  
}

int MaterDataFlowExpert::GetMacdLine(double &buffer[]) {
   return GetMacd(buffer, MAIN_LINE);
}

int MaterDataFlowExpert::GetMacdSignalLine(double &buffer[]) {
   return GetMacd(buffer, SIGNAL_LINE);
}

int MaterDataFlowExpert::GetMacdHistogram(double &buffer[]) {

   double main_buffer[], signal_buffer[];
   ArrayResize(main_buffer, m_historyWidowLength);
   ArrayResize(signal_buffer, m_historyWidowLength);

   int handle = GetMacd(main_buffer, MAIN_LINE);
   if(handle==INVALID_HANDLE) { 
      return handle;
   }

   handle = GetMacd(signal_buffer, SIGNAL_LINE);
   if(handle==INVALID_HANDLE) { 
      return handle;
   }

   for(int i = 0; i < m_historyWidowLength; i++) {
      buffer[i] = main_buffer[i] - signal_buffer[i];
   }

   return(INIT_SUCCEEDED);
}


int MaterDataFlowExpert::GetMacd(double &buffer[], int buffer_num) {
   //int handle = iMACD(m_symbol.Name(), m_timeframe, 12, 26, 9, PRICE_CLOSE); 
/*    
   if(handle==INVALID_HANDLE) { 
      //--- сообщим о неудаче и выведем номер ошибки 
      PrintFormat("Не удалось создать хэндл индикатора iRSI для пары %s/%s/%s, код ошибки %d", 
                  m_symbol.Name(), 
                  EnumToString(m_timeframe), 
                  buffer_num, 
                  GetLastError()); 
      //--- работа индикатора завершается досрочно 
      return(INIT_FAILED); 
   }   
*/   
   CopyBuffer(m_handle_macd, buffer_num, 1, m_historyWidowLength, buffer);
   //IndicatorRelease(handle);
   return(INIT_SUCCEEDED);  
    
}



//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit() 
   {

//--- create all necessary objects
   if(!ExtExpert.Init())
      return(INIT_FAILED);
//--- secceed
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
void OnTick() 
   {
   
      ExtExpert.Processing();
  
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