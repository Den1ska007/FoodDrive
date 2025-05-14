using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDrive.Entities;
using FoodDrive.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FoodDrive.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AdminController(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Contacts() => View();

        public IActionResult Index() => View();

        // 🟢 Admins CRUD
        public async Task<IActionResult> ListAdmin() =>
            View(await _context.Admins.ToListAsync());

        public IActionResult CreateAdmin() => View();

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(Admin admin)
        {
            if (!ModelState.IsValid) return View(admin);

            admin.PasswordHash = _passwordHasher.HashPassword(admin, admin.PasswordHash);
            admin.Role = "Admin";

            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListAdmin");
        }

        public async Task<IActionResult> EditAdmin(int id) =>
            View(await _context.Admins.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> EditAdmin(Admin admin)
        {
            if (!ModelState.IsValid) return View(admin);

            var existing = await _context.Admins.FindAsync(admin.Id);
            if (existing == null) return NotFound();

            existing.Name = admin.Name;
            if (!string.IsNullOrEmpty(admin.PasswordHash))
                existing.PasswordHash = _passwordHasher.HashPassword(existing, admin.PasswordHash);

            _context.Admins.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListAdmin");
        }

        // 🟢 Customers CRUD
        public async Task<IActionResult> List() =>
            View(await _context.Customers.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> UpdateBalance(int customerId, decimal amount)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return NotFound();

            customer.Balance += amount;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditCustomer", new { id = customerId });
        }

        public IActionResult CreateCustomer()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = new Customer
            {
                Name = model.Name,
                Address = model.Address,
                PasswordHash = _passwordHasher.HashPassword(null, model.Password),
                Role = "Customer",
                Balance = model.Balance
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Клієнта успішно створено!";
            return RedirectToAction("List");
        }

        public async Task<IActionResult> EditCustomer(int id) =>
            View(await _context.Customers.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);

            var existing = await _context.Customers.FindAsync(customer.Id);
            if (existing == null) return NotFound();

            existing.Name = customer.Name;
            existing.Address = customer.Address;
            existing.Balance = customer.Balance;

            if (!string.IsNullOrEmpty(customer.PasswordHash))
                existing.PasswordHash = _passwordHasher.HashPassword(existing, customer.PasswordHash);

            _context.Customers.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Carts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            // Видалення пов'язаних даних
            _context.Orders.RemoveRange(customer.Orders);
            _context.Carts.RemoveRange(customer.Carts);
            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        // 🟢 Dishes CRUD
        public async Task<IActionResult> ListDishes() =>
            View(await _context.Dishes.ToListAsync());

        public IActionResult CreateDish() => View();

        [HttpPost]
        public async Task<IActionResult> CreateDish(Dish dish)
        {
            if (!ModelState.IsValid) return View(dish);

            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListDishes");
        }

        public async Task<IActionResult> EditDish(int id) =>
            View(await _context.Dishes.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> EditDish(Dish dish)
        {
            if (!ModelState.IsValid) return View(dish);

            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListDishes");
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
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Dish)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> EditOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(new EditOrderViewModel
            {
                Id = order.Id,
                SelectedStatus = order.Status
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(EditOrderViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var order = await _context.Orders.FindAsync(model.Id);
            if (order == null) return NotFound();

            order.Status = model.SelectedStatus;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListOrders");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Знайти замовлення з усіма зв'язками
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    TempData["Error"] = "Замовлення не знайдено";
                    return RedirectToAction("ListOrders");
                }

                // 2. Повернути кошти клієнту
                if (order.Status != Status.Cancelled && order.User is Customer customer)
                {
                    customer.Balance += order.TotalPrice;
                    _context.Customers.Update(customer);
                }

                // 3. Видалити всі пов'язані записи
                _context.OrderItems.RemoveRange(order.Items);

                // 4. Видалити саме замовлення
                _context.Orders.Remove(order);

                // 5. Зберегти зміни
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Замовлення #{order.Id} видалено. Кошти повернено клієнту";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                
                TempData["Error"] = "Не вдалося видалити замовлення";
            }

            return RedirectToAction("ListOrders");
        }

        // 🟢 Reviews CRUD
        public async Task<IActionResult> ListReviews() =>
            View(await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Dish)
                .ToListAsync());

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

            return RedirectToAction("ListReviews");
        }

        public async Task<IActionResult> EditReview(int id) =>
            View(await _context.Reviews.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> EditReview(Review review)
        {
            if (!ModelState.IsValid) return View(review);

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListReviews");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListReviews");
        }
    }
}