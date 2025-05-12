using System;
using System.ComponentModel.DataAnnotations.Schema;
using FoodDrive.Interfaces;
using FoodDrive.Models;

namespace FoodDrive.Entities
{
    [Table("users")]
    public class Admin : User
    {
        [Column("permission_level")]
        public int PermissionLevel { get; set; } = 1;
    }
}
