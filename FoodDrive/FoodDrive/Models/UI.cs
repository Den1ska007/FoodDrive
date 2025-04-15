using FoodDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FoodDrive.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace FoodDrive.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRepository<Admin> _adminRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Dish> _dishRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly UserRepository _userRepository;


        public AdminController(
            IRepository<Admin> adminRepository,
            IRepository<Customer> customerRepository,
            IRepository<Dish> dishRepository,
            IRepository<Order> orderRepository,
            IRepository<Review> reviewRepository,
            UserRepository userRepository)
        {
            _adminRepository = adminRepository;
            _customerRepository = customerRepository;
            _dishRepository = dishRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
        }
        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        // 🟢 Admins CRUD
        public IActionResult ListAdmin() => View(_adminRepository.GetSorted());
        
        public IActionResult CreateAdmin() => View();
        
        [HttpPost]
        public IActionResult CreateAdmin(Admin admin)
        {
            admin.id = _adminRepository.GetAll().Any() ?
                  _adminRepository.GetAll().Max(a => a.id) + 1 : 1;
            _adminRepository.Add(admin);
            return RedirectToAction("ListAdmin");
        }
        public IActionResult EditAdmin(int id) => View(_adminRepository.GetById(id));
        
        [HttpPost]
        public IActionResult EditAdmin(Admin admin)
        {
            var existingAdmin = _adminRepository.GetById(admin.id);
            if (existingAdmin != null)
            {
                _adminRepository.Remove(existingAdmin);
            }
            _adminRepository.Add(admin);
            return RedirectToAction("ListAdmin");
        }

        // 🟢 Customers CRUD
        public IActionResult List() => View(_customerRepository.GetSorted());
        public IActionResult CreateCustomer() => View();
        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Якщо потрібно встановити ID
                customer.id = _customerRepository.GetAll().Any() ?
                            _customerRepository.GetAll().Max(c => c.id) + 1 : 1;

                _customerRepository.Add(customer);
                return RedirectToAction("List");
            }
            return View(customer);
        }
        public IActionResult EditCustomer(int id) => View(_customerRepository.GetById(id));
        [HttpPost]
        public IActionResult EditCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = _customerRepository.GetById(customer.id);
                if (existingCustomer != null)
                {
                    _customerRepository.Remove(existingCustomer);
                }
                _customerRepository.Add(customer);
                return RedirectToAction("List");
            }
            return View(customer);
        }
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer != null)
            {
                _customerRepository.Remove(customer);
            }
            return RedirectToAction("List");
        }
        // 🟢 Dishes CRUD
        public IActionResult ListDishes() => View(_dishRepository.GetSorted());
        public IActionResult CreateDish() => View();
        [HttpPost]
        public IActionResult CreateDish(Dish dish)
        {
            _dishRepository.Add(dish);
            return RedirectToAction("ListDishes");
        }
        public IActionResult EditDish(int id) => View(_dishRepository.GetById(id));
        [HttpPost]
        public IActionResult EditDish(Dish dish)
        {
            var existingDish = _dishRepository.GetById(dish.id);
            if (existingDish != null)
            {
                _dishRepository.Remove(existingDish);
            }
            _dishRepository.Add(dish);
            return RedirectToAction("ListDishes");
        }

        public IActionResult DeleteDish(int id)
        {
            var dish = _dishRepository.GetById(id);
            if (dish != null)
            {
                _dishRepository.Remove(dish);
            }
            return RedirectToAction("ListDishes");
        }

        // 🟢 Orders CRUD
        public IActionResult ListOrders() => View(_orderRepository.GetSorted());
        public IActionResult CreateOrder() => View();
        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            _orderRepository.Add(order);
            return RedirectToAction("ListOrders");
        }
        public IActionResult EditOrder(int id) => View(_orderRepository.GetById(id));
        [HttpPost]
        public IActionResult EditOrder(Order order)
        {
            var existingOrder = _orderRepository.GetById(order.id);
            if (existingOrder != null)
            {
                _orderRepository.Remove(existingOrder);
            }
            _orderRepository.Add(order);
            return RedirectToAction("ListOrders");
        }

        // 🟢 Reviews CRUD
        public IActionResult ListReviews() => View(_reviewRepository.GetSorted());
        public IActionResult CreateReview() => View();
        [HttpPost]
        public IActionResult CreateReview(Review review)
        {
            _reviewRepository.Add(review);
            return RedirectToAction("ListReviews");
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
            return RedirectToAction("ListReviews");
        }
    }
}

