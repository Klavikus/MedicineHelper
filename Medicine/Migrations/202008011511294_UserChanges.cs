namespace Medicine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChanges : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Users");
            CreateTable(
                "dbo.Users",
                c => new
                {
                    UserId = c.Long(nullable: false),
                    NickName = c.String(),
                    LastCommandIndex = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.UserId, unique: true);
        }
        
        public override void Down()
        {
        }
    }
}
