using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Xml.Linq;
using DatabaseVersion.ScriptTask;
using st = DatabaseVersion.ScriptTask.ScriptTask;

namespace DatabaseVersion.Tests.ScriptTask
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
            bool canHandle = factory.CanHandle(element);

            // Assert
            Assert.True(canHandle);
        }

        [Fact]
        public void ShouldNotBeAbleToHandleInvalidElementName()
        {
            // Arrange
            XElement element = new XElement(XName.Get("include"));

            // Act
            bool canHandle = factory.CanHandle(element);

            // Assert
            Assert.False(canHandle);
        }

        [Fact]
        public void ShouldNotBeAbleToHandleNullElement()
        {
            // Arrange
            XElement element = null;

            // Act
            bool canHandle = factory.CanHandle(element);

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
            IDatabaseTask task = this.factory.Create(element);

            // Assert
            Assert.IsType<st>(task);
        }
    }
}
