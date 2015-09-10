using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Interfaces
{
    public interface ILoggerOutput
    {
        void Error(string message);
        void Error(string message, params object[] args);
        void Error(string message, Exception exception);
        void Info(string message);
        void Info(string message, params object[] args);
        void Debug(string message);
        void Debug(string message, params object[] args);
        void Warn(string message);
        void Warn(string message, params object[] args);
    }
}
