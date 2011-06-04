using FluentNHibernate.Mapping;
namespace DatabaseVersion.Version.NumericVersion
{
    public class NumericVersionMap : ClassMap<NumericVersion>
    {
        public NumericVersionMap()
        {
            this.Table("Version");
            this.Id(v => v.Version);
            this.Map(v => v.UpdatedOn);
        }
    }
}
