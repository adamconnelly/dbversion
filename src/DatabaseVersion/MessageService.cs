using System;
using log4net;

namespace dbversion
{
    using System.ComponentModel.Composition;

    /// <summary>
    /// Provides methods for outputting messages to the console.
    /// </summary>
    [Export(typeof(IMessageService))]
    public class MessageService : IMessageService
    {
        public readonly ILog Log;

        public MessageService()
        {
            Log = LogManager.GetLogger(System.Reflection.Assembly.GetCallingAssembly(), "MessageServiceLogger");
            log4net.Config.XmlConfigurator.Configure(
                new System.IO.FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
        }

        /// <summary>
        /// Writes an empty line out to the console.
        /// </summary>
        public void WriteLine()
        {
            System.Console.WriteLine();
        }

        /// <summary>
        /// Writes the specified message out to the console.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);

            Log.Info(message);
        }

        /// <summary>
        /// Writes the specified message out to the logger
        /// </summary>
        /// <param name="message"></param>
        public void WriteDebugLine(string message)
        {
            Log.Debug(message);
        }

        /// <summary>
        /// Writes the specified message out to the console and the exception to the logger
        /// </summary>
        /// <param name="message"></param>
        public void WriteExceptionLine(string message, Exception exception)
        {
            System.Console.WriteLine(message);

            Log.Error(message, exception);
        }
    }
}
