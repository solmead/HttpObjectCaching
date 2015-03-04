namespace SqlCeDatabaseCaching.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CachedEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Object = c.String(maxLength: 4000),
                        Created = c.DateTime(nullable: false),
                        Changed = c.DateTime(nullable: false),
                        TimeOut = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CachedEntries");
        }
    }
}
