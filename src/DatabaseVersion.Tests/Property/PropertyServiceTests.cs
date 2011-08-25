using dbversion.Property;namespace dbversion.Tests.Property{
    using System.Collections.Generic;
    using System.Linq;
    
    using Xunit;

    //   using dbversion.Property;
    public class PropertyServiceTests    {
        [Fact]        public void ShouldBeAbleToGetPropertyFromIndexer()        {
            // Arrange
            PropertyService service = new PropertyService();
            service["myProperty"] = "myValue";

            // Act
            string propertyValue = service["myProperty"];

            // Assert
            Assert.Equal("myValue", propertyValue);
        }

        [Fact]        public void ShouldReturnNullIfPropertyIsNotSet()        {
            // Arrange
            PropertyService service = new PropertyService();

            // Act
            string propertyValue = service["myProperty"];

            // Assert
            Assert.Null(propertyValue);
        }

        [Fact]        public void ShouldBeAbleToGetAllPropertiesWithPrefix()        {
            // Arrange
            PropertyService service = new PropertyService();
            service["myProperty.value1"] = "value1";
            service["myProperty.value2"] = "value2";
            service["someOtherProperty.value1"] = "value3";

            // Act
            var properties = service.StartingWith("myProperty");

            // Assert
            Assert.Equal(2, properties.Count());
            Assert.Equal("value1", properties.Where(p => p.Key == "myProperty.value1").Single().Value);
            Assert.Equal("value2", properties.Where(p => p.Key == "myProperty.value2").Single().Value);
        }
    }
}

