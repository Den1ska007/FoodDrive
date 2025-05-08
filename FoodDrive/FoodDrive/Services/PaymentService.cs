using System.Threading.Tasks;

public class PaymentService : IPaymentService
{
    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount)
    {
        // Симуляція оплати через зовнішній сервіс
        await Task.Delay(500); // Імітація затримки
        return new PaymentResult { Success = true };
    }
}