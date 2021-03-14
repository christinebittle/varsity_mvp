namespace varsity_w_auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sports2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sports",
                c => new
                    {
                        SportID = c.Int(nullable: false, identity: true),
                        SportName = c.String(),
                    })
                .PrimaryKey(t => t.SportID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sports");
        }
    }
}
