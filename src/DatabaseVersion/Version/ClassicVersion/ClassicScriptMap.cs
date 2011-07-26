using FluentNHibernate.Mapping;

namespace DatabaseVersion.Version.ClassicVersion
{
    public class ClassicScriptMap : ClassMap<ClassicVersionScript>
    {
        public ClassicScriptMap()
        {
            this.Table("VersionTask");
            this.Id(t => t.Id).Column("id_versiontask").GeneratedBy.GuidComb();
            this.Map(t => t.UpdatedOn).Column("updated_date");
            this.Map(t => t.Name).Column("script_name");
            this.Map(t => t.ScriptOrder).Column("script_order");
            this.References(t => t.Version).Column("id_version").Cascade.All();
        }
    }
}
