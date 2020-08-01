namespace Medicine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        Message = c.String(),
                        SendTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MessageLogs");
        }
    }
}
