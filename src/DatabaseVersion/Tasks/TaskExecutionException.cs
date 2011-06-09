namespace DatabaseVersion.Tasks
{
    using System;

    /// <summary>
    /// Thrown if a task fails to execute correctly.
    /// </summary>
    public class TaskExecutionException : Exception
    {
        public TaskExecutionException(string message)
            : base(message)
        {
        }

        public TaskExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
