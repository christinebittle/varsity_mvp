namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class player_picextension : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "PicExtension");
        }
    }
}
