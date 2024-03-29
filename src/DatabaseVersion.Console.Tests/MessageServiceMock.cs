namespace dbversion.Console.Tests
{
    using System.Text;

    public class MessageServiceMock : IMessageService
    {
        private readonly StringBuilder builder = new StringBuilder();

        public string Contents
        {
            get
            {
                return this.builder.ToString();
            }
        }

        public void WriteLine()
        {
            builder.AppendLine();
        }

        public void WriteLine(string input)
        {
            builder.AppendLine(input);
        }

        public void WriteDebugLine(string message)
        {
            builder.AppendLine(message);
        }

        public void WriteExceptionLine(string message, System.Exception exception)
        {
            builder.AppendLine(message);
        }
    }
}

