namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sport_team : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "SportID", c => c.Int());
            CreateIndex("dbo.Teams", "SportID");
            AddForeignKey("dbo.Teams", "SportID", "dbo.Sports", "SportID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teams", "SportID", "dbo.Sports");
            DropIndex("dbo.Teams", new[] { "SportID" });
            DropColumn("dbo.Teams", "SportID");
        }
    }
}
