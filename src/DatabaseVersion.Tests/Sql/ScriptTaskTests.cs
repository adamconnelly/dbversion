namespace dbversion.Tests.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;
    using dbversion.Tasks.Sql;
    using Moq;
    using System.Data;
    using System.IO;
    using dbversion.Property;
    using dbversion.Tasks;
    using dbversion.Version;
    using NHibernate;

    public class ScriptTaskTests
    {
        private readonly Mock<ISession> session = new Mock<ISession>() { DefaultValue = DefaultValue.Mock };
        private readonly Mock<IDbCommand> command = new Mock<IDbCommand>();
        private readonly Mock<IMessageService> messageService = new Mock<IMessageService> { DefaultValue = DefaultValue.Mock };
        private readonly Mock<IPropertyService> propertyService = new Mock<IPropertyService>();

        public ScriptTaskTests()
        {
            this.session.Setup(s => s.Connection.CreateCommand()).Returns(command.Object);
            this.command.SetupProperty(c => c.CommandTimeout);
        }

        [Fact]
        public void ShouldUseDatabaseArchiveToGetScript()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetScriptPath("1\\database.xml", "scripts\\schema.sql")).Returns("1\\scripts\\schema.sql");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(this.GetStream("A"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(session.Object, 1, 1);

            // Assert
            version.Verify(v => v.Archive.GetFile("1\\scripts\\schema.sql"));
        }

        [Fact]
        public void ShouldUseSessionToExecuteScript()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetScriptPath("1\\database.xml", "scripts\\schema.sql")).Returns("1\\scripts\\schema.sql");
            version.Setup(v => v.Archive.GetFile("1\\scripts\\schema.sql")).Returns(GetStream("ABCDE"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(session.Object, 1, 1);

            // Assert
            command.VerifySet(c => c.CommandText = "ABCDE");
            command.Verify(c => c.ExecuteNonQuery());
        }

        [Fact]
        public void ShouldSplitScriptIntoCommandsBySeparator()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream("ABCDE" + Environment.NewLine + "GO" + Environment.NewLine + "FGHIJ"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(session.Object, 1, 1);

            // Assert
            command.VerifySet(c => c.CommandText = "ABCDE");
            command.VerifySet(c => c.CommandText = "FGHIJ");
        }

        private static readonly string ScriptWithDifferentCasedSeparators =
            "ABCDE" + Environment.NewLine +
            "GO" + Environment.NewLine +
            "FGHIJ" + Environment.NewLine +
            "go" + Environment.NewLine +
            "DEFJAB" + Environment.NewLine +
            "Go" + Environment.NewLine +
            "LDKSIE" + Environment.NewLine +
            "gO" + Environment.NewLine +
            "dkjsaks";

        [Fact]
        public void ShouldIgnoreSeparatorCaseWhenExecuting()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithDifferentCasedSeparators));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(session.Object, 1, 1);

            // Assert
            command.VerifySet(c => c.CommandText = "ABCDE");
            command.VerifySet(c => c.CommandText = "FGHIJ");
            command.VerifySet(c => c.CommandText = "DEFJAB");
            command.VerifySet(c => c.CommandText = "LDKSIE");
            command.VerifySet(c => c.CommandText = "dkjsaks");
        }

        private static readonly string ScriptWithSeparatorsWithinLines =
            "insert into books (name) values ('Great Book');" + Environment.NewLine +
            "GO" + Environment.NewLine +
            "update books set name = 'Good to go' where name = 'Great Book';" + Environment.NewLine +
            "go" + Environment.NewLine +
            "delete from books;";

        [Fact]
        public void ShouldOnlySplitSeparatorsOnNewLinesWhenExecuting()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorsWithinLines));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(session.Object, 1, 1);

            // Assert
            command.VerifySet(c => c.CommandText = "insert into books (name) values ('Great Book');");
            command.VerifySet(c => c.CommandText = "update books set name = 'Good to go' where name = 'Great Book';");
            command.VerifySet(c => c.CommandText = "delete from books;");
        }

        private static readonly string ScriptWithSeparatorAtStartOfScript =
            "GO" + Environment.NewLine +
            "update books set name = 'Good to go' where name = 'Great Book';";

        [Fact]
        public void ShouldAllowSeparatorAtStartOfScriptWhenExecuting()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorAtStartOfScript));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(session.Object, 1, 1);

            // Assert
            command.VerifySet(c => c.CommandText = "update books set name = 'Good to go' where name = 'Great Book';");
        }

        private static readonly string ScriptWithSeparatorAtEndOfScript =
            "update books set name = 'Good to go' where name = 'Great Book';" + Environment.NewLine +
            "go";

        [Fact]
        public void ShouldAllowSeparatorAtEndOfScriptWhenExecuting()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorAtEndOfScript));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(session.Object, 1, 1);

            // Assert
            command.VerifySet(c => c.CommandText = "update books set name = 'Good to go' where name = 'Great Book';");
        }

        [Fact]
        public void ShouldThrowTaskExecutionExceptionIfExecutingScriptThrowsException()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetScriptPath("1\\database.xml", "scripts\\schema.sql")).Returns("1\\scripts\\schema.sql");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorAtEndOfScript));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);
            Exception exception = new Exception();
            command.Setup(c=>c.ExecuteNonQuery()).Throws(exception);

            // Act
            Exception thrownException = Record.Exception(() => task.Execute(session.Object, 1, 1));

            // Assert
            Assert.IsType<TaskExecutionException>(thrownException);
            Assert.Equal("Failed to execute Batch 1 of script \"1\\scripts\\schema.sql\". " + exception.Message, thrownException.Message);
            Assert.Same(exception, thrownException.InnerException);
        }

        [Fact]
        public void ShouldThrowTaskExecutionExceptionIfScriptPathDoesNotExist()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetScriptPath("1\\database.xml", "scripts\\schema.sql")).Returns("1\\scripts\\schema.sql");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns((Stream)null);
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object, messageService.Object, this.propertyService.Object);

            // Act
            Exception thrownException = Record.Exception(() => task.Execute(new Mock<ISession>().Object, 1, 1));

            // Assert
            Assert.IsType<TaskExecutionException>(thrownException);
            Assert.Equal("The script file \"1\\scripts\\schema.sql\" does not exist in the archive.", thrownException.Message);
        }

        [Fact]
        public void ShouldSetCommandTimeoutIfSpecified()
        {
            // Arrange
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion>{ DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetScriptPath("1\\database.xml", "scripts\\schema.sql")).Returns("1\\scripts\\schema.sql");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorAtEndOfScript));
            var task = new ScriptTask("scripts\\test.sql", 0, version.Object, this.messageService.Object, this.propertyService.Object);

            propertyService.Setup(p => p["dbversion.sql.command_timeout"]).Returns(new Property { Value = "100" });

            // Act
            task.Execute(this.session.Object, 1, 1);

            // Assert
            Assert.Equal(100, task.TaskTimeout);
        }

        [Fact]
        public void ShouldNotSetCommandTimeoutIfNotSpecified()
        {
            // Arrange
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetScriptPath("1\\database.xml", "scripts\\schema.sql")).Returns("1\\scripts\\schema.sql");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorAtEndOfScript));
            var task = new ScriptTask("scripts\\test.sql", 0, version.Object, this.messageService.Object, this.propertyService.Object);

            // Act
            task.Execute(this.session.Object, 1, 1);

            // Assert
            Assert.Null(task.TaskTimeout);
        }

        private Stream GetStream(string contents)
        {
            return new MemoryStream(Encoding.Default.GetBytes(contents));
        }
    }
}
