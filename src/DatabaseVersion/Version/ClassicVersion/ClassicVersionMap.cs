using FluentNHibernate.Mapping;
using DatabaseVersion.Connections;
using System.ComponentModel.Composition;
namespace DatabaseVersion.Version.ClassicVersion
{
    [Export(typeof(IHibernateMapping))]
    public class ClassicVersionMap : ClassMap<ClassicVersion>, IHibernateMapping
    {
        public ClassicVersionMap()
        {
            this.Table("Version");
            this.Id(v => v.Id).Column("id_version").GeneratedBy.GuidComb();
            this.Map(v => v.CreatedOn).Column("created_date");
            this.Map(v => v.UpdatedOn).Column("updated_date");
            this.Map(v => v.Version).Column("version");
            this.HasMany(v => v.Tasks).Table("VersionTask").KeyColumn("id_version").Not.LazyLoad().Cascade.All();
        }
    }
}
