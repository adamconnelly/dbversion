namespace dbversion.Console
{
    using System.ComponentModel.Composition;

    /// <summary>
    /// Provides methods for outputting messages to the console.
    /// </summary>
    [Export(typeof(IMessageService))]
    public class ConsoleMessageService : IMessageService
    {
        /// <summary>
        /// Writes the specified message out to the console.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
