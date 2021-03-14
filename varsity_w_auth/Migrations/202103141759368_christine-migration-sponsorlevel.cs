namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class christinemigrationsponsorlevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sponsors", "SponsorLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sponsors", "SponsorLevel");
        }
    }
}
