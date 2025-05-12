using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;
using FoodDrive.Interfaces;
using FoodDrive.Models;


namespace FoodDrive.Entities
{

    [Table("users")]
    public class User
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column("password_hash")]
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [Column("address")]
        [StringLength(200)]
        public string Address { get; set; }
        [Column("role")]
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Customer";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}