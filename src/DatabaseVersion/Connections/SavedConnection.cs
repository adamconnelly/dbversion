namespace dbversion.Connections
{
    public class SavedConnection
    {
        public SavedConnection()
        {
        }
        
        public SavedConnection(
            string name, string connectionString, string connectionProvider, string driverClass, string dialect, bool isDefault)
        {
            this.Name = name;
            this.ConnectionString = connectionString;
            this.ConnectionProvider = connectionProvider;
            this.DriverClass = driverClass;
            this.Dialect = dialect;
            this.IsDefault = isDefault;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is the default connection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the connection.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connection provider.
        /// </summary>
        /// <value>
        /// The connection provider.
        /// </value>
        public string ConnectionProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the driver class.
        /// </summary>
        /// <value>
        /// The driver class.
        /// </value>
        public string DriverClass
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dialect.
        /// </summary>
        /// <value>
        /// The dialect.
        /// </value>
        public string Dialect
        {
            get;
            set;
        }

        /// <summary>
        /// Makes a copy of the connection.
        /// </summary>
        public SavedConnection Copy()
        {
            return new SavedConnection(
                this.Name, this.ConnectionString, this.ConnectionProvider, this.DriverClass, this.Dialect, this.IsDefault);
        }
    }
}

