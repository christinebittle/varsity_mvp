namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldsformvp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PlayerFirstName", c => c.String());
            AddColumn("dbo.Players", "PlayerLastName", c => c.String());
            AddColumn("dbo.Sponsors", "SponsorURL", c => c.String());
            DropColumn("dbo.Players", "PlayerName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "PlayerName", c => c.String());
            DropColumn("dbo.Sponsors", "SponsorURL");
            DropColumn("dbo.Players", "PlayerLastName");
            DropColumn("dbo.Players", "PlayerFirstName");
        }
    }
}
