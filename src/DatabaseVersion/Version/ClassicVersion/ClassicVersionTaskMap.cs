using FluentNHibernate.Mapping;
using DatabaseVersion.Connections;
using System.ComponentModel.Composition;

namespace DatabaseVersion.Version.ClassicVersion
{
    [Export(typeof(IHibernateMapping))]
    public class ClassicVersionTaskMap : ClassMap<ClassicVersionTask>, IHibernateMapping
    {
        public ClassicVersionTaskMap()
        {
            this.Table("VersionTask");
            this.Id(t => t.Id).Column("id_versiontask").GeneratedBy.GuidComb();
            this.Map(t => t.UpdatedOn).Column("updated_date");
            this.Map(t => t.Name).Column("script_name");
            this.Map(t => t.ExecutionOrder).Column("script_order");
            this.References(t => t.Version).Column("id_version").Cascade.All();
        }
    }
}
