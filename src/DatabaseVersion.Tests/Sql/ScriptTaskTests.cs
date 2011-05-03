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

        private Stream GetStream(string contents)
        {
            return new MemoryStream(Encoding.Default.GetBytes(contents));
        }
    }
}
