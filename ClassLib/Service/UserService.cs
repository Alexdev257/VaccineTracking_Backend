using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using ClassLib.DTO.User;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ClassLib.DTO.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using static System.Net.WebRequestMethods;
using Google.Apis.Auth;
using Amazon.SimpleNotificationService;
using Amazon;
using Amazon.SimpleNotificationService.Model;
using PayPal.Core;
using PayPal.v1.Orders;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Caching.Distributed;
using ClassLib.DTO.VaccineCombo;
using ClassLib.DTO.Child;
//using Microsoft.AspNetCore.Identity.Data;

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
        private readonly ChildRepository _childRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _redisCache;

        public UserService(UserRepository userRepository, IMapper mapper, JwtHelper jwtHelper, IConfiguration configuration, EmailService emailService, ChildRepository childRepository, IWebHostEnvironment env, IMemoryCache cache, IDistributedCache redisCache)
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
            _childRepository = childRepository;
            _env = env;
            _cache = cache;
            _redisCache = redisCache;
        }


        public async Task<List<GetUserResponse>> getAllService()
        {
            var res = await _userRepository.getAll();
            List<GetUserResponse> list = new List<GetUserResponse>();
            foreach(var listItem in res)
            {
                if (listItem.IsDeleted == false && listItem.Status.ToLower() == "active")
                {
                    var rs = _mapper.Map<GetUserResponse>(listItem);
                    list.Add(rs);
                }
            }
            return list;
        }

        public async Task<List<GetUserResponse>> getAllServiceAdmin()
        {
            var res = await _userRepository.getAll();
            List<GetUserResponse> list = new List<GetUserResponse>();
            foreach(var item in res)
            {
                var rs = _mapper.Map<GetUserResponse>(item);
                list.Add(rs);
            }
            return list;
        }

        public async Task<GetUserResponse?> getUserByIdService(int Id)
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
            if(user.IsDeleted == true)
            {
                throw new UnauthorizedAccessException("User was deleted.");
            }
            else if(user.Status.ToLower() != "active")
            {
                throw new UnauthorizedAccessException("User was inactived.");
            }
            var userRes = _mapper.Map<GetUserResponse>(user);
            return userRes;

        }

        public async Task<GetUserResponse?> getUserByIdServiceAdmin(int Id)
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


        //public async Task<bool> registerAsync(DTO.User.RegisterRequest registerRequest)
        //{
        //    if (string.IsNullOrWhiteSpace(registerRequest.Name) ||
        //        string.IsNullOrWhiteSpace(registerRequest.PhoneNumber) ||
        //        string.IsNullOrWhiteSpace(registerRequest.Username) ||
        //        string.IsNullOrWhiteSpace(registerRequest.Password))
        //    {
        //        throw new Exception("Please fill all information.");
        //    }
        //    var existUsername = await _userRepository.getUserByUsernameAsync(registerRequest.Username);
        //    if (existUsername != null)
        //    {
        //        throw new Exception("Exist username. Please choose another.");
        //    }

        //    var existPhone = await _userRepository.getUserByPhoneAsync(registerRequest.PhoneNumber);
        //    if (existPhone != null)
        //    {
        //        throw new Exception("Exist phone number. Please choose another.");
        //    }

        //    var existGmail = await _userRepository.getUserByGmailAsync(registerRequest.Gmail);
        //    if (existGmail != null)
        //    {
        //        throw new Exception("Exist gmail. Please choose another.");
        //    }
        //    string otp = VerifyCodeHelper.GenerateSixRandomCode();

        //    //_cache.Set($"VerifyCode_{otp}", otp, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterName_{otp}", registerRequest.Name, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterUserName_{otp}", registerRequest.Username, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterGmail_{otp}", registerRequest.Gmail, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterPassword_{otp}", registerRequest.Password, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterPhoneNumber_{otp}", registerRequest.PhoneNumber, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterDateOfBirth_{otp}", registerRequest.DateOfBirth, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterAvatar_{otp}", registerRequest.Avatar, TimeSpan.FromMinutes(5));
        //    //_cache.Set($"RegisterGender_{otp}", registerRequest.Gender, TimeSpan.FromMinutes(5));
        //    var data = new DTO.User.RegisterRequest()
        //    {
        //        Name = registerRequest.Name,
        //        Username = registerRequest.Username,
        //        Gmail = registerRequest.Gmail,
        //        Password = registerRequest.Password,
        //        PhoneNumber = registerRequest.PhoneNumber,
        //        DateOfBirth = registerRequest.DateOfBirth,
        //        Avatar = registerRequest.Avatar,
        //        Gender = registerRequest.Gender,
        //    };

        //    string jsonData = System.Text.Json.JsonSerializer.Serialize(data);
        //    await _redisCache.SetStringAsync($"RegisterRequest_{otp}", jsonData, new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        //    });
        //    /*try
        //    {
        //        string encryptPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
        //        var user = _mapper.Map<User>(registerRequest);
        //        user.Password = encryptPassword;
        //        user.Role = "User";
        //        //user.CreatedAt = DateTime.UtcNow;
        //        user.CreatedAt = Helpers.TimeProvider.GetVietnamNow();
        //        user.Status = "Active";

        //        await _userRepository.addUserAsync(user);
        //        var result = _mapper.Map<RegisterResponse>(user);
        //        return result;
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        throw new Exception($"Error saving data: {ex.InnerException?.Message ?? ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Undefined Error: {ex.Message}");
        //    }*/
        //    string templatePath = Path.Combine(_env.WebRootPath, "templates", "VerifyOtpRegisterTemplate.html");
        //    var placeholders = new Dictionary<string, string>
        //    {
        //        { "UserName", registerRequest.Name},
        //        { "VerifyCode", otp}
        //    };
        //     return await _emailService.sendEmailService(registerRequest.Gmail, "Verify Register", templatePath, placeholders);
        //}

        //public async Task<RegisterResponse?> verifyCodeRegisterAsync(VerifyRegisterRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.Otp))
        //    {
        //        throw new ArgumentNullException("Otp can not be blank");
        //    }
        //    //RegisterResponse result = new RegisterResponse();
        //    //_cache.TryGetValue("GmailKey", out string? storedGmail)
        //    //if (_cache.TryGetValue($"VerifyCode_{request.Otp}", out string? storedOtp) &&
        //    //    _cache.TryGetValue($"RegisterName_{request.Otp}", out string? storedName) &&
        //    //    _cache.TryGetValue($"RegisterUserName_{request.Otp}", out string? storedUserName) &&
        //    //    _cache.TryGetValue($"RegisterGmail_{request.Otp}", out string? storedGmail) &&
        //    //    _cache.TryGetValue($"RegisterPassword_{request.Otp}", out string? storedPassword) &&
        //    //    _cache.TryGetValue($"RegisterPhoneNumber_{request.Otp}", out string? storedPhoneNumber) &&
        //    //    (_cache.TryGetValue($"RegisterDateOfBirth_{request.Otp}", out string? storedDateOfBirth) && DateTime.TryParse(storedDateOfBirth, out DateTime castedDateOfBirth)) &&
        //    //    _cache.TryGetValue($"RegisterAvatar_{request.Otp}", out string? storedAvatar) &&
        //    //    _cache.TryGetValue($"RegisterGender_{request.Otp}", out string? storedGender))
        //    //{

        //    string? jsonData = await _redisCache.GetStringAsync($"RegisterRequest_{request.Otp}");
        //    if (string.IsNullOrEmpty(jsonData))
        //    {
        //        throw new UnauthorizedAccessException("Expired");
        //    }

        //    var registerRequest = System.Text.Json.JsonSerializer.Deserialize<DTO.User.RegisterRequest>(jsonData);
        //    if (registerRequest == null)
        //    {
        //        throw new Exception("Invalid registration data");
        //    }
        //    try
        //    {
        //            //    public string Name { get; set; } = null!;
        //            //public string Username { get; set; } = null!;
        //            //public string Gmail { get; set; } = null!;
        //            //public string Password { get; set; } = null!;
        //            //public string PhoneNumber { get; set; } = null!;
        //            //public DateTime DateOfBirth { get; set; }
        //            //public string Avatar { get; set; } = null!;
        //            //public int Gender { get; set; }
        //            //if(request.Otp == storedOtp)
        //            //{
        //            //    _cache.Remove($"VerifyCode_{request.Otp}");
        //            //    DTO.User.RegisterRequest registerRequest = new DTO.User.RegisterRequest()
        //            //    {
        //            //        Name = storedName,
        //            //        Username = storedUserName,
        //            //        Gmail = storedGmail,
        //            //        Password = storedPassword,
        //            //        PhoneNumber = storedPhoneNumber,
        //            //        DateOfBirth = castedDateOfBirth,
        //            //        Avatar = storedAvatar,
        //            //        Gender = int.Parse(storedGender),
        //            //    };

        //                //_cache.Remove($"RegisterName_{request.Otp}");
        //                //_cache.Remove($"RegisterUserName_{request.Otp}");
        //                //_cache.Remove($"RegisterGmail_{request.Otp}");
        //                //_cache.Remove($"RegisterPassword_{request.Otp}");
        //                //_cache.Remove($"RegisterPhoneNumber_{request.Otp}");
        //                //_cache.Remove($"RegisterDateOfBirth_{request.Otp}");
        //                //_cache.Remove($"RegisterAvatar_{request.Otp}");
        //                //_cache.Remove($"RegisterGender_{request.Otp}");

        //                var user = _mapper.Map<User>(registerRequest);
        //                string encryptPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
        //                user.Password = encryptPassword;
        //                user.Role = "User";

        //                user.CreatedAt = Helpers.TimeProvider.GetVietnamNow();
        //                user.Status = "Active";

        //                await _userRepository.addUserAsync(user);
        //                var result = _mapper.Map<RegisterResponse>(user);

        //        await _redisCache.RemoveAsync($"RegisterRequest_{request.Otp}");
        //        return result;

        //        //}
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        throw new DbUpdateException($"Error saving data: {ex.InnerException?.Message ?? ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Undefined Error: {ex.Message}");
        //    }
        //    //}
        //    //else
        //    //{
        //    //    throw new UnauthorizedAccessException("Expired");
        //    //}
        //    //return result;

        //}

        public async Task<bool> registerAsync(DTO.User.RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Name) ||
                string.IsNullOrWhiteSpace(registerRequest.PhoneNumber) ||
                string.IsNullOrWhiteSpace(registerRequest.Username) ||
                string.IsNullOrWhiteSpace(registerRequest.Password))
            {
                throw new Exception("Please fill all information.");
            }

            var existUsername = await _userRepository.getUserByUsernameAsync(registerRequest.Username);
            if (existUsername != null)
            {
                throw new Exception("Exist username. Please choose another.");
            }

            var existPhone = await _userRepository.getUserByPhoneAsync(registerRequest.PhoneNumber);
            if (existPhone != null)
            {
                throw new Exception("Exist phone number. Please choose another.");
            }

            var existGmail = await _userRepository.getUserByGmailAsync(registerRequest.Gmail);
            if (existGmail != null)
            {
                throw new Exception("Exist gmail. Please choose another.");
            }

            string otp = VerifyCodeHelper.GenerateSixRandomCode();

            var data = new DTO.User.RegisterRequest()
            {
                Name = registerRequest.Name,
                Username = registerRequest.Username,
                Gmail = registerRequest.Gmail,
                Password = registerRequest.Password,
                PhoneNumber = registerRequest.PhoneNumber,
                DateOfBirth = registerRequest.DateOfBirth,
                Avatar = registerRequest.Avatar,
                Gender = registerRequest.Gender,
            };

            // Lưu vào MemoryCache với thời gian hết hạn là 5 phút
            _cache.Set($"RegisterRequest_{otp}", data, TimeSpan.FromMinutes(5));

            // Gửi email xác thực OTP
            string templatePath = Path.Combine(_env.WebRootPath, "templates", "VerifyOtpRegisterTemplate.html");
            var placeholders = new Dictionary<string, string>
            {
                { "UserName", registerRequest.Name },
                { "VerifyCode", otp }
            };

            return await _emailService.sendEmailService(registerRequest.Gmail, "Verify Register", templatePath, placeholders);
        }

        public async Task<RegisterResponse?> verifyCodeRegisterAsync(VerifyRegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Otp))
            {
                throw new ArgumentNullException("Otp can not be blank");
            }

            // Lấy dữ liệu từ MemoryCache
            if (!_cache.TryGetValue($"RegisterRequest_{request.Otp}", out DTO.User.RegisterRequest? registerRequest) || registerRequest == null)
            {
                throw new UnauthorizedAccessException("Expired OTP");
            }

            try
            {
                var user = _mapper.Map<User>(registerRequest);
                string encryptPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
                user.Password = encryptPassword;
                user.Role = "User";
                user.CreatedAt = Helpers.TimeProvider.GetVietnamNow();
                user.Status = "Active";

                await _userRepository.addUserAsync(user);
                var result = _mapper.Map<RegisterResponse>(user);

                // Xóa khỏi MemoryCache sau khi đăng ký thành công
                _cache.Remove($"RegisterRequest_{request.Otp}");

                return result;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException($"Error saving data: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Undefined Error: {ex.Message}");
            }
        }



        public async Task<LoginResponse?> loginAsync(DTO.User.LoginRequest loginRequest)
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
                        var refreshTokenModel = new Models.RefreshToken
                        {
                            Id = long.Parse(tokenId),
                            UserId = user.Id,
                            //AccessToken = "abc not luu",
                            AccessToken = accessToken,
                            RefreshToken1 = refreshToken,
                            IsUsed = false,
                            IsRevoked = false,
                            //IssuedAt = DateTime.UtcNow,
                            IssuedAt = Helpers.TimeProvider.GetVietnamNow(),
                            //ExpiredAt = DateTime.UtcNow.AddDays(1),
                            ExpiredAt = Helpers.TimeProvider.GetVietnamNow().AddDays(1),

                        };
                        await _userRepository.addRefreshToken(refreshTokenModel);
                        return userRes;
                    }
                }
            }
        }

        public async Task<LoginResponse?> loginByGoogleAsync(string googleToken, string clientID)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { $"{clientID}"}
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
            if(payload == null)
            {
                throw new UnauthorizedAccessException("Invalid Google Token");
            }

            var user = await _userRepository.getUserByGmailAsync(payload.Email);
            if(user == null)
            {
                user = new User()
                {
                    Name = payload.Name,
                    Username = payload.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword("123"),
                    Gmail = payload.Email,
                    Avatar = payload.Picture,
                    //CreatedAt = DateTime.UtcNow,
                    CreatedAt = Helpers.TimeProvider.GetVietnamNow(),
                    PhoneNumber = "undefined",
                    //DateOfBirth = DateTime.UtcNow,
                    DateOfBirth = Helpers.TimeProvider.GetVietnamNow(),
                    Gender = 0,
                    Role = "User",
                    Status = "Active",
                    IsDeleted = false,
                };
                await _userRepository.addUserAsync(user);
            }

            var (tokenId, accessToken, refreshToken) = _jwtHelper.generateToken(user);
            var loginRes = _mapper.Map<LoginResponse>(user);
            loginRes.AccessToken = accessToken;
            loginRes.RefreshToken = refreshToken;
            var refreshTokenModel = new Models.RefreshToken
            {
                Id = long.Parse(tokenId),
                UserId = user.Id,
                //AccessToken = "abc not luu",
                AccessToken = accessToken,
                RefreshToken1 = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                //IssuedAt = DateTime.UtcNow,
                IssuedAt = Helpers.TimeProvider.GetVietnamNow(),
                //ExpiredAt = DateTime.UtcNow.AddDays(1),
                ExpiredAt = Helpers.TimeProvider.GetVietnamNow().AddDays(1),

            };
            await _userRepository.addRefreshToken(refreshTokenModel);

            return loginRes;
        }

        public async Task<bool> loginByPhoneAsync(string phoneNumber)
        {
            // bool check = false;
            // if (string.IsNullOrWhiteSpace(phoneNumber))
            // {
            //     throw new ArgumentNullException("Phone number can not be blank");
            //     //return check;
            // }
            // var user = await _userRepository.getUserByPhoneAsync(phoneNumber);
            // if (user == null)
            // {
            //     throw new Exception("Phone number is not exist");
            // }
            // if (!phoneNumber.StartsWith("+"))
            // {
            //     if (phoneNumber.StartsWith("0"))
            //     {
            //         phoneNumber = "+84" + phoneNumber.Substring(1);
            //     }
            //     else
            //     {
            //         throw new Exception("Invalid phone number format");
            //     }
            // }
            // string? awsAccessKey = _configuration["AWS:AccessKey"];
            // string? awsSecretKey = _configuration["AWS:SecretKey"];
            // string? awsRegion = _configuration["AWS:Region"];
            // var otp = VerifyCodeHelper.GenerateSixRandomCode();
            // // luu otp vo cache
            // _cache.Set($"OTPLogin_{otp}", otp, TimeSpan.FromMinutes(5));

            // using var snsClient = new AmazonSimpleNotificationServiceClient(
            //    awsAccessKey,
            //    awsSecretKey,
            //    RegionEndpoint.GetBySystemName(awsRegion)
            //);

            // var request = new PublishRequest
            // {
            //     Message = $"Your OTP for login into Vaccine Tracking System is: {otp}",
            //     PhoneNumber = phoneNumber,
            // };

            // var n = await snsClient.PublishAsync(request);
            // if (n != null)
            // {
            //     check = true;
            // }
            // return check;

            //sms

            //var request = new PublishRequest
            //{
            //    Message = $"Your OTP for login into Vaccine Tracking System is: {otp}",
            //    PhoneNumber = phoneNumber,
            //    MessageAttributes = new Dictionary<string, MessageAttributeValue>
            //    {
            //        {
            //            "AWS.SNS.SMS.SMSType", new MessageAttributeValue
            //            {
            //                StringValue = "Transactional",
            //                DataType = "String"
            //            }
            //        }
            //    }
            //};
            //var res = await snsClient.PublishAsync(request);
            //if (res != null && !string.IsNullOrEmpty(res.MessageId))
            //{
            //    check = true;
            //}
            //return check;
            return false;
        }


        public async Task<LoginResponse?> verifyOtpForLoginAsync(string phoneNumber, string otp)
        {
            var user = await _userRepository.getUserByPhoneAsync(phoneNumber);
            if (user == null)
            {
                throw new ArgumentNullException("Phone number is not exist");
            }
            if(_cache.TryGetValue($"OTPLogin_{otp}", out string? otpCache))
            {
                if(otpCache == otp)
                {
                    _cache.Remove("OTPLogin");
                    var (tokenId, accessToken, refreshToken) = _jwtHelper.generateToken(user);
                    LoginResponse res = new LoginResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                    };
                    var refreshTokenModel = new Models.RefreshToken
                    {
                        Id = long.Parse(tokenId),
                        UserId = user.Id,
                        AccessToken = accessToken,
                        RefreshToken1 = refreshToken,
                        IsUsed = false,
                        IsRevoked = false,
                        //IssuedAt = DateTime.UtcNow,
                        IssuedAt = Helpers.TimeProvider.GetVietnamNow(),
                        //ExpiredAt = DateTime.UtcNow.AddDays(1),
                        ExpiredAt = Helpers.TimeProvider.GetVietnamNow().AddDays(1),

                    };
                    await _userRepository.addRefreshToken(refreshTokenModel);
                    return res;
                }
                else
                {
                    throw new Exception("Wrong OTP. Please try again!");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("OTP has expired!");
            }
        }

        public async Task<LoginResponse?> RefreshTokenService(LoginResponse refreshRequest)
        {
            if (string.IsNullOrEmpty(refreshRequest.AccessToken))
            {
                throw new UnauthorizedAccessException("Access Token is missing.");
            }
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
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                    ValidateLifetime = false, // ko kiem token het han
                };
                //check 1: AccessToken valid format
                var tokenInVerification = tokenHandler.ValidateToken(refreshRequest.AccessToken, tokenValidateParam, out var validatedToken);

                //check 2: check algorithm
                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    jwtSecurityToken.Header.Alg == null ||
                    !jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256, 
                        StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    throw new SecurityTokenInvalidAlgorithmException("Invalid Token Algorithm.");
                }
                //check 3" check accessToken expired
                var utcExpiredToken = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = _jwtHelper.ConvertUnixTimeToDateTime(utcExpiredToken).AddHours(7); // do UTC lệch giờ VN 7 tiếng

                if (expireDate > Helpers.TimeProvider.GetVietnamNow())
                {
                    throw new Exception("Access Token has not yet expired");
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

                if (refreshToken.ExpiredAt < Helpers.TimeProvider.GetVietnamNow())
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
                var refreshTokenModel = new Models.RefreshToken
                {
                    Id = long.Parse(tokenId),
                    UserId = user.Id,
                    //AccessToken = "abc not luu",
                    AccessToken = newAccessToken,
                    RefreshToken1 = newRefreshToken,
                    IsUsed = false,
                    IsRevoked = false,
                    //IssuedAt = DateTime.UtcNow,
                    IssuedAt = Helpers.TimeProvider.GetVietnamNow(),
                    //ExpiredAt = DateTime.UtcNow.AddDays(1),
                    ExpiredAt = Helpers.TimeProvider.GetVietnamNow().AddDays(1),

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
                string.IsNullOrWhiteSpace(request.Avatar) ||
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
            user.Avatar = request.Avatar;
            user.Gmail = request.Gmail;
            user.PhoneNumber = request.PhoneNumber;
            user.IsDeleted = request.isDeleted;
            if (request.isDeleted == false)
            {
                user.Status = "Active";
            }
            else
            {
                user.Status = "Inactive";
            }
            return await _userRepository.updateUser(user);
        }


        public async Task<bool> UpdateUserAdmin(int userId, UpdateUserAdminRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId.ToString()))
            {
                throw new ArgumentNullException("User id can not be blank");
            }
            if(string.IsNullOrWhiteSpace(request.Name) ||
               string.IsNullOrWhiteSpace(request.DateOfBirth.ToString()) ||
               string.IsNullOrWhiteSpace(request.Gender.ToString()) ||
               string.IsNullOrWhiteSpace(request.Avatar) ||
               string.IsNullOrWhiteSpace(request.Gmail) ||
               string.IsNullOrWhiteSpace(request.PhoneNumber) ||
               string.IsNullOrWhiteSpace(request.Status))
            {
                throw new ArgumentNullException("Please fill all the blank");
            }
            var user = await _userRepository.getUserByIdAsync(userId);
            if(user == null)
            {
                throw new ArgumentException("User is not exist");
            }
            user.Name = request.Name;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;
            user.Avatar = request.Avatar;
            user.Gmail = request.Gmail;
            user.PhoneNumber = request.PhoneNumber;
            user.Status = request.Status;
            if(request.Status.ToLower() == "active")
            {
                user.IsDeleted = false;
            }
            else if(request.Status.ToLower() == "inactive")
            {
                user.IsDeleted = true;
            }
            
            List<Child> currentChildren = user.Children.Where(c => !c.IsDeleted).ToList();
            if (request.childIds == null || request.childIds.Count == 0)
            {
                foreach(var child in currentChildren)
                {
                    child.IsDeleted = true;
                    child.Status = "Inactive";
                    await _childRepository.UpdateChild(child);
                }
            }
            else
            {
                List<int> requestChildIds = request.childIds.ToList();
                List<int> existChildIds = currentChildren.Select(c => c.Id).ToList();
                // Tìm những Child cần thêm
                var childIdsToAdd = requestChildIds.Except(existChildIds).ToList();
                List<Child> childrenToAdd = new List<Child>();
                foreach (var childId in childIdsToAdd)
                {
                    var child = await _childRepository.GetChildById(childId);
                    if (child != null)
                    {
                        if (child.ParentId != userId)
                        {
                            throw new UnauthorizedAccessException($"Child with ID {childId} does not belong to this user.");
                        }
                        child.IsDeleted = false;
                        child.Status = "Active";
                        await _childRepository.UpdateChild(child);
                        childrenToAdd.Add(child);
                    }
                }

                foreach (var child in childrenToAdd)
                {
                    user.Children.Add(child);
                }

                // Tìm những Child cần xóa (Soft Delete)
                var childrenToRemove = currentChildren.Where(c => !requestChildIds.Contains(c.Id)).ToList();
                foreach (var child in childrenToRemove)
                {
                    child.IsDeleted = true;
                    child.Status = "Inactive";
                    await _childRepository.UpdateChild(child);
                }
            }

            return await _userRepository.updateUser(user);
        }

        public async Task<GetUserChildResponse?> getUserChildByIdAdmin(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            var user = await _userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("User is not exist");
            }
            if (user.IsDeleted == true)
            {
                throw new UnauthorizedAccessException("User was inactived");
            }
            List<int> childIds = user.Children.Select(user => user.Id).ToList() ?? new List<int>();
            List<GetChildResponse> children = new List<GetChildResponse>();
            foreach (var childId in childIds)
            {
                var child = await _childRepository.GetChildById(childId);
                if (child != null)
                {
                    var rs = new GetChildResponse()
                    {
                        Id = child.Id,
                        ParentId = child.ParentId,
                        Name = child.Name,
                        DateOfBirth = child.DateOfBirth,
                        Gender = child.Gender,
                        Status = child.Status,
                        CreatedAt = child.CreatedAt,
                    };
                    children.Add(rs);
                }

            }
            var response = new GetUserChildResponse()
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                DateOfBirth = user.DateOfBirth,
                gender = user.Gender,
                Gmail = user.Gmail,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Avatar = user.Avatar,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                Children = children
            };

            return response;
        }

        public async Task<List<GetUserChildResponse>> getAllUserChildAdmin()
        {
            var users = await _userRepository.getAll();
            if(users.Count == 0)
            {
                throw new ArgumentException("Do not exist any user");
            }

            List<GetUserChildResponse> res = new List<GetUserChildResponse>();
            foreach (var user in users)
            {
                List<int> childIds = user.Children.Select(user => user.Id).ToList() ?? new List<int>();
                List<GetChildResponse> children = new List<GetChildResponse>();
                foreach (var childId in childIds)
                {
                    var child = await _childRepository.GetChildById(childId);
                    if (child != null)
                    {
                        var rs = new GetChildResponse()
                        {
                            Id = child.Id,
                            ParentId = child.ParentId,
                            Name = child.Name,
                            DateOfBirth = child.DateOfBirth,
                            Gender = child.Gender,
                            Status = child.Status,
                            CreatedAt = child.CreatedAt,
                        };
                        children.Add(rs);
                    }

                }
                var response = new GetUserChildResponse()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.Name,
                    DateOfBirth = user.DateOfBirth,
                    gender = user.Gender,
                    Gmail = user.Gmail,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Avatar = user.Avatar,
                    Status = user.Status,
                    CreatedAt = user.CreatedAt,
                    Children = children
                };
                res.Add(response);
            }
            return res;
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

        public async Task<bool> forgotPasswordAsync(DTO.User.ForgotPasswordRequest request)
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
                    var updated = await _userRepository.updateUser(user);
                    _cache.Remove("GmailKey");
                    check = updated;
                    return check;
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Gmail has expired.");
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

        public async Task<bool> CreateStaff(CreateStaffRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("Name can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                throw new ArgumentNullException("Userame can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentNullException("Password can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Gmail))
            {
                throw new ArgumentNullException("Gmail can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                throw new ArgumentNullException("Phone number can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.DateOfBirth.ToString()))
            {
                throw new ArgumentNullException("Date of birth can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Avatar))
            {
                throw new ArgumentNullException("Avatar can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Gender.ToString()))
            {
                throw new ArgumentNullException("Gender can not be blank");
            }
            var existUserName = await _userRepository.getUserByUsernameAsync(request.Username);
            if(existUserName != null)
            {
                throw new ArgumentException("Username is already exist");
            }
            var existPhone = await _userRepository.getUserByPhoneAsync(request.PhoneNumber);
            if(existPhone != null)
            {
                throw new ArgumentException("Phone Number is already exist");
            }
            var existGmail = await _userRepository.getUserByGmailAsync(request.Gmail);
            if(existGmail != null)
            {
                throw new ArgumentException("Gmail is already exist");
            }
            try
            {
                string encryptPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var user = _mapper.Map<User>(request);
                user.Password = encryptPassword;
                user.Role = "Staff";
                user.CreatedAt = Helpers.TimeProvider.GetVietnamNow();
                user.Status = "Active";

                var result = await _userRepository.addUserAsync(user);
                return result;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error saving data: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Undefined Error: {ex.Message}");
            }

        }

        public async Task<bool> SoftDeleteUser(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            var user = await _userRepository.getUserByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("Do not exist user in the system");
            }
            if(user.IsDeleted == true)
            {
                throw new ArgumentException("User was deleted");
            }
            user.IsDeleted = true;
            user.Status = "Inactive";
            return await _userRepository.updateUser(user);
        }




    }
}
