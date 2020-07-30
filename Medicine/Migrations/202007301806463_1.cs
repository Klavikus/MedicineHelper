namespace Medicine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Shedules");
            CreateTable(
                "dbo.Shedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CourseId = c.Guid(nullable: false),
                        MedId = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Dose = c.Double(nullable: false),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Shedules", new[] { "Id" });
            DropTable("dbo.Shedules");
        }
    }
}
