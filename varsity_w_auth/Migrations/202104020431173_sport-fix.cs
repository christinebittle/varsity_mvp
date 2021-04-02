namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sportfix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Teams", "SportID", "dbo.Sports");
            DropIndex("dbo.Teams", new[] { "SportID" });
            AlterColumn("dbo.Teams", "SportID", c => c.Int(nullable: false));
            CreateIndex("dbo.Teams", "SportID");
            AddForeignKey("dbo.Teams", "SportID", "dbo.Sports", "SportID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teams", "SportID", "dbo.Sports");
            DropIndex("dbo.Teams", new[] { "SportID" });
            AlterColumn("dbo.Teams", "SportID", c => c.Int());
            CreateIndex("dbo.Teams", "SportID");
            AddForeignKey("dbo.Teams", "SportID", "dbo.Sports", "SportID");
        }
    }
}
