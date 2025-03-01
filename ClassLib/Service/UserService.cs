using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using BCrypt.Net;
using ClassLib.DTO.User;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ClassLib.DTO.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using static System.Net.WebRequestMethods;

namespace ClassLib.Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        //private readonly FirebaseAuth _firebaseAuth;
        //private readonly HttpClient _httpClient;
        //private readonly string _firebaseApiKey;

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;

        public UserService(UserRepository userRepository, IMapper mapper, JwtHelper jwtHelper, IConfiguration configuration, EmailService emailService, IWebHostEnvironment env, IMemoryCache cache)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtHelper = jwtHelper;

            //var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/firebase-config.json");
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile(firebaseConfigPath)
            //});
            //_firebaseAuth = FirebaseAuth.DefaultInstance;
            //_httpClient = new HttpClient();
            //_firebaseApiKey = configuration["Firebase:ApiKey"];
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _emailService = emailService;
            _env = env;
            _cache = cache;
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
                throw new Exception("Exist phone number. Please choose another.");
            }

            var existGmail = await _userRepository.getUserByGmailAsync(registerRequest.Gmail);
            if (existGmail != null)
            {
                throw new Exception("Exist gmail. Please choose another.");
            }
            try
            {
                string encryptPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
                var user = _mapper.Map<User>(registerRequest);
                //user.Id = 1;
                user.Password = encryptPassword;
                user.Role = "User";
                user.CreatedAt = DateTime.UtcNow;
                user.Status = "Active";

                await _userRepository.addUserAsync(user);
                var result = _mapper.Map<RegisterResponse>(user);
                return result;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Lỗi khi lưu dữ liệu: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi không xác định: {ex.Message}");
            }
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
                    else if (user.Status == "Inactive")
                    {
                        throw new UnauthorizedAccessException("Account is inactive.");
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
                            ExpiredAt = DateTime.UtcNow.AddDays(1),

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
                if (refreshToken == null)
                {
                    throw new UnauthorizedAccessException("Refresh token not found.");
                }

                if (refreshToken.IsUsed)
                {
                    throw new UnauthorizedAccessException("Refresh token has been used.");
                }

                if (refreshToken.IsRevoked)
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
                    ExpiredAt = DateTime.UtcNow.AddDays(1),
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

        public async Task<LoginResponse?> getRefreshTokenByUserIdService(int? userId)
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

        /*public async Task<string> sendOtpAsync(string phoneNumber)
        {
            var user = await _userRepository.getUserByPhoneAsync(phoneNumber);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            try
            {
                var firebaseAuth = FirebaseAuth.DefaultInstance;
                var verificationId = await firebaseAuth.CreateSessionCookieAsync(phoneNumber, new FirebaseAdmin.Auth.SessionCookieOptions
                {
                    ExpiresIn = TimeSpan.FromMinutes(5),
                });
                return verificationId;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during send OTP: " + e.Message);
            }
        }

        public async Task<LoginResponse?> verifyOtpAsync(string phoneNumber, string otp)
        {
            try
            {
                var firebaseAuth = FirebaseAuth.DefaultInstance;
                var signInResult = await firebaseAuth.VerifyPhoneNumberAsync(phoneNumber, otp);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during verify OTP: " + e.Message);
            }
        }*/

        /*public async Task<string> sendOtpAsync(string phoneNumber)
        {
            var user = await _userRepository.getUserByPhoneAsync(phoneNumber);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={_firebaseApiKey}";

            var payload = new { phoneNumber = phoneNumber };
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("An error occurred during send OTP: " + jsonResponse);
            }

            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
            return result["sessionInfo"];
        }

        public async Task<LoginResponse> verifyOtpAsync(string sessionInfo, string otp)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPhoneNumber?key={_firebaseApiKey}";

            var payload = new { sessionInfo = sessionInfo, code = otp };
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("An error occurred during verify OTP: " + jsonResponse);
            }

            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
            string phoneNumber = result["phoneNumber"];

            var user = await _userRepository.getUserByPhoneAsync(phoneNumber);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

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
                ExpiredAt = DateTime.UtcNow.AddDays(1),

            };

            await _userRepository.addRefreshToken(refreshTokenModel);
            return userRes;
        }*/

        public async Task<bool> VerifyRecaptchaAsync(string userIp)
        {
            var secretKey = _configuration["GoogleRecaptcha:SecretKey"];
            var requestUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&remoteip={userIp}";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(requestUrl);
            var jsonResponse = JObject.Parse(response);

            return jsonResponse["success"]?.Value<bool>() == true;
        }
        public async Task<string?> SendOtpAsync(string phoneNumber)
        {
            /*var firebaseAuth = FirebaseAuth.DefaultInstance;
            var session = await firebaseAuth.CreateSessionCookieAsync(phoneNumber, new FirebaseAdmin.Auth.SessionCookieOptions
            {
                ExpiresIn = TimeSpan.FromMinutes(5),
            });
            return session;

            using (var httpClient = new HttpClient())
            {
                var firebaseApiKey = _configuration["Firebase:APIKey"]; // Lấy API Key từ appsettings.json
                var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={firebaseApiKey}";

                var requestBody = new
                {
                    phoneNumber = phoneNumber,
                    recaptchaToken = "RECAPTCHA_TOKEN" // Tùy chọn, nếu bạn sử dụng Google Recaptcha
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(requestUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Lỗi gửi OTP: {responseString}");
                }

                var responseJson = JsonConvert.DeserializeObject<JObject>(responseString);
                return responseJson?["sessionInfo"]?.ToString();
            }*/

            //string phoneNumber = phoneNumber1;
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = "+84" + phoneNumber.Substring(1);
            }
            //if (phoneNumber.StartsWith("+"){
            //    phoneNumber = phoneNumber;
            //}

            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber), "Phone number cannot be null or empty.");

            // Kiểm tra _configuration có bị null không
            if (_configuration == null)
                throw new Exception("_configuration is not initialized.");

            var firebaseApiKey = _configuration["Firebase:APIKey"];
            if (string.IsNullOrEmpty(firebaseApiKey))
                throw new Exception("Firebase API Key is missing in appsettings.json.");

            var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={firebaseApiKey}";

            var requestBody = new
            {
                phoneNumber = phoneNumber,
                //recaptchaToken = "RECAPTCHA_TOKEN" // Tùy chọn, nếu bạn sử dụng Google Recaptcha
            };

            using (var httpClient = new HttpClient())
            {
                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                Console.WriteLine($"Sending OTP to {phoneNumber} - Request: {jsonRequest}");

                HttpResponseMessage response;
                try
                {
                    response = await httpClient.PostAsync(requestUrl, content);
                }
                catch (HttpRequestException httpEx)
                {
                    throw new Exception($"Lỗi khi gửi yêu cầu đến Firebase: {httpEx.Message}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Firebase Response: {responseString}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Lỗi gửi OTP: {response.StatusCode} - {responseString}");
                }

                var responseJson = JsonConvert.DeserializeObject<JObject>(responseString);
                var sessionInfo = responseJson?["sessionInfo"]?.ToString();

                if (string.IsNullOrEmpty(sessionInfo))
                {
                    throw new Exception("Không nhận được sessionInfo từ Firebase.");
                }

                return sessionInfo;
            }
        }

        /*public async Task<LoginResponse> VerifyOtpAsync(VerifyOtpRequest request)
        {
            var firebaseAuth = FirebaseAuth.DefaultInstance;
            var decodedToken = await firebaseAuth.VerifyIdTokenAsync(request.IdToken);

            var user = await _userRepository.getUserByPhoneAsync(request.PhoneNumber);
            if (user == null) throw new Exception("User not found!");

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
                ExpiredAt = DateTime.UtcNow.AddDays(1),

            };
            await _userRepository.addRefreshToken(refreshTokenModel);

            return userRes;
        }*/

        public async Task<string?> VerifyOtpAsync(string sessionInfo, string otp)
        {
            using (var httpClient = new HttpClient())
            {
                var firebaseApiKey = _configuration["Firebase:APIKey"];
                var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPhoneNumber?key={firebaseApiKey}";

                var requestBody = new
                {
                    sessionInfo = sessionInfo,
                    code = otp
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(requestUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Lỗi xác thực OTP: {responseString}");
                }

                var responseJson = JsonConvert.DeserializeObject<JObject>(responseString);
                return responseJson?["idToken"]?.ToString(); // ID Token dùng để xác thực người dùng
            }
        }

        public async Task<bool> updateUserAsync(int userId, UpdateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId.ToString()))
            {
                throw new ArgumentNullException("Id can not blank");
            }
            {

            }
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.DateOfBirth.ToString()) ||
                string.IsNullOrWhiteSpace(request.Gmail) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                throw new ArgumentNullException("Please fill all field.");
            }
            var user = await _userRepository.getUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
                //return false;
            }

            //user.Username = request.Username;
            user.Name = request.Name;
            user.DateOfBirth = request.DateOfBirth;
             user.Gender = request.Gender;
            user.Gmail = request.Gmail;
            user.PhoneNumber = request.PhoneNumber;
            return await _userRepository.updateUser(user);
        }

        public async Task<bool> deleteUserAsync(int userId)
        {
            if (string.IsNullOrWhiteSpace(userId.ToString()))
            {
                throw new ArgumentNullException("Id can not blank");
            }
            var user = await _userRepository.getUserByIdAsync(userId);
            //var user = new User
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return await _userRepository.deleteUser(user);

        }

        public async Task<bool> forgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.getUserByGmailAsync(request.Gmail);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            string templatePath = Path.Combine(_env.WebRootPath, "templates", "newEmailTemplate.html");

            var verifyCode = VerifyCodeHelper.GenerateSixRandomCode();
            _cache.Set("VerifyCodeKey", verifyCode, TimeSpan.FromMinutes(5));
            _cache.Set("GmailKey", request.Gmail, TimeSpan.FromMinutes(5));
            //_cache.Set("UserNameKey", request.Username, TimeSpan.FromMinutes(5));
            //_cache.Set("NewPasswordKey", request.newPassword, TimeSpan.FromMinutes(5));
            var placeholders = new Dictionary<string, string>
            {
                { "UserName", user.Name},
                { "VerifyCode", verifyCode}
            };

            return await _emailService.sendEmailService(user.Gmail, "ResetPassword", templatePath, placeholders);

        }

        public async Task<bool> verifyForgotPasswordCodeAsync(VerifyForgotPasswordRequest request)
        {
            bool check = false;
            if (_cache.TryGetValue("VerifyCodeKey", out string? storedVerifyCode))
            {
                
                if (storedVerifyCode == request.VerifyCode)
                {
                    //string hashPassword = BCrypt.Net.BCrypt.HashPassword(_cache.Get<string>("NewPasswordKey"));
                    _cache.Remove("VerifyCodeKey");
                    check = true;
                    return check;
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid verify code.");
                    return check;
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Verify code has expired.");
                return check;
            }
        }

        public async Task<bool> changePasswordAsync(ChangePasswordRequest request)
        {
            bool check = false;
            if(_cache.TryGetValue("GmailKey", out string? storedGmail))
            {
                var user = await _userRepository.getUserByGmailAsync(storedGmail);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("User not found.");
                }
                else
                {
                    string hashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                    user.Password = hashPassword;
                    await _userRepository.updateUserPassword(storedGmail, hashPassword);
                    _cache.Remove("GmailKey");
                    check = true;
                    return check;
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Gmail has expired.");
                return check;
            }
        }

        public async Task<bool> disableUserAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not blank");
            }
            var user = await _userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            else
            {
                if (user.Status != "Active")
                {
                    throw new InvalidOperationException("User is already inactive.");
                }
            }
            user.Status = "Inactive";
            return await _userRepository.updateUser(user);
        }




    }
}
