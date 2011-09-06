namespace dbversion.Console.Tests.Command.Version
{
    using System;
    using System.Diagnostics;

    using dbversion.Console;
    using dbversion.Console.Command.Version;

    using Moq;

    using Xunit;

    public class DisplayVersionCommandTests
    {
        [Fact]
        public void ShouldDisplayProgramNameAndVersion()
        {
            // Arrange
            var command = new DisplayVersionCommand();
            var messageService = new Mock<IMessageService>();
            command.MessageService = messageService.Object;
            var name = typeof(dbversion.Console.Program).Assembly.GetName().Name;
            var version = typeof(dbversion.Console.Program).Assembly.GetName().Version;
            string expectedTitle = string.Format("{0} {1}", name, version);

            // Act
            command.Execute(new[] { "version" });

            // Assert
            messageService.Verify(m => m.WriteLine(expectedTitle));
        }

        [Fact]
        public void ShouldWriteAssemblyCopyrightStatement()
        {
            // Arrange
            var command = new DisplayVersionCommand();
            var messageService = new Mock<IMessageService>();
            command.MessageService = messageService.Object;
            var assembly = typeof(dbversion.Console.Program).Assembly;
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            // Act
            command.Execute(new[] { "version" });

            // Assert
            messageService.Verify(m => m.WriteLine(versionInfo.LegalCopyright));
        }

        [Fact]
        public void ShouldWriteLicenseInformation()
        {
            // Arrange
            var command = new DisplayVersionCommand();
            var messageService = new Mock<IMessageService>();
            command.MessageService = messageService.Object;

            // Act
            command.Execute(new[] { "version" });

            // Assert
            messageService.Verify(m => m.WriteLine("License MIT: The MIT License"));
            messageService.Verify(m => m.WriteLine("This is free software: you are free to change and redistribute it."));
            messageService.Verify(m => m.WriteLine("There is NO WARRANTY, to the extent permitted by law."));
        }
    }
}

