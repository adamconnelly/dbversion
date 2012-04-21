namespace DatabaseVersion.Tests.Version.ClassicVersion
{
    using System;

    using dbversion.Version.ClassicVersion;
    using ClassicVersion = dbversion.Version.ClassicVersion.ClassicVersion;

    using Xunit;

    public class ClassicVersionProviderTests
    {
        [Fact]
        public void ShouldOrderVersionsBasedOnSystemVersion()
        {
            // Arrange
            ClassicVersionProvider provider = new ClassicVersionProvider();
            var comparer = provider.GetComparer();

            var version1 = new ClassicVersion("4.9");
            var version2 = new ClassicVersion("4.10");

            // Act
            var result = comparer.Compare(version1, version2);

            // Assert
            Assert.True(result < 0, "The versions were not ordered correctly");
        }
    }
}

