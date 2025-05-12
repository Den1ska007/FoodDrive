using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodDrive.Entities
{
    [Table("order_items")]
    public class OrderItem
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("order_id")]
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Column("dish_id")]
        [ForeignKey("Dish")]
        public int DishId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        public Dish Dish { get; set; }
    }
}
