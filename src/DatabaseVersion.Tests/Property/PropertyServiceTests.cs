using Moq;namespace dbversion.Tests.Property{
    using System.Collections.Generic;
    using System.Linq;

    using dbversion.Property;
    
    using Xunit;
    public class PropertyServiceTests    {
        [Fact]        public void ShouldBeAbleToGetPropertyFromIndexer()        {
            // Arrange
            PropertyService service = new PropertyService();
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

        [Fact]        public void ShouldBeAbleToSetDefaultProperties()        {
            // Arrange
            var service = new PropertyService();

            var defaults1 = new List<Property>
            {
                new Property { Key = "property1", Value = "property1Value" },
                new Property { Key = "property2", Value = "property2Value" }
            };

            var defaults2 = new List<Property>
            {
                new Property { Key = "property3", Value = "property3Value" }
            };

            Mock<IHaveDefaultProperties > defaulter1 = new Mock<IHaveDefaultProperties>();
            defaulter1.Setup(d => d.DefaultProperties).Returns(defaults1);
            Mock<IHaveDefaultProperties > defaulter2 = new Mock<IHaveDefaultProperties>();
            defaulter2.Setup(d => d.DefaultProperties).Returns(defaults2);

            service.PropertyDefaulters = new[] { defaulter1.Object, defaulter2.Object };

            // Act
            service.SetDefaultProperties();

            // Assert
            Assert.Equal("property1Value", service["property1"].Value);
            Assert.Equal("property2Value", service["property2"].Value);
            Assert.Equal("property3Value", service["property3"].Value);
        }

        [Fact]        public void ShouldDoNothingIfNoDefaultPropertiesExist()        {
            // Arrange
            var service = new PropertyService();

            // Act
            service.SetDefaultProperties();

            // Assert
            Assert.Equal(0, service.Properties.Count());
        }
    }
}
