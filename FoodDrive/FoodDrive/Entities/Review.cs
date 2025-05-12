using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodDrive.Entities
{
    [Table("reviews")]
    public class Review
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Column("dish_id")]
        [ForeignKey("Dish")]
        public int DishId { get; set; }

        [Column("rating")]
        [Range(1, 5)]
        public byte Rating { get; set; }

        [Column("text")]
        [StringLength(500)]
        public string Text { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навігаційні властивості
        public User User { get; set; }
        public Dish Dish { get; set; }
    }
}
