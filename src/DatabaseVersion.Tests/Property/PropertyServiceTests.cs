namespace dbversion.Tests.Property{
    using System.Collections.Generic;
    using System.Linq;

    using dbversion.Property;
    
    using Xunit;

    //   using dbversion.Property;
    public class PropertyServiceTests    {
        [Fact]        public void ShouldBeAbleToGetPropertyFromIndexer()        {
            // Arrange
            PropertyService service = new PropertyService();
            //service["myProperty"] = new Property { Key = "myProperty", Value = "mValue" };
            service.Add(new Property { Key = "myProperty", Value = "myValue" });

            // Act
            var propertyValue = service["myProperty"];

            // Assert
            Assert.Equal("myValue", propertyValue.Value);
        }

        [Fact]        public void ShouldReturnNullIfPropertyIsNotSet()        {
            // Arrange
            PropertyService service = new PropertyService();

            // Act
            var propertyValue = service["myProperty"];

            // Assert
            Assert.Null(propertyValue);
        }
        
        [Fact]        public void ShouldBeAbleToGetAllPropertiesWithPrefix()        {
            // Arrange
            PropertyService service = new PropertyService();
            service.Add(new Property { Key = "myProperty.value1", Value = "value1" });
            service.Add(new Property { Key = "myProperty.value2", Value = "value2" });
            service.Add(new Property { Key = "someOtherProperty.value1", Value = "value3" });

            // Act
            var properties = service.StartingWith("myProperty");

            // Assert
            Assert.Equal(2, properties.Count());
            Assert.Equal("value1", properties.Where(p => p.Key == "myProperty.value1").Single().Value);
            Assert.Equal("value2", properties.Where(p => p.Key == "myProperty.value2").Single().Value);
        }
    }
}

