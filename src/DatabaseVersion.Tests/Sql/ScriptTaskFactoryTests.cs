using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Xml.Linq;
using DatabaseVersion.Tasks;
using DatabaseVersion.Tasks.Sql;

namespace DatabaseVersion.Tests.Sql
{
    public class ScriptTaskFactoryTests
    {
        private readonly ScriptTaskFactory factory = new ScriptTaskFactory();

        #region CanHandle
        [Fact]
        public void ShouldBeAbleToHandleScriptElement()
        {
            // Arrange
            XElement element = new XElement(XName.Get("script"));

            // Act
            bool canHandle = factory.CanCreate(element);

            // Assert
            Assert.True(canHandle);
        }

        [Fact]
        public void ShouldNotBeAbleToHandleInvalidElementName()
        {
            // Arrange
            XElement element = new XElement(XName.Get("include"));

            // Act
            bool canHandle = factory.CanCreate(element);

            // Assert
            Assert.False(canHandle);
        }

        [Fact]
        public void ShouldNotBeAbleToHandleNullElement()
        {
            // Arrange
            XElement element = null;

            // Act
            bool canHandle = factory.CanCreate(element);

            // Assert
            Assert.False(canHandle);
        }
        #endregion

        [Fact]
        public void ShouldCreateScriptTask()
        {
            // Arrange
            XElement element = new XElement(
                XName.Get("script"),
                new XAttribute(XName.Get("file"), "path/file.sql"));

            // Act
            IDatabaseTask task = this.factory.Create(element, 0, null);

            // Assert
            Assert.IsType<ScriptTask>(task);
        }

        [Fact]
        public void ShouldSetFileNameOfScript()
        {
            // Arrange
            XElement element = new XElement(
                XName.Get("script"),
                new XAttribute(XName.Get("file"), "path/file.sql"));

            // Act
            IDatabaseTask task = this.factory.Create(element, 0, null);

            // Assert
            Assert.Equal("path/file.sql", ((ScriptTask)task).FileName);
        }

        [Fact]
        public void ShouldSetExecutionOrderOfTask()
        {
            // Arrange
            XElement element = new XElement(
                XName.Get("script"),
                new XAttribute(XName.Get("file"), "path/file.sql"));

            // Act
            IDatabaseTask task = this.factory.Create(element, 25, null);

            // Assert
            Assert.Equal(25, task.ExecutionOrder);
        }
    }
}
