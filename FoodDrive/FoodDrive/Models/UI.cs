using FoodDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace StressLessStore.Controllers
{
    public class AdminController : Controller, BaseEntity
    {
        private readonly AdminRepository _adminRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly OrderRepository _orderRepository;
        private readonly DishRepository _dishRepository;
        private readonly ReviewRepository _reviewRepository;

        public AdminController()
        {
            _adminRepository = new AdminRepository();
            _customerRepository = new CustomerRepository();
            _orderRepository = new OrderRepository();
            _dishRepository = new DishRepository();
            _reviewRepository = new ReviewRepository();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Menu"); // Головна = меню
        }

        public IActionResult Menu()
        {
            return View();
        }

        // 🟢 Admins CRUD
        public IActionResult Admins() => View(_adminRepository.GetSorted());
        public IActionResult CreateAdmin() => View();
        [HttpPost]
        public IActionResult CreateAdmin(Admin admin)
        {
            _adminRepository.Add(admin);
            return RedirectToAction("Admins");
        }
        public IActionResult EditAdmin(int id) => View(_adminRepository.GetById(id));
        [HttpPost]
        public IActionResult EditAdmin(Admin admin)
        {
            var existingAdmin = _adminRepository.GetById(admin.Id);
            if (existingAdmin != null)
            {
                _adminRepository.Remove(existingAdmin);
            }
            _adminRepository.Add(admin);
            return RedirectToAction("Admins");
        }

        // 🟢 Customers CRUD
        public IActionResult Customers() => View(_customerRepository.GetSorted());
        public IActionResult CreateCustomer() => View();
        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            _customerRepository.Add(customer);
            return RedirectToAction("Customers");
        }
        public IActionResult EditCustomer(int id) => View(_customerRepository.GetById(id));
        [HttpPost]
        public IActionResult EditCustomer(Customer customer)
        {
            var existingCustomer = _customerRepository.GetById(customer.Id);
            if (existingCustomer != null)
            {
                _customerRepository.Remove(existingCustomer);
            }
            _customerRepository.Add(customer);
            return RedirectToAction("Customers");
        }

        // 🟢 Dishes CRUD
        public IActionResult Dishes() => View(_dishRepository.GetSorted());
        public IActionResult CreateDish() => View();
        [HttpPost]
        public IActionResult CreateDish(Dish dish)
        {
            _dishRepository.Add(dish);
            return RedirectToAction("Dishes");
        }
        public IActionResult EditDish(int id) => View(_dishRepository.GetById(id));
        [HttpPost]
        public IActionResult EditDish(Dish dish)
        {
            var existingDish = _dishRepository.GetById(dish.Id);
            if (existingDish != null)
            {
                _dishRepository.Remove(existingDish);
            }
            _dishRepository.Add(dish);
            return RedirectToAction("Dishes");
        }


        // 🟢 Orders CRUD
        public IActionResult Orders() => View(_orderRepository.GetSorted());
        public IActionResult CreateOrder() => View();
        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            _orderRepository.Add(order);
            return RedirectToAction("Orders");
        }
        public IActionResult EditOrder(int id) => View(_orderRepository.GetById(id));
        [HttpPost]
        public IActionResult EditOrder(Order order)
        {
            var existingOrder = _orderRepository.GetById(order.Id);
            if (existingOrder != null)
            {
                _orderRepository.Remove(existingOrder);
            }
            _orderRepository.Add(order);
            return RedirectToAction("Orders");
        }

        // 🟢 Reviews CRUD
        public IActionResult Reviews() => View(_reviewRepository.GetSorted());
        public IActionResult CreateReview() => View();
        [HttpPost]
        public IActionResult CreateReview(Review review)
        {
            _reviewRepository.Add(review);
            return RedirectToAction("Reviews");
        }
        public IActionResult EditReview(int id) => View(_reviewRepository.GetById(id));
        [HttpPost]
        public IActionResult EditReview(Review review)
        {
            var existingReview = _reviewRepository.GetById(review.Id);
            if (existingReview != null)
            {
                _reviewRepository.Remove(existingReview);
            }
            _reviewRepository.Add(review);
            return RedirectToAction("Reviews");
        }
    }
}

