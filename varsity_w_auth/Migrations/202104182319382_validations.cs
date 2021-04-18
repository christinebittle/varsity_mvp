namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validations : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Players", "PlayerFirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Players", "PlayerLastName", c => c.String(nullable: false));
            AlterColumn("dbo.Sponsors", "SponsorName", c => c.String(nullable: false));
            AlterColumn("dbo.Sponsors", "SponsorURL", c => c.String(nullable: false));
            AlterColumn("dbo.Sports", "SportName", c => c.String(nullable: false));
            AlterColumn("dbo.Supports", "SupportMessage", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Supports", "SupportMessage", c => c.String());
            AlterColumn("dbo.Sports", "SportName", c => c.String());
            AlterColumn("dbo.Sponsors", "SponsorURL", c => c.String());
            AlterColumn("dbo.Sponsors", "SponsorName", c => c.String());
            AlterColumn("dbo.Players", "PlayerLastName", c => c.String());
            AlterColumn("dbo.Players", "PlayerFirstName", c => c.String());
        }
    }
}
