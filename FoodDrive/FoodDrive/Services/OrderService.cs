using FoodDrive.Models;

public class OrderService
{
    private readonly DishRepository _dishRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly OrderRepository _orderRepository;

    public OrderService(
        DishRepository dishRepository,
        CustomerRepository customerRepository,
        OrderRepository orderRepository)
    {
        _dishRepository = dishRepository;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
    }

    public OrderResult PlaceOrder(Cart cart)
    {
        // Перевірка наявності страв та запасу
        foreach (var item in cart.Items)
        {
            var dish = _dishRepository.GetById(item.DishId);
            if (dish == null || dish.Stock < item.Quantity)
            {
                return new OrderResult { Success = false, Message = "Недостатньо товару на складі" };
            }
        }

        var customer = _customerRepository.GetById(cart.UserId);
        if (customer == null || customer.Balance < cart.Total)
        {
            return new OrderResult { Success = false, Message = "Недостатньо коштів" };
        }

        customer.Balance -= cart.Total;

        _customerRepository.Update(customer);

        var updatedCustomer = _customerRepository.GetById(customer.id);

        foreach (var item in cart.Items)
        {
            var dish = _dishRepository.GetById(item.DishId);
            dish.Stock -= item.Quantity;
            _dishRepository.Update(dish);
        }

        var order = new Order
        {
            User = customer,
            Products = cart.Items.Select(i => i.Dish).ToList(),
            TotalPrice = cart.Total,
            Status = Status.Pending
        };

        _orderRepository.Add(order);
        return new OrderResult { Success = true, Order = order };
    }
}