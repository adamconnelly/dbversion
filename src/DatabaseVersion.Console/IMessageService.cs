namespace dbversion.Console
{
    /// <summary>
    /// Used to output messages to the user.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Writes an empty line.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name='message'>
        /// The message to write.
        /// </param>
        void WriteLine(string message);
    }
}

