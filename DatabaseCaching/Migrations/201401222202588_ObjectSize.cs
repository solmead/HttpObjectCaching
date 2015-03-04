namespace SqlCeDatabaseCaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ObjectSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CachedEntries", "Object", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CachedEntries", "Object", c => c.String(maxLength: 4000));
        }
    }
}
