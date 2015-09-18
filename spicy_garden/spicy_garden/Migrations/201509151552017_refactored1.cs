namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refactored1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "OptionSelected", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MenuItems", "OptionSelected");
        }
    }
}
