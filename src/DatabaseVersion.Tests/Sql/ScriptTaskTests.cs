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
    using dbversion.Tasks;
    using dbversion.Version;
    using NHibernate;

    public class ScriptTaskTests
    {
        private readonly Mock<ISession> session = new Mock<ISession>() { DefaultValue = DefaultValue.Mock };
        private readonly Mock<IMessageService> messageService = new Mock<IMessageService> { DefaultValue = DefaultValue.Mock };

        [Fact]
        public void ShouldUseDatabaseArchiveToGetScript()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetScriptPath("1\\database.xml", "scripts\\schema.sql")).Returns("1\\scripts\\schema.sql");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(this.GetStream("A"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            task.Execute(session.Object, messageService.Object);

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
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            task.Execute(session.Object, messageService.Object);

            // Assert
            session.Verify(s => s.CreateSQLQuery("ABCDE").ExecuteUpdate());
        }

        [Fact]
        public void ShouldSplitScriptIntoCommandsBySeparator()
        {
            // Arrange
            Mock<IDatabaseVersion > version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream("ABCDE" + Environment.NewLine + "GO" + Environment.NewLine + "FGHIJ"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            task.Execute(session.Object, messageService.Object);

            // Assert
            session.Verify(s => s.CreateSQLQuery("ABCDE").ExecuteUpdate());
            session.Verify(s => s.CreateSQLQuery("FGHIJ").ExecuteUpdate());
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
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            task.Execute(session.Object, messageService.Object);

            // Assert
            session.Verify(s => s.CreateSQLQuery("ABCDE").ExecuteUpdate());
            session.Verify(s => s.CreateSQLQuery("FGHIJ").ExecuteUpdate());
            session.Verify(s => s.CreateSQLQuery("DEFJAB").ExecuteUpdate());
            session.Verify(s => s.CreateSQLQuery("LDKSIE").ExecuteUpdate());
            session.Verify(s => s.CreateSQLQuery("dkjsaks").ExecuteUpdate());
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
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            task.Execute(session.Object, messageService.Object);

            // Assert
            session.Verify(s => s.CreateSQLQuery("insert into books (name) values ('Great Book');").ExecuteUpdate());
            session.Verify(s => s.CreateSQLQuery("update books set name = 'Good to go' where name = 'Great Book';").ExecuteUpdate());
            session.Verify(s => s.CreateSQLQuery("delete from books;").ExecuteUpdate());
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
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            task.Execute(session.Object, messageService.Object);

            // Assert
            session.Verify(s => s.CreateSQLQuery("update books set name = 'Good to go' where name = 'Great Book';").ExecuteUpdate());
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
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            task.Execute(session.Object, messageService.Object);

            // Assert
            session.Verify(s => s.CreateSQLQuery("update books set name = 'Good to go' where name = 'Great Book';").ExecuteUpdate());
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
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Exception exception = new Exception();
            session.Setup(s => s.CreateSQLQuery(It.IsAny<string>()).ExecuteUpdate()).Throws(exception);

            // Act
            Exception thrownException = Record.Exception(() => task.Execute(session.Object, messageService.Object));

            // Assert
            Assert.IsType<TaskExecutionException>(thrownException);
            Assert.Equal("Failed to execute script \"1\\scripts\\schema.sql\". " + exception.Message, thrownException.Message);
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
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);

            // Act
            Exception thrownException = Record.Exception(() => task.Execute(new Mock<ISession>().Object, messageService.Object));

            // Assert
            Assert.IsType<TaskExecutionException>(thrownException);
            Assert.Equal("The script file \"1\\scripts\\schema.sql\" does not exist in the archive.", thrownException.Message);
        }

        private Stream GetStream(string contents)
        {
            return new MemoryStream(Encoding.Default.GetBytes(contents));
        }
    }
}
