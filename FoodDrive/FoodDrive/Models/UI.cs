using FoodDrive.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FoodDrive.Interfaces;


namespace FoodDrive.Controllers
{
    public class AdminController : Controller
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
            return View();
        }

        // 🟢 Admins CRUD
        public IActionResult ListAdmin() => View(_adminRepository.GetSorted());
        
        public IActionResult CreateAdmin() => View();
        
        [HttpPost]
        public IActionResult CreateAdmin(Admin admin)
        {
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
            _customerRepository.Add(customer);
            return RedirectToAction("List");
        }
        public IActionResult EditCustomer(int id) => View(_customerRepository.GetById(id));
        [HttpPost]
        public IActionResult EditCustomer(Customer customer)
        {
            var existingCustomer = _customerRepository.GetById(customer.id);
            if (existingCustomer != null)
            {
                _customerRepository.Remove(existingCustomer);
            }
            _customerRepository.Add(customer);
            return RedirectToAction("List");
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

