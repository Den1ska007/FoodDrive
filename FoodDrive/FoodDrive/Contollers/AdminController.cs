using System.Net.NetworkInformation;
using System.Reflection;
using FoodDrive.Models;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FoodDrive.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using FoodDrive.Entities;
using Microsoft.EntityFrameworkCore;


namespace FoodDrive.Contollers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> ListAdmin()
        {
            var admins = await _context.Users
                .OfType<Admin>()
                .AsNoTracking()
                .OrderBy(a => a.Id)
                .ToListAsync();

            return View(admins);
        }

        [HttpGet]
        public IActionResult CreateAdmin() => View();

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin);
            }

            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(admin.PasswordHash);
            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListAdmin");
        }

        [HttpGet]
        public async Task<IActionResult> EditAdmin(int id)
        {
            var admin = await _context.Users
                .OfType<Admin>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return admin == null ? NotFound() : View(admin);
        }

        [HttpPost]
        public async Task<IActionResult> EditAdmin(Admin updatedAdmin)
        {
            if (!ModelState.IsValid) return View(updatedAdmin);

            var existingAdmin = await _context.Users
                .OfType<Admin>()
                .FirstOrDefaultAsync(a => a.Id == updatedAdmin.Id);

            if (existingAdmin == null) return NotFound();

            existingAdmin.Name = updatedAdmin.Name;

            if (!string.IsNullOrEmpty(updatedAdmin.PasswordHash))
            {
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedAdmin.PasswordHash);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListAdmin));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Users
                .OfType<Admin>()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admin == null) return NotFound();

            _context.Users.Remove(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListAdmin));
        }
        // 🟢 Customers CRUD
        public async Task<IActionResult> List()
        {
            var customers = await _context.Users
                .OfType<Customer>()
                .AsNoTracking()
                .Include(c => c.Orders)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(customers);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBalance(int customerId, decimal amount)
        {
            var customer = await _context.Users
                .OfType<Customer>()
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null) return NotFound();

            customer.Balance += amount;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditCustomer), new { id = customerId });
        }
        [HttpGet]
        public IActionResult CreateCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);

            customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(customer.PasswordHash);
            await _context.Users.AddAsync(customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Клієнта успішно створено!";
            return RedirectToAction(nameof(List));
        }
        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var customer = await _context.Users
                .OfType<Customer>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return customer == null ? NotFound() : View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer updatedCustomer)
        {
            if (!ModelState.IsValid) return View(updatedCustomer);

            var existingCustomer = await _context.Users
                .OfType<Customer>()
                .FirstOrDefaultAsync(c => c.Id == updatedCustomer.Id);

            if (existingCustomer == null) return NotFound();

            existingCustomer.Name = updatedCustomer.Name;
            existingCustomer.Address = updatedCustomer.Address;
            existingCustomer.Balance = updatedCustomer.Balance;

            if (!string.IsNullOrEmpty(updatedCustomer.PasswordHash))
            {
                existingCustomer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedCustomer.PasswordHash);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Users
                .OfType<Customer>()
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            _context.Users.Remove(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }
        // 🟢 Dishes CRUD
        public async Task<IActionResult> ListDishes()
        {
            var dishes = await _context.Dishes
                .AsNoTracking()
                .Include(d => d.Reviews)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(dishes);
        }

        [HttpGet]
        public IActionResult CreateDish() => View();

        [HttpPost]
        public async Task<IActionResult> CreateDish(Dish dish)
        {
            if (!ModelState.IsValid) return View(dish);

            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Страву успішно створено!";
            return RedirectToAction(nameof(ListDishes));
        }

        [HttpGet]
        public async Task<IActionResult> EditDish(int id)
        {
            var dish = await _context.Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            return dish == null ? NotFound() : View(dish);
        }

        [HttpPost]
        public async Task<IActionResult> EditDish(Dish updatedDish)
        {
            if (!ModelState.IsValid) return View(updatedDish);

            _context.Dishes.Update(updatedDish);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListDishes));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListDishes");
        }


        // 🟢 Orders CRUD
        public async Task<IActionResult> ListOrders()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Dish)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


        public IActionResult CreateOrder() => View();
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (!ModelState.IsValid) return View(order);

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListOrders");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, Status status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListOrders));
        }

        [HttpGet]
        public async Task<IActionResult> EditOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            var model = new EditOrderViewModel
            {
                Id = order.Id,
                SelectedStatus = order.Status
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditOrder(EditOrderViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var order = await _context.Orders.FindAsync(model.Id);
            if (order == null) return NotFound();

            order.UserId = model.Id;
            order.Status = model.SelectedStatus;
            order.TotalPrice = order.Items.Sum(i => i.Dish.Price * i.Quantity);
            order.OrderDate = DateTime.UtcNow;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListOrders");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListOrders));
        }

        // 🟢 Reviews CRUD
        public async Task<IActionResult> ListReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Dish)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }
        [HttpGet]
        public async Task<IActionResult> CreateReview()
        {
            var model = new CreateReviewViewModel
            {
                Users = await _context.Users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Name} (ID: {u.Id})"
                    })
                    .ToListAsync(),
                Dishes = await _context.Dishes
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = $"{d.Name} (ID: {d.Id})"
                    })
                    .ToListAsync()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var review = new Review
            {
                UserId = model.SelectedUserId,
                DishId = model.SelectedDishId,
                Rating = model.Rating,
                Text = model.Text,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListReviews));
        }
        public async Task<IActionResult> EditReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Dish)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(int id, Review model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                return NotFound();
            }

            try
            {
                // Оновлюємо тільки поля, які дозволено змінювати
                existingReview.Rating = model.Rating;
                existingReview.Text = model.Text;
                existingReview.CreatedAt = DateTime.UtcNow;

                _context.Update(existingReview);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Відгук успішно оновлено";
                return RedirectToAction("ListReviews");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError("", "Конфлікт редагування. Запис був змінений іншим користувачем.");
                return View(model);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Помилка збереження змін. Спробуйте ще раз.");
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListReviews));
        }
        private bool DishExists(int id) => _context.Dishes.Any(e => e.Id == id);
        private bool UserExists(int id) => _context.Users.Any(e => e.Id == id);
    }
}

