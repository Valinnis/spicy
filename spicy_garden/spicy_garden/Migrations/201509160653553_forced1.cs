namespace spicy_garden.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class forced1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "Sauces", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "Sauces");
        }
    }
}
