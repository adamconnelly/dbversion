using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using DatabaseVersion.Sql;
using Moq;
using System.Data;
using System.IO;

namespace DatabaseVersion.Tests.Sql
{
    public class ScriptTaskTests
    {
        [Fact]
        public void ShouldUseDatabaseArchiveToGetScript()
        {
            // Arrange
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(this.GetStream("A"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            connection.Setup(c => c.CreateCommand()).Returns(new Mock<IDbCommand>().Object);

            // Act
            task.Execute(connection.Object);

            // Assert
            version.Verify(v => v.Archive.GetFile("1\\scripts\\schema.sql"));
        }

        [Fact]
        public void ShouldUseConnectionToExecuteScript()
        {
            // Arrange
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile("1\\scripts\\schema.sql")).Returns(GetStream("ABCDE"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupProperty(c => c.CommandText);
            connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            // Act
            task.Execute(connection.Object);

            // Assert
            Assert.Equal("ABCDE", command.Object.CommandText);
            command.Verify(c => c.ExecuteNonQuery());
        }

        [Fact]
        public void ShouldSplitScriptIntoCommandsBySeparator()
        {
            // Arrange
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream("ABCDE" + Environment.NewLine + "GO" + Environment.NewLine + "FGHIJ"));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupProperty(c => c.CommandText);
            connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            // Act
            task.Execute(connection.Object);

            // Assert
            command.VerifySet(c => c.CommandText = "ABCDE");
            command.VerifySet(c => c.CommandText = "FGHIJ");
            command.Verify(c => c.ExecuteNonQuery(), Times.Exactly(2));
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
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithDifferentCasedSeparators));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupProperty(c => c.CommandText);
            connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            // Act
            task.Execute(connection.Object);

            // Assert
            command.VerifySet(c => c.CommandText = "ABCDE");
            command.VerifySet(c => c.CommandText = "FGHIJ");
            command.VerifySet(c => c.CommandText = "DEFJAB");
            command.VerifySet(c => c.CommandText = "LDKSIE");
            command.VerifySet(c => c.CommandText = "dkjsaks");
            command.Verify(c => c.ExecuteNonQuery(), Times.Exactly(5));
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
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorsWithinLines));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupProperty(c => c.CommandText);
            connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            // Act
            task.Execute(connection.Object);

            // Assert
            command.VerifySet(c => c.CommandText = "insert into books (name) values ('Great Book');");
            command.VerifySet(c => c.CommandText = "update books set name = 'Good to go' where name = 'Great Book';");
            command.VerifySet(c => c.CommandText = "delete from books;");
            command.Verify(c => c.ExecuteNonQuery(), Times.Exactly(3));
        }

        private static readonly string ScriptWithSeparatorAtStartOfScript =
            "GO" + Environment.NewLine +
            "update books set name = 'Good to go' where name = 'Great Book';";

        [Fact]
        public void ShouldAllowSeparatorAtStartOfScriptWhenExecuting()
        {
            // Arrange
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorAtStartOfScript));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupProperty(c => c.CommandText);
            connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            // Act
            task.Execute(connection.Object);

            // Assert
            command.VerifySet(c => c.CommandText = "update books set name = 'Good to go' where name = 'Great Book';");
            command.Verify(c => c.ExecuteNonQuery(), Times.Exactly(1));
        }

        private static readonly string ScriptWithSeparatorAtEndOfScript =
            "update books set name = 'Good to go' where name = 'Great Book';" + Environment.NewLine +
            "go";

        [Fact]
        public void ShouldAllowSeparatorAtEndOfScriptWhenExecuting()
        {
            // Arrange
            Mock<IDatabaseVersion> version = new Mock<IDatabaseVersion> { DefaultValue = DefaultValue.Mock };
            version.Setup(v => v.ManifestPath).Returns("1\\database.xml");
            version.Setup(v => v.Archive.GetFile(It.IsAny<string>())).Returns(
                GetStream(ScriptWithSeparatorAtEndOfScript));
            ScriptTask task = new ScriptTask("scripts\\schema.sql", 0, version.Object);
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            Mock<IDbCommand> command = new Mock<IDbCommand>();
            command.SetupProperty(c => c.CommandText);
            connection.Setup(c => c.CreateCommand()).Returns(command.Object);

            // Act
            task.Execute(connection.Object);

            // Assert
            command.VerifySet(c => c.CommandText = "update books set name = 'Good to go' where name = 'Great Book';");
            command.Verify(c => c.ExecuteNonQuery(), Times.Exactly(1));
        }

        private Stream GetStream(string contents)
        {
            return new MemoryStream(Encoding.Default.GetBytes(contents));
        }
    }
}
