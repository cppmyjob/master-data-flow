using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Utils
{

    public class ConsoleLoggerOutputFactory : ILoggerOutputFactory
    {
        public ILoggerOutput Create()
        {
            return new ConsoleLoggerOutput();
        }
    }

    public class ConsoleLoggerOutput : ILoggerOutput
    {
        public void Error(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void Error(string message, params object[] args)
        {
            Console.Error.WriteLine(message, args);
        }

        public void Error(string message, Exception exception)
        {
            Console.Error.WriteLine("{0} {1} {2}", message,  exception.Message, exception.StackTrace);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}
