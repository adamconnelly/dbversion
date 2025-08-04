dbversion
=========

A tool for creating and upgrading databases using SQL scripts. dbversion keeps track of what scripts have already been applied to your database so that it can figure out what scripts it needs to run to upgrade your database to the current version.

## Project Status

This project is archived and no-longer actively maintained. Since dbversion was released there are now many other tools that are more widely supported so there is not a good reason to use it anymore.

## Quick Start

### Installation
  1. Download the latest version from [here](https://github.com/adamconnelly/dbversion/releases/latest).
  2. Extract dbversion.zip somewhere and add it to your path. For example, I normally put it in C:\Apps\dbversion.

If you now open a command line, and type ```dbversion version``` you should see something like the following output:

```
C:\git\dbversion [master +0 ~1 -0]> dbversion version
dbversion 0.2.0.0
Copyright © Adam Connelly 2013
License MIT: The MIT License
This is free software: you are free to change and redistribute it.
There is NO WARRANTY, to the extent permitted by law.
```

### Add a Saved Connection
Saved connections mean you don't have to enter your database connection details every time you run dbversion. To add an initial saved connection run the following command:

```
dbversion saved-connection -ndatabase1 -c"data source=(local);initial catalog=database1;integrated security=true"
```

This will create a saved connection named database1, and make it your default connection. Now that you have a default connection, you don't need to specify database connection details when running other commands. dbversion will assume you want to use your default connection unless you specify another connection string.

### Create / Update a database
To create a new database or update an existing database, you run `dbversion create`. This will compare the available versions in your database archive to what has already been applied to your database, and will automatically run whatever scripts are required to bring your database up to date.

The following command will update your default database to the current version stored in the `sql` subdirectory:

```
dbversion create -a.\sql
```

If you wanted to update a different saved connection, you would run:

```
dbversion create -a.\sql -sdatabase2
```

And if you wanted to just specify a different connection string, you could run:

```
dbversion create -a.\sql -c"data source=someserver;initial catalog=..."
```

### Database Archive Structure
The version information is stored in a database archive. This can either be a folder on disk, or a zip file. Each database archive has the following structure:

```
Root
|   properties.xml
|
+---1.0
|   |   001 CREATE TABLE person.sql
|   |   002 ALTER TABLE person Add name.sql
|   |   ...
|   |   database.xml
|
+---1.1
|   |   001 CREATE TABLE address.sql
|   |   ...
|   |   database.xml
```

The root of the archive contains one or more version folders, and an optional `properties.xml` file. The properties.xml file allows you to override application settings on a per-archive basis, so that if you need to share any settings with other team members you can.

Each version folder contains one or more script files, along with a `database.xml` file.
### database.xml file
The database.xml file specifies the name of a version, along with the scripts that should be applied to get you to the version. Here's an example file:

```
<database version="1.0">
  <script file="001 CREATE TABLE person.sql"/>
  <script file="002 ALTER TABLE person Add name.sql"/>
</database>
```

The scripts will be applied in the order they're specified in the `database.xml` file.

### Getting Help
All of the available dbversion commands can be listed by using `dbversion help`:

```
C:\git\dbversion [master +0 ~1 -0]> dbversion help
  check             Checks whether all of the versions and tasks have been installed in the database.
  create            Creates or upgrades a database using the specified archive.
  help              Displays information about using the application and then exits.
  history           Prints out a history of the installed versions.
  saved-connection  Allows connection details to be saved and managed.
  version           Displays version information and then exits.

  Use dbversion help [command] for more help on a command.
```

You can get help for a specific command by using `dbversion help [command]`:

```
C:\git\dbversion [master +0 ~1 -0]> dbversion help create
Usage: dbversion create [options]

Options:
  -a, --archive             The archive to create the database from.
  -c, --connectionString    The database connection string.
  -v, --version             The version to create or upgrade to.
  -p, --connectionProvider  The hibernate connection provider.
  -d, --driverClass         The hibernate driver class.
  -l, --dialect             The hibernate dialect.
  -s, --saved-connection    The name of the saved connection to use.
  --simulate                Indicates that the update should be simulated and no actual changes should be made.
  -m, --missing             Indicates that any missing tasks should be executed. If the -v flag is used, only that version will be checked.
  -r, --rollback            Indicates that any changes made by the command should be rolled back.
```

### Database Connections
By default dbversion assumed you're using Microsoft SQL Server, so if that's the case you can just use standard SQL Server connection strings as normal. dbversion uses NHibernate under the hood to connect to the database, so you should be able to use any database that NHibernate supports.

To use some other type of database, you need to specify the connection provider, dialect, and driver class you want to use. So you might use something like the following to connect to a MySQL database:

```
dbversion create -a.\sql -p"NHibernate.Connection.DriverConnectionProvider" -d"NHibernate.Driver.MySqlDataDriver" -l"NHibernate.Dialect.MySQLDialect" -c"Data Source=1.2.3.4;Database=test;User Id=user;Password=password123
```
