using System.Data.Entity.Migrations;

namespace RollingRoad.Infrastructure.DataAccess.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        DataSet_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataSet", t => t.DataSet_Id, cascadeDelete: true)
                .Index(t => t.DataSet_Id);
            
            CreateTable(
                "dbo.DataPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Double(nullable: false),
                        DataList_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataLists", t => t.DataList_Id, cascadeDelete: true)
                .Index(t => t.DataList_Id);
            
            CreateTable(
                "dbo.DataSet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DataLists", "DataSet_Id", "dbo.DataSet");
            DropForeignKey("dbo.DataPoints", "DataList_Id", "dbo.DataLists");
            DropIndex("dbo.DataPoints", new[] { "DataList_Id" });
            DropIndex("dbo.DataLists", new[] { "DataSet_Id" });
            DropTable("dbo.DataSet");
            DropTable("dbo.DataPoints");
            DropTable("dbo.DataLists");
        }
    }
}
