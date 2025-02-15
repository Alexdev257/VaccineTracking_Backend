using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using BCrypt.Net;
using ClassLib.DTO.User;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.Extensions.Configuration;

namespace ClassLib.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;

        public UserService(UserRepository userRepository, IMapper mapper, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
        }


        public async Task<List<User>> getAllService()
        {
            return await _userRepository.getAll();
        }

        public async Task<LoginResponse?> loginAsync(LoginRequest loginRequest)
        {
            var user = await _userRepository.getUserByUsernameAsync(loginRequest.Username);

            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                throw new ArgumentException("Username or Password cannot be empty.");
            }
            else
            {
                
                if (user == null) 
                {
                    throw new UnauthorizedAccessException("Account does not exist.");
                }
                else
                {
                    if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                    {
                        throw new UnauthorizedAccessException("Incorrect password.");
                    }
                    else
                    {
                        var userRes = _mapper.Map<LoginResponse>(user);
                        var token = _jwtHelper.generateToken(userRes);
                        userRes.Token = token;
                        return userRes;
                    }
                }
            }
        }

        public async Task<RegisterResponse?> registerAsync(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Name) ||
                string.IsNullOrWhiteSpace(registerRequest.PhoneNumber) ||
                string.IsNullOrWhiteSpace(registerRequest.Username) ||
                string.IsNullOrWhiteSpace(registerRequest.Password))
            {
                throw new Exception("Vui lòng nhập đầy đủ thông tin.");
            }
            var existUsername = await _userRepository.getUserByUsernameAsync(registerRequest.Username);
            if (existUsername != null)
            {
                throw new Exception("Exist username. Please choose another.");
            }

            var existPhone = await _userRepository.getUserByUsernameAsync(registerRequest.PhoneNumber);
            if (existPhone != null)
            {
                throw new Exception("Exist phone number. Please choosee another.");
            }

            string encryptPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            var user = _mapper.Map<User>(registerRequest);
            user.Password = encryptPassword;
            user.Role = "User";
            user.CreatedAt = DateTime.UtcNow;
            user.Status = "Active";

            await _userRepository.addUserAsync(user);
            var result = _mapper.Map<RegisterResponse>(user);
            return result; 
        }
    }
}
