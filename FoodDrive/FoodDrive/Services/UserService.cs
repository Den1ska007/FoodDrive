// Services/UserService.cs
using FoodDrive.Interfaces;
using FoodDrive.Models;
using System.Security.Claims;

namespace FoodDrive.Services
{
    public class UserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public bool UpdateUserProfile(int userId, string name, string address)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                return false;

            user.Name = name;
            user.Address = address;

            _userRepository.Update(user);
            return true;
        }

        public User GetUserProfile(int userId)
        {
            return _userRepository.GetById(userId);
        }
    }
}