namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class playerhaspic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PlayerHasPic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "PlayerHasPic");
        }
    }
}
