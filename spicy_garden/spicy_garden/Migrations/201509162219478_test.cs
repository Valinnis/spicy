namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.OrderItems", name: "Option_Id", newName: "OptionId");
            RenameIndex(table: "dbo.OrderItems", name: "IX_Option_Id", newName: "IX_OptionId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.OrderItems", name: "IX_OptionId", newName: "IX_Option_Id");
            RenameColumn(table: "dbo.OrderItems", name: "OptionId", newName: "Option_Id");
        }
    }
}
