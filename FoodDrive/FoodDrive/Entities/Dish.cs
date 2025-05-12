// Dish.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FoodDrive.Interfaces;
using FoodDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDrive.Entities
{
    [Table("dishes")]
    public class Dish
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100)]
        public string Name { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [Column("price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна має бути додатньою")]
        [Precision(10, 2)]
        public decimal Price { get; set; }

        [Column("stock")]
        public int Stock { get; set; }

        [Column("rating")]
        [Range(1, 5, ErrorMessage = "Рейтинг має бути від 1 до 5")]
        public int Rating { get; set; }

        // Додано нову властивість
        [Column("type_of_dish", TypeName = "nvarchar(24)")]
        [Required(ErrorMessage = "Тип кухні обов'язковий")]
        public TypeOfDish TypeOfDish { get; set; }

        // Навігаційні властивості
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}