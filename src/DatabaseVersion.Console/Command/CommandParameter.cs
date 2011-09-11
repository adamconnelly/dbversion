namespace dbversion.Console.Command
{
    public class CommandParameter
    {
        public CommandParameter(string shortOption, string longOption, string description)
        {
            this.ShortOption = shortOption;
            this.LongOption = longOption;
            this.Description = description;
        }

        public string ShortOption
        {
            get;
            private set;
        }

        public string LongOption
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }
    }
}
