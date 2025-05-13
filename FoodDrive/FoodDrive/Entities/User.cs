using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;
using FoodDrive.Interfaces;
using FoodDrive.Models;


namespace FoodDrive.Entities
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Users")]
    public abstract class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}