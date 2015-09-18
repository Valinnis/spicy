namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refactored : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "Category", c => c.Int(nullable: false));
            AddColumn("dbo.MenuItems", "Sauce", c => c.Int(nullable: false));
            AddColumn("dbo.MenuItems", "HasSauce", c => c.Boolean(nullable: false));
            AddColumn("dbo.MenuItems", "isSelected", c => c.Boolean());
            AddColumn("dbo.MenuItems", "Quantity", c => c.Int());
            AddColumn("dbo.MenuItems", "HalfOrder", c => c.Boolean());
            AddColumn("dbo.MenuItems", "HasHalfOrder", c => c.Boolean());
            AddColumn("dbo.MenuItems", "SpiceLevel", c => c.Int());
            AddColumn("dbo.MenuItems", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.MenuItems", "Option_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.MenuItems", "Option_Id");
            AddForeignKey("dbo.MenuItems", "Option_Id", "dbo.MenuOptions", "Id");
            DropColumn("dbo.MenuItems", "MenuCategory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MenuItems", "MenuCategory", c => c.Int(nullable: false));
            DropForeignKey("dbo.MenuItems", "Option_Id", "dbo.MenuOptions");
            DropIndex("dbo.MenuItems", new[] { "Option_Id" });
            DropColumn("dbo.MenuItems", "Option_Id");
            DropColumn("dbo.MenuItems", "Discriminator");
            DropColumn("dbo.MenuItems", "SpiceLevel");
            DropColumn("dbo.MenuItems", "HasHalfOrder");
            DropColumn("dbo.MenuItems", "HalfOrder");
            DropColumn("dbo.MenuItems", "Quantity");
            DropColumn("dbo.MenuItems", "isSelected");
            DropColumn("dbo.MenuItems", "HasSauce");
            DropColumn("dbo.MenuItems", "Sauce");
            DropColumn("dbo.MenuItems", "Category");
        }
    }
}
