namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spicescale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "SpiceLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "SpiceLevel");
        }
    }
}
