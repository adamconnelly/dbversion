using FluentNHibernate.Mapping;
namespace dbversion.Version.NumericVersion
{
    public class NumericVersionMap : ClassMap<NumericVersion>
    {
        public NumericVersionMap()
        {
            this.Table("Version");
            this.Id(v => v.Id).Column("id_version").GeneratedBy.GuidComb();
            this.Map(v => v.CreatedOn).Column("created_date");
            this.Map(v => v.UpdatedOn).Column("updated_date");
            this.Map(v => v.Version).Column("version");
            this.HasMany<NumericVersionTask>(v => v.Tasks).Table("VersionTask").KeyColumn("id_version").Not.LazyLoad().Cascade.All();
        }
    }
}
