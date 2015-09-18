namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forced : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MenuItems", "Sauces", c => c.Int());
            AlterColumn("dbo.OrderItems", "SpiceLevel", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderItems", "SpiceLevel", c => c.Int(nullable: false));
            AlterColumn("dbo.MenuItems", "Sauces", c => c.Int(nullable: false));
        }
    }
}
