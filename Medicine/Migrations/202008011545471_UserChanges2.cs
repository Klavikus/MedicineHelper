namespace Medicine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChanges2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.Long(nullable: false),
                    NickName = c.String(),
                    LastCommandIndex = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserId, unique: true);

        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Users");
            AlterColumn("dbo.Users", "UserId", c => c.Long(nullable: false, identity: true));
            DropColumn("dbo.Users", "Id");
            AddPrimaryKey("dbo.Users", "UserId");
            CreateIndex("dbo.Users", "UserId", unique: true);
        }
    }
}
