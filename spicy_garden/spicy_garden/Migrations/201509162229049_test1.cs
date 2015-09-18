namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderItems", "OptionId", "dbo.MenuOptions");
            DropIndex("dbo.OrderItems", new[] { "OptionId" });
            AlterColumn("dbo.OrderItems", "OptionId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderItems", "OptionId", c => c.String(maxLength: 128));
            CreateIndex("dbo.OrderItems", "OptionId");
            AddForeignKey("dbo.OrderItems", "OptionId", "dbo.MenuOptions", "Id");
        }
    }
}
