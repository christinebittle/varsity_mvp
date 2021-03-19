namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class support_messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Supports",
                c => new
                    {
                        SupportID = c.Int(nullable: false, identity: true),
                        SupportMessage = c.String(),
                        SupportDate = c.DateTime(nullable: false),
                        TeamID = c.Int(nullable: false),
                        Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SupportID)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .ForeignKey("dbo.Teams", t => t.TeamID, cascadeDelete: true)
                .Index(t => t.TeamID)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Supports", "TeamID", "dbo.Teams");
            DropForeignKey("dbo.Supports", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.Supports", new[] { "Id" });
            DropIndex("dbo.Supports", new[] { "TeamID" });
            DropTable("dbo.Supports");
        }
    }
}
