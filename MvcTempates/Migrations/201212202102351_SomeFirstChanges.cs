namespace MvcTempates.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeFirstChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employers", "Street", c => c.String());
            AddColumn("dbo.Employers", "ZipCode", c => c.String());
            AddColumn("dbo.Employers", "Place", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employers", "Place");
            DropColumn("dbo.Employers", "ZipCode");
            DropColumn("dbo.Employers", "Street");
        }
    }
}
