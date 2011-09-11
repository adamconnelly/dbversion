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
        }
    }
}
