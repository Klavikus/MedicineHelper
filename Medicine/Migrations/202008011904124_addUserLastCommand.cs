namespace Medicine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserLastCommand : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastCommand", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LastCommand");
        }
    }
}
