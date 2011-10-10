using System;

namespace dbversion
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

        /// <summary>
        /// Writes the specified debug message.
        /// </summary>
        /// <param name="message">The message to write</param>
        void WriteDebugLine(string message);

        /// <summary>
        /// Writes the specified message and exception.
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="exception">The exception to write</param>
        void WriteExceptionLine(string message, Exception exception);
    }
}

