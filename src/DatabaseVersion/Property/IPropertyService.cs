using System;

namespace DatabaseVersion.Property
{
    public interface IPropertyService
    {
        string this[string propertyName]
        {
            get;set;
        }
    }
}

