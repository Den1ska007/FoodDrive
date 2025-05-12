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

        [Column("balance")]
        [Precision(10, 2)]
        public decimal Balance { get; set; } = 0;
        
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Cart> Carts { get; set; } = new List<Cart>();
    }

}