namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refactored4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MenuItems", "Option_Id", "dbo.MenuOptions");
            DropIndex("dbo.MenuItems", new[] { "Option_Id" });
            AddColumn("dbo.MenuItems", "Sauces", c => c.Int(nullable: false));
            AddColumn("dbo.OrderItems", "Option_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.OrderItems", "Option_Id");
            AddForeignKey("dbo.OrderItems", "Option_Id", "dbo.MenuOptions", "Id");
            DropColumn("dbo.MenuItems", "Sauce");
            DropColumn("dbo.MenuItems", "isSelected");
            DropColumn("dbo.MenuItems", "Quantity");
            DropColumn("dbo.MenuItems", "HalfOrder");
            DropColumn("dbo.MenuItems", "HasHalfOrder");
            DropColumn("dbo.MenuItems", "OptionSelected");
            DropColumn("dbo.MenuItems", "SpiceLevel");
            DropColumn("dbo.MenuItems", "Discriminator");
            DropColumn("dbo.MenuItems", "Option_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MenuItems", "Option_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.MenuItems", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.MenuItems", "SpiceLevel", c => c.Int());
            AddColumn("dbo.MenuItems", "OptionSelected", c => c.String());
            AddColumn("dbo.MenuItems", "HasHalfOrder", c => c.Boolean());
            AddColumn("dbo.MenuItems", "HalfOrder", c => c.Boolean());
            AddColumn("dbo.MenuItems", "Quantity", c => c.Int());
            AddColumn("dbo.MenuItems", "isSelected", c => c.Boolean());
            AddColumn("dbo.MenuItems", "Sauce", c => c.Int(nullable: false));
            DropForeignKey("dbo.OrderItems", "Option_Id", "dbo.MenuOptions");
            DropIndex("dbo.OrderItems", new[] { "Option_Id" });
            DropColumn("dbo.OrderItems", "Option_Id");
            DropColumn("dbo.MenuItems", "Sauces");
            CreateIndex("dbo.MenuItems", "Option_Id");
            AddForeignKey("dbo.MenuItems", "Option_Id", "dbo.MenuOptions", "Id");
        }
    }
}
