using FoodDrive.Interfaces;
using FoodDrive.Models;

public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public bool UpdateUserProfile(User user)
    {
        try
        {
            var existingUser = _userRepository.GetById(user.id);
            if (existingUser == null) return false;

            existingUser.Name = user.Name;
            existingUser.Address = user.Address;

            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = user.Password; // Хешування відбувається автоматично
            }

            _userRepository.Update(existingUser);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public User GetUserProfile(int userId)
    {
        return _userRepository.GetById(userId);
    }
}