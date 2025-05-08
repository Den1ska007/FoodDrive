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
        if (cart == null) throw new ArgumentNullException(nameof(cart));
        foreach (var item in cart.Items)
        {
            var dish = _dishRepository.GetById(item.DishId);
            if (dish == null)
                return new OrderResult { Success = false, Message = $"Страва не знайдена: {item.DishId}" };
            if (dish.stock < item.Quantity)
            {
                return new OrderResult

                {
                    Success = false,
                    Message = $"Недостатньо '{dish.Name}' на складі"
                };
            }
        }

        var customer = _customerRepository.GetById(cart.UserId);
        if (customer == null)
        { 
            return new OrderResult { Success = false, Message = "Клієнт не знайдений" };
        }
        if (customer.Balance < cart.Total)
            return new OrderResult { Success = false, Message = "Недостатньо коштів" };

        customer.Balance -= cart.Total;
        _customerRepository.Update(customer);

        foreach (var item in cart.Items)
        {
            var dish = _dishRepository.GetById(item.DishId);
            dish.stock -= item.Quantity;
            _dishRepository.Update(dish);
        }

        var dishes = cart.Items.Select(item => _dishRepository.GetById(item.DishId)).ToList();

        var order = new Order
        {
            User = customer,
            Products = dishes,
            TotalPrice = cart.Total,
            Status = Status.Completed
        };

        _orderRepository.Add(order);

        return new OrderResult { Success = true, Order = order };
    }
}