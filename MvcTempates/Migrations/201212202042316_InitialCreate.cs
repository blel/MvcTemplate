namespace MvcTempates.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Sent = c.DateTime(),
                        FK_EmployerID = c.Int(nullable: false),
                        SentTo_ID = c.Int(),
                        Application_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Employers", t => t.SentTo_ID)
                .ForeignKey("dbo.Applications", t => t.Application_ID)
                .Index(t => t.SentTo_ID)
                .Index(t => t.Application_ID);
            
            CreateTable(
                "dbo.Employers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        Telephone = c.String(),
                        Email = c.String(),
                        FK_EmployerID = c.Int(),
                        Contact_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Employers", t => t.Contact_ID)
                .Index(t => t.Contact_ID);
            
            CreateTable(
                "dbo.Remarks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        DateOfRemark = c.DateTime(),
                        Content = c.String(),
                        FK_ApplicationID = c.Int(nullable: false),
                        BelongsToApplication_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Applications", t => t.BelongsToApplication_ID)
                .Index(t => t.BelongsToApplication_ID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Remarks", new[] { "BelongsToApplication_ID" });
            DropIndex("dbo.People", new[] { "Contact_ID" });
            DropIndex("dbo.Applications", new[] { "Application_ID" });
            DropIndex("dbo.Applications", new[] { "SentTo_ID" });
            DropForeignKey("dbo.Remarks", "BelongsToApplication_ID", "dbo.Applications");
            DropForeignKey("dbo.People", "Contact_ID", "dbo.Employers");
            DropForeignKey("dbo.Applications", "Application_ID", "dbo.Applications");
            DropForeignKey("dbo.Applications", "SentTo_ID", "dbo.Employers");
            DropTable("dbo.Remarks");
            DropTable("dbo.People");
            DropTable("dbo.Employers");
            DropTable("dbo.Applications");
        }
    }
}
