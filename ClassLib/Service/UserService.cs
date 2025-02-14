using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.DTO.Request;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        //public UserService(UserRepository userRepository) => _userRepository = userRepository;

        public async Task<List<User>> getAllService()
        {
            return await _userRepository.getAll();
        }

        public async Task<User?> loginAsync(LoginRequest loginRequest)
        {
            if(loginRequest.username.Equals("") || loginRequest.password.Equals(""))
            {
                throw new ArgumentException("username or password can not be empty");
            }

            var user = await _userRepository.getUserByUsernameAsync(loginRequest.username);
            if (user != null)
            {
                if (loginRequest.password == user.Password)
                {
                    return user;
                }
                else
                {
                    throw new UnauthorizedAccessException("Incorrect password.");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Account is nor exist");
            }
        }

        public async Task<User?> registerAsync(RegisterRequest registerRequest)
        {
            var user = await _userRepository.getUserByUsernameAsync(registerRequest.Username);
            if(user != null)
            {
                throw new Exception("Exist username. Please choose another one.");
            }
            //else
            //{
            //    //check another field
            //}
            return user;
        }
    }
}
