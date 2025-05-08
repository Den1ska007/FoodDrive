using System.Net.NetworkInformation;
using System.Reflection;
using FoodDrive.Models;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FoodDrive.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FoodDrive.Contollers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRepository<Admin> _adminRepository;
        private readonly IRepository<Dish> _dishRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly UserRepository _userRepository;
        private readonly CustomerRepository _customerRepository;


        public AdminController(
            IRepository<Admin> adminRepository,
            IRepository<Dish> dishRepository,
            IRepository<Order> orderRepository,
            IRepository<Review> reviewRepository,
            UserRepository userRepository,
            CustomerRepository customerRepository)
        {
            _adminRepository = adminRepository;
            _dishRepository = dishRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }
        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UpdateBalance(int customerId, decimal amount)
        {
            _customerRepository.UpdateBalance(customerId, amount);
            return RedirectToAction("EditCustomer", new { id = customerId });
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
            _userRepository.Add(admin);
            return RedirectToAction("ListAdmin");
        }
        public IActionResult EditAdmin(int id) => View(_adminRepository.GetById(id));
        
        [HttpPost]
        public IActionResult EditAdmin(Admin admin)
        {
            var existingAdmin = _adminRepository.GetById(admin.id);
            
            _adminRepository.Update(admin);
            _userRepository.Update(admin);
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
                _customerRepository.Add(customer);
                _userRepository.Add(customer);
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
                
                _customerRepository.Update(customer);
                _userRepository.Update(customer);
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
        [HttpPost]
        public IActionResult CreateDish(Dish dish)
        {
            if (!ModelState.IsValid)
            {
                // Повертаємо форму з помилками
                return View(dish);
            }
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
        


        // 🟢 Reviews CRUD
        public IActionResult ListReviews() => View(_reviewRepository.GetSorted());
        public IActionResult CreateReview()
        {
            var model = new CreateReviewViewModel
            {
                
                Users = _userRepository.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Value = u.id.ToString(),
                        Text = $"{u.Name} (ID: {u.id})"
                    }).ToList(),

                Dishes = _dishRepository.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Value = d.id.ToString(),
                        Text = $"{d.Name} (ID: {d.id})"
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateReview(CreateReviewViewModel model)
        {
            var review = new Review
            {
                UserId = model.SelectedUserId,
                DishId = model.SelectedDishId,
                Rating = model.Rating,
                Text = model.Text,
                Date = DateTime.Now
            };

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

