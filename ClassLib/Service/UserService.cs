using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.IdentityModel.Tokens;

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

        public async Task<GetUserResponse?> getUserByIdService(int? Id)
        {
            var user = await _userRepository.getUserByIdAsync(Id);
            if (string.IsNullOrWhiteSpace(Id.ToString()))
            {
                throw new Exception("Id can not blank");
            }
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            var userRes = _mapper.Map<GetUserResponse>(user);
            return userRes;
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
                        var (tokenId, accessToken, refreshToken) = _jwtHelper.generateToken(user);
                        var userRes = _mapper.Map<LoginResponse>(user);
                        userRes.AccessToken = accessToken;
                        userRes.RefreshToken = refreshToken;
                        var refreshTokenModel = new RefreshToken
                        {
                            Id = long.Parse(tokenId),
                            UserId = user.Id,
                            //AccessToken = "abc not luu",
                            AccessToken = accessToken,
                            RefreshToken1 = refreshToken,
                            IsUsed = false,
                            IsRevoked = false,
                            IssuedAt = DateTime.UtcNow,
                            ExpiredAt = DateTime.UtcNow.AddMinutes(2),

                        };
                        await _userRepository.addRefreshToken(refreshTokenModel);
                        return userRes;
                    }
                }
            }
        }

        public async Task<LoginResponse?> refreshTokenAsync1(LoginResponse refreshTokenRequest)
        {
            var secretKey = _jwtHelper.getSecretKey();
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateIssuerSigningKey = true,
                //ValidIssuer = jwtSettings["Issuer"],
                //ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ValidateLifetime = false, // ko kiem token het han
            };
            //check 1: AccessToken vlid format
            var tokenInVerification = tokenHandler.ValidateToken(refreshTokenRequest.AccessToken, tokenValidateParam, out var validatedToken);

            //check 2: check algorithm
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals
                    (SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
                if (result == false)
                {
                    throw new SecurityTokenInvalidAlgorithmException("Invalid Token");
                }
            }

            //check 3" check accessToken expired
            var utcExpiredToken = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);


            var expireDate = _jwtHelper.ConvertUnixTimeToDateTime(utcExpiredToken);

            if (expireDate > DateTime.UtcNow)
            {
                throw new Exception("Access Token has not yet expired");
            }

            //check 4: refreshToken exist in DB
            var storedToken = await _userRepository.getRefreshTokenAsync(refreshTokenRequest.RefreshToken);
            if (storedToken == null)
            {
                throw new Exception("Refresh Token does not exist");
            }

            //check 5: refreshToken is revoked / used
            if (storedToken.IsUsed == true)
            {
                throw new Exception("Refresh Token has been used");
            }
            if (storedToken.IsRevoked == true)
            {
                throw new Exception("Refresh Token has been revoked");
            }

            return new LoginResponse
            {
                AccessToken = refreshTokenRequest.AccessToken,
                RefreshToken = refreshTokenRequest.RefreshToken
            };
        }

        public async Task<LoginResponse?> RefreshTokenAsync(LoginResponse refreshRequest)
        {
            if (string.IsNullOrEmpty(refreshRequest.RefreshToken))
                throw new UnauthorizedAccessException("Refresh token is required.");

            var storedToken = await _userRepository.getRefreshTokenAsync(refreshRequest.RefreshToken);
            if (storedToken == null || storedToken.IsRevoked)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (storedToken.ExpiredAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired.");

            if (storedToken.IsUsed)
                throw new UnauthorizedAccessException("Refresh token has already been used.");

            var user = await _userRepository.getUserByIdAsync(storedToken.UserId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var (tokenId, newAccessToken, newRefreshToken) = _jwtHelper.generateToken(user);

            storedToken.IsUsed = true;
            storedToken.IsRevoked = true;
            await _userRepository.updateRefreshTokenAsync(storedToken);

            var newRefreshTokenModel = new RefreshToken
            {
                Id = long.Parse(tokenId),
                UserId = user.Id,
                AccessToken = newAccessToken,
                RefreshToken1 = newRefreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };
            await _userRepository.addRefreshToken(newRefreshTokenModel);

            var userRes = _mapper.Map<LoginResponse>(user);
            userRes.AccessToken = newAccessToken;
            userRes.RefreshToken = newRefreshToken;

            return userRes;
        }

        public async Task<LoginResponse?> RefreshTokenService(LoginResponse refreshRequest)
        {
            try
            {
                var secretKey = _jwtHelper.getSecretKey();
                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenValidateParam = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = jwtSettings["Issuer"],
                    //ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                    ValidateLifetime = false, // ko kiem token het han
                };
                //check 1: AccessToken valid format
                var tokenInVerification = tokenHandler.ValidateToken(refreshRequest.AccessToken, tokenValidateParam, out var validatedToken);

                //check 2: check algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals
                        (SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        throw new SecurityTokenInvalidAlgorithmException("Invalid Token");
                    }
                }
                //check 3" check accessToken expired
                var utcExpiredToken = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = _jwtHelper.ConvertUnixTimeToDateTime(utcExpiredToken);

                if (expireDate > DateTime.UtcNow)
                {
                    throw new Exception("Access Token has not yer expired");
                }

                var refreshToken = await _userRepository.getRefreshTokenAsync(refreshRequest.RefreshToken);
                if(refreshToken == null)
                {
                    throw new UnauthorizedAccessException("Refresh token not found.");
                }

                if (refreshToken.IsUsed)
                {
                    throw new UnauthorizedAccessException("Refresh token has been used.");
                }

                if(refreshToken.IsRevoked)
                {
                    throw new UnauthorizedAccessException("Refresh token has been revoked.");
                }

                if (refreshToken.ExpiredAt < DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Refresh token has expired.");
                }

                // generate new access token
                var user = await _userRepository.getUserByIdAsync(refreshToken.UserId);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found.");
                }

                var (tokenId, newAccessToken, newRefreshToken) = _jwtHelper.generateToken(user);

                var userRes = _mapper.Map<LoginResponse>(user);
                userRes.AccessToken = newAccessToken;
                userRes.RefreshToken = newRefreshToken;

                //update refresh token
                refreshToken.IsUsed = true;
                await _userRepository.updateRefreshTokenAsync(refreshToken);

                // generate new refresh token and save to db
                var refreshTokenModel = new RefreshToken
                {
                    Id = long.Parse(tokenId),
                    UserId = user.Id,
                    AccessToken = newAccessToken,
                    RefreshToken1 = newRefreshToken,
                    IsUsed = false,
                    IsRevoked = false,
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddMinutes(2),
                };
                await _userRepository.addRefreshToken(refreshTokenModel);

                return userRes;
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("Authentication failed: " + e.Message);
            }
            catch (Exception e)
            {
                // Log error
                throw new Exception("An error occurred during token refresh: " + e.Message);
            }
        }

        public async Task<LoginResponse?> getRefreshTokenByUserIdService(int userId)
        {
            var refreshTokenModel = await _userRepository.getRefreshTokenByUserId(userId);
            if (refreshTokenModel == null)
            {
                throw new UnauthorizedAccessException("Refresh token not found.");
            }
            return new LoginResponse
            {
                AccessToken = refreshTokenModel.AccessToken,
                RefreshToken = refreshTokenModel.RefreshToken1
            };
        }





    }
}
