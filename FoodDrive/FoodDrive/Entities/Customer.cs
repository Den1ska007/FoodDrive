using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FoodDrive.Interfaces;
using FoodDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDrive.Entities
{
    [Table("users")]
    public class Customer : User
    {

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000, ErrorMessage = "Баланс не може бути від'ємним")]
        public decimal Balance { get; set; }
        public List<Order> Orders { get; set; } = new();
        public List<Cart> Carts { get; set; } = new();
    }

}