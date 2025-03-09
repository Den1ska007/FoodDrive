using FoodDrive.Models;

class Program
{
    static void Main()
    {
        UserRepository userRepo = new UserRepository();
        CustomerRepository customerRepo = new CustomerRepository();
        AdminRepository adminRepo = new AdminRepository();
        OrderRepository orderRepo = new OrderRepository();
        ReviewRepository reviewRepo = new ReviewRepository();
        DishRepository dishRepo = new DishRepository();
        TimeSpan time = DateTime.Now.TimeOfDay;
        // ��������� ������������
        var tolik = new Customer(1, "Tolik", "pass123", 0969426905, "Gorenka", new List<Order>());
        var deniska = new Customer(2, "Deniska", "secure456", 0667759065, "Gorenka", new List<Order>());
        var admin = new Admin(3, "Egorik", "adminpass", 0684848377, "Gorenka");
        customerRepo.Add(tolik);
        customerRepo.Add(deniska);
        adminRepo.Add(admin);

        var italian = new TypeOfDish();

        // ��������� ������
        var pizza = new Dish(1, "Pizza", "Peperony", 120, italian, 10, new List<Review>(), 4.5f);
        dishRepo.Add(pizza);

        // ��������� ������
        var review = new Review(1, tolik, pizza, "Very tasty", 5, DateTime.Now);
        reviewRepo.Add(review);
        pizza.AddReview(review);

        // ��������� ���������
        var order = new Order(1, tolik, new List<Dish> { pizza }, Status.Pending, time);
        orderRepo.Add(order);

        // ³���������� ��� ������������
        Console.WriteLine("�����������:");
        foreach (var user in customerRepo.GetAll())
            Console.WriteLine(user.GetInfo());

        // ³���������� ��� ��������
        Console.WriteLine("��������:");
        foreach (var product in dishRepo.GetAll())
            Console.WriteLine($"{product.Id} - {product.Name} - ${product.Price}");

        // ³���������� ��� ���������
        Console.WriteLine("����������:");
        foreach (var ord in orderRepo.GetAll())
            Console.WriteLine($"Order {ord.Id}: {ord.User.Name} - ${ord.TotalPrice} - {ord.Status}");

        // ��������� �����������
        customerRepo.Remove(deniska);

        // ³���������� ������������ ���� ���������
        Console.WriteLine("����������� ���� ���������:");
        foreach (var user in customerRepo.GetAll())
            Console.WriteLine(user.GetInfo());
    }
}
