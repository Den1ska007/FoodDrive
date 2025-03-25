using FoodDrive.Interfaces;
using FoodDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FoodDrive.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerController(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET: /Customer/Index
        public IActionResult Index()
        {
            // Отримуємо список всіх користувачів
            var customers = _customerRepository.GetAll() as List<Customer>;
            return View(customers); // Передаємо список у вигляд
        }

        // POST: /Customer/Add
        [HttpPost]
        public IActionResult Add(string name, string password, int mobilenum, string adres)
        {
            var customer = new Customer(0, name, password, mobilenum, adres, new List<Order>());
            _customerRepository.Add(customer);
            return RedirectToAction("Index");
        }

        // POST: /Customer/Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer != null)
            {
                _customerRepository.Remove(customer);
            }
            return RedirectToAction("Index");
        }
    }
}
