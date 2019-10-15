using System;
using System.Collections.Generic;
using System.Web;
using log4net;
using System.IO;
using System.Diagnostics;

namespace MimerConsumerConsoleApplication.Utils
{
    public class Log
    {
        // Log level names are taken from: http://en.wikipedia.org/wiki/Syslog#Severity_levels
        // The meaning of some of the log levels differs slightly from the standard above, 
        // due to the need to distinguish between problems that cancels the action and problems that do not
        public enum Level
        {
            Emergency = 0,      // Currently not used but reserved for panic situations
            Alert = 1,          // Currently not used but reserved for very critical errors that must be addressed immediately.
            Critical = 2,       // Action was cancelled due to another Exception (typically a program or system issue)
            Error = 3,          // Action was cancelled due to a VisibleException (typically a user problem)
            Warning = 4,        // A error has occured but the action was not cancelled. Could still be quite critical.
            Notice = 5,         // Events that are unusual but not error conditions
            Info = 6,           // Normal operational messages
            Debug = 7           // Currently not used but reserved for information useful to developers for debugging the application
        };

        static readonly ILog m_FlowLog;
        static readonly ILog m_ExceptionLog;
        static readonly bool isUserInteractive;
        static readonly string m_GarblogFileName;
        static bool m_IsConfigured;

        static Log()
        {
            try
            {
                m_FlowLog = LogManager.GetLogger("FlowLog");
                m_ExceptionLog = LogManager.GetLogger("ExceptionLog");
                isUserInteractive = Environment.UserInteractive;
                m_GarblogFileName = "Garblog.txt";
                m_IsConfigured = IsConfigured();
                if (!m_IsConfigured)
                {
                    Log.WriteToGarblogFile("log4net has not been configured");
                }
            }
            catch (Exception ex)
            {
                //something went wrong during initialisation of log4net - log this to garblog
                Log.WriteToGarblogFile(ex);
            }
        }

        // Report a normal operation message
        public static void Write(object obj)
        {
            Write(obj.ToString());
        }

        public static void Write(string message)
        {
            WriteFlow(Level.Info, message, null);
        }

        // Report an unusual event, something that might give a problem in the future
        public static void WriteNotice(string message)
        {
            WriteFlow(Level.Notice, message, null);
        }

        // Report an error that does not cancel the current action
        public static void WriteWarning(string message, Exception exception)
        {
            WriteFlow(Level.Warning, message, exception);
        }
        public static void WriteWarning(string message)
        {
            WriteFlow(Level.Warning, message, null);
        }


        static void WriteFlow(Level level, string message, Exception exception)
        {
            string fullMessage = message;
            for (Exception excep = exception; excep != null; excep = excep.InnerException)
                fullMessage += " -> " + excep.Message;

            Write(m_FlowLog, level, "  " + fullMessage, null);

            if (isUserInteractive)
                Console.WriteLine(fullMessage);
        }

        public static void WriteFlow(Level level, string message)
        {
            Write(m_FlowLog, level, message, null);
        }

        public static void WriteException(Level level, string message, Exception exception)
        {
            WriteFlow(level, message, exception);
            Write(m_ExceptionLog, level, message, exception);
        }

        public static bool IsEnabledFlow(Level level)
        {
            return IsEnabled(m_FlowLog, level);
        }

        static void Write(ILog log, Level level, string message, Exception exception)
        {
            //write to garblog if log4net is not configured
            if (!m_IsConfigured)
            {
                if (exception != null)
                    WriteToGarblogFile(exception);
                else
                    WriteToGarblogFile(message);
            }
            else
            {
                switch (level)
                {
                    case Level.Emergency: log.Fatal(message, exception); break;
                    case Level.Alert: log.Fatal(message, exception); break;
                    case Level.Critical: log.Error(message, exception); break;
                    case Level.Error: log.Error(message, exception); break;
                    case Level.Warning: log.Warn(message, exception); break;
                    case Level.Notice: log.Info(message, exception); break;
                    case Level.Info: log.Info(message, exception); break;
                    case Level.Debug: log.Debug(message, exception); break;
                }
            }
        }

        static bool IsEnabled(ILog log, Level level)
        {
            switch (level)
            {
                case Level.Emergency: return log.IsFatalEnabled;
                case Level.Alert: return log.IsFatalEnabled;
                case Level.Critical: return log.IsErrorEnabled;
                case Level.Error: return log.IsErrorEnabled;
                case Level.Warning: return log.IsWarnEnabled;
                case Level.Notice: return log.IsInfoEnabled;
                case Level.Info: return log.IsInfoEnabled;
                case Level.Debug: return log.IsDebugEnabled;
                default: return false;
            }
        }

        public static bool IsConfigured()
        {
            return LogManager.GetRepository().Configured;
        }

        public static void WriteToGarblogFile(string message)
        {
            string exePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string garblogPath = exePath + m_GarblogFileName;
            if (!File.Exists(garblogPath))
            {
                using (StreamWriter sw = File.CreateText(garblogPath))
                {
                    sw.WriteLine(String.Format("{0}:{1}", DateTime.Now.ToString(), message.Trim()));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(garblogPath))
                {
                    sw.WriteLine(String.Format("{0}:{1}", DateTime.Now.ToString(), message.Trim()));
                }
            }
            //log to event log as an addition
            EventLog.WriteEntry("MimerConsumer", message);
        }

        public static void WriteToGarblogFile(Exception exception)
        {
            string exePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string garblogPath = exePath + m_GarblogFileName;
            if (!File.Exists(garblogPath))
            {
                using (StreamWriter sw = File.CreateText(garblogPath))
                {
                    sw.WriteLine(String.Format("{0}:{1}->{2}", DateTime.Now.ToString(), exception.GetType(), exception.Message));
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(garblogPath))
                {
                    sw.WriteLine(String.Format("{0}:{1}->{2}", DateTime.Now.ToString(), exception.GetType(), exception.Message));
                }
            }
            //log to event log as an addition
            EventLog.WriteEntry("MimerConsumer", String.Format("{0}:{1}->{2}", DateTime.Now.ToString(), exception.GetType(), exception.Message), EventLogEntryType.Error);
        }
    }

    public class ContextLogger
    {
        private string Context { get; set; }

        public ContextLogger(string context)
        {
            Context = "[" + context + "] ";
        }

        public void Write(string message)
        {
            Log.Write(Context + message);
        }

        public void WriteException(Log.Level level, string message, Exception e)
        {
            Log.WriteException(level, Context + message, e);
        }

        public void WriteWarning(string message)
        {
            Log.WriteWarning(Context + message);
        }
    }
}

