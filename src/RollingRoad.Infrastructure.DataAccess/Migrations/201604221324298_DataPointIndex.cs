namespace RollingRoad.Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataPointIndex : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataPoints", "Index", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataPoints", "Index");
        }
    }
}
