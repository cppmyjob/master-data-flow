using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Utils
{
    public class Logger : ILoggerOutput
    {
        // ReSharper disable InconsistentNaming
        private static volatile ILoggerOutputFactory _factory = null;
        private static volatile ILoggerOutput _logger = null;
        private static readonly Logger _instance = new Logger();
        // ReSharper restore InconsistentNaming

        public static Logger Instance
        {
            get
            {
                if (_logger == null && _factory != null)
                {
                    _logger = _factory.Create();
                }
                return _instance;
            }
        }

        public static void SetFactory(ILoggerOutputFactory factory)
        {
            _factory = factory;
        }

        public static void StopLogging()
        {
            _factory = null;
            _logger = null;
        }

        public void Error(string message)
        {
            if (_logger == null)
                return;
            _logger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            if (_logger == null)
                return;
            _logger.Error(String.Format(message, args));
        }

        public void Error(string message, Exception exception)
        {
            if (_logger == null)
                return;
            _logger.Error(message, exception);
        }

        public void Info(string message)
        {
            if (_logger == null)
                return;
            _logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            if (_logger == null)
                return;
            _logger.Info(String.Format(message, args));
        }

        public void Debug(string message)
        {
            if (_logger == null)
                return;
            _logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            if (_logger == null)
                return;
            _logger.Debug(String.Format(message, args));
        }

        public void Warn(string message)
        {
            if (_logger == null)
                return;
            _logger.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            if (_logger == null)
                return;
            _logger.Warn(String.Format(message, args));
        }

    }

}
