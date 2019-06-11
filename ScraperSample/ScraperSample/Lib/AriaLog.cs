using System.Configuration;
using Microsoft.Applications.Telemetry;
using Microsoft.Applications.Telemetry.Server;
using System.Diagnostics;
using System;
using Castle.Core.Logging;
using WebGrease; //added this because LogManager was not working 

namespace RecurrenceManager.Lib
{
    public class AriaLog
    {
        private static ILogger _logger = LogManager.Initialize(ConfigurationManager.AppSettings["AriaTenantToken"]);

        private static EventProperties _ScraperSampleProps
        {
            get
            {
                return new EventProperties
                {
                    Name = ConfigurationManager.AppSettings["ScraperSampleTable"],
                    Timestamp = DateTime.Now
                };
            }
        }

        private static EventProperties _UhrsQueuecProps
        {
            get
            {
                return new EventProperties
                {
                    Name = ConfigurationManager.AppSettings["UHRSQueueLogTable"],
                    Timestamp = DateTime.Now
                };
            }
        }

        private static EventProperties _ScrapeRecurrencesProps
        {
            get
            {
                return new EventProperties
                {
                    Name = ConfigurationManager.AppSettings["logTableName"],
                    Timestamp = DateTime.Now
                };
            }
        }

        public static void WriteLogEntry(Exception exception, EventLogEntryType type, string message = null)
        {
            try
            {
                var ae = exception as AggregateException;
                if (ae != null)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        Console.WriteLine($"{message} {e.Message} --> {e.StackTrace}");
                        _logger.LogEvent(GetErrorProps(type, e));
                    }
                }
                else
                {
                    Console.WriteLine($"{message} {exception.Message} --> {exception.StackTrace}");
                    _logger.LogEvent(GetErrorProps(type, exception));
                }
            }
            catch (Exception Ex)
            {

                Ex.ToString();
            }
        }

        public static void WriteUhrsQueueLogEntry(Exception ex, EventLogEntryType type, string message = null)
        {
            try
            {
                var ae = ex as AggregateException;
                if (ae != null)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        Console.WriteLine($"{message} {e.Message} --> {e.StackTrace}");
                        _logger.LogEvent(GetUhrsQueueErrorProps(type, e));
                    }
                }
                else
                {
                    Console.WriteLine($"{message} {ex.Message} --> {ex.StackTrace}");
                    _logger.LogEvent(GetUhrsQueueErrorProps(type, ex));
                }
            }
            catch
            {
                //no action
            }
        }

        public static void WriteUhrsQueueLogEntry(EventLogEntryType type, string message)
        {
            try
            {
                {
                    _logger.LogEvent(GetUhrsQueueErrorProps(type, message));
                }
            }
            catch
            {
                //no action
            }
        }

        public static void WriteScrapeRecurrencesLogEntry(EventLogEntryType type, string message)
        {
            try
            {
                {
                    _logger.LogEvent(GetErrorProps(type, message));
                }
            }
            catch
            {
                //no action
            }
        }

        private static EventProperties GetErrorProps(EventLogEntryType type, Exception ex)
        {
            return _ScrapeRecurrencesProps.SetProperty(type.ToString(), $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");

        }
        private static EventProperties GetErrorProps(EventLogEntryType type, string message)
        {
            return _ScrapeRecurrencesProps.SetProperty(type.ToString(), message);
        }


        private static EventProperties GetUhrsQueueErrorProps(EventLogEntryType type, Exception ex)
        {
            return _UhrsQueuecProps.SetProperty(type.ToString(), $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        private static EventProperties GetUhrsQueueErrorProps(EventLogEntryType type, string message)
        {
            return _UhrsQueuecProps.SetProperty(type.ToString(), message);
        }
    }
}
