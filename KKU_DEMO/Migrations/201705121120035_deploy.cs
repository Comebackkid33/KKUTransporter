namespace KKU_DEMO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deploy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Causes", "Type", c => c.String());
            AddColumn("dbo.Sensors", "NoLoadCount", c => c.Int(nullable: false));
            AddColumn("dbo.Shifts", "State", c => c.String());
            AddColumn("dbo.Incidents", "SensorId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "ChatId", c => c.Int(nullable: false));
            CreateIndex("dbo.Incidents", "SensorId");
            AddForeignKey("dbo.Incidents", "SensorId", "dbo.Sensors", "Id");
            DropColumn("dbo.Shifts", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shifts", "Status", c => c.String());
            DropForeignKey("dbo.Incidents", "SensorId", "dbo.Sensors");
            DropIndex("dbo.Incidents", new[] { "SensorId" });
            DropColumn("dbo.AspNetUsers", "ChatId");
            DropColumn("dbo.Incidents", "SensorId");
            DropColumn("dbo.Shifts", "State");
            DropColumn("dbo.Sensors", "NoLoadCount");
            DropColumn("dbo.Causes", "Type");
        }
    }
}
