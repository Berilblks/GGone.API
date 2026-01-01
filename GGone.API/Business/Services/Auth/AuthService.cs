using AutoMapper;
using GGone.API.Business.Abstracts;
using GGone.API.Data;
using GGone.API.Models;
using GGone.API.Models.Auth;
using GGone.API.Models.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GGone.API.Business.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly GGoneDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(GGoneDbContext context, IConfiguration config, IMapper mapper, IEmailService emailService, ICurrentUserService currentUserService)
        {
            _config = config;
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _currentUserService = currentUserService;
        }

        // LOGIN işlemleri
        public async Task<BaseResponse<LoginResponse>> Login(LoginRequest request)
        {
            BaseResponse<LoginResponse> response = new();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
            {
                response.Error = "User not found.";
                response.ErrorCode = (int)ErrorCode.UserNotFound;
                return response;
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Error = "Wrong password.";
                response.ErrorCode = (int)ErrorCode.WrongPassword;
                return response;
            }

            response.Data = CreateToken(user);

            return response;
        }

        //REGİSTER işlemleri
        public async Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request)
        {
            BaseResponse<RegisterResponse> response = new();

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                response.Error = "Email already exists.";
                response.ErrorCode = (int)ErrorCode.EmailAlreadyExists;
                return response;
            }

            CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

            var user = _mapper.Map<User>(request);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            var createdUser = _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Success = true;

            return response;
            
        }

        //Create Password Hash
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        //Create Token 
        private LoginResponse CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var tokenString =  new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse()
            {
                Token = tokenString,
                Expiration = token.ValidTo
            };
        }

        //Password Hash Doğrulama
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }

        // Change Password
        public async Task<BaseResponse<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request)
        {
            BaseResponse<ChangePasswordResponse> response = new();

            // Kullanıcıyı Email ile bulma
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                response.Error = "User not found.";
                response.ErrorCode = (int)ErrorCode.UserNotFound;
                return response;
            }

            if (!VerifyPasswordHash(request.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                response.Error = "Your current password is incorrect.";
                response.ErrorCode = (int)ErrorCode.PasswordIncorrect;
                return response;
            }

            if (request.OldPassword == request.NewPassword)
            {
                response.Error = "The new password cannot be the same as your current password.";
                response.ErrorCode = (int)ErrorCode.NewPasswordIsSameAsOld;
                return response;
            }

            CreatePasswordHash(request.NewPassword, out byte[] newHash, out byte[] newSalt);

            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
   
            response.Data = new ChangePasswordResponse()
            {
                IsSuccess = true,
                Message = "Your password has been updated successfully."
            };

            return response;
         }

        public async Task<BaseResponse<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordRequest request)
        {
            BaseResponse<ForgetPasswordResponse> response = new();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                response.Error = "User not found.";
                response.ErrorCode = (int)ErrorCode.UserNotFound;
                return response;
            }

            CreatePasswordHash(request.NewPassword, out byte[] newHash, out byte[] newSalt);

            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

            var createdUser = _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = new ForgetPasswordResponse()
            {
                IsSuccess = true,
                Message = "Your password has been updated successfully."
            };

            return response;

        }

        public async Task<BaseResponse<SendVerificationCodeResponse>> SendVerificationCode(SendVerificationCodeRequest request)
        {
            BaseResponse<SendVerificationCodeResponse> response = new();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                // Security: Don't reveal if email exists, just return success or generic message.
                // However, generally for UX we might say "User not found" or handle gracefully.
                // Given the existing pattern in this app returns "User not found", I will stick to it for consistency.
                response.Error = "User not found.";
                response.ErrorCode = (int)ErrorCode.UserNotFound;
                return response;
            }

            // Generate 6 digit code
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            user.ResetPasswordToken = code;
            user.ResetPasswordExpires = DateTime.Now.AddMinutes(15); // Valid for 15 minutes

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Send Email
            var emailBody = $"<h3>Your Verification Code</h3><p>Use the following code to reset your password: <b>{code}</b></p><p>This code is valid for 15 minutes.</p>";
            
            try 
            {
                Console.WriteLine($"[Email Service] Sending verification code to {user.Email}...");
                await _emailService.SendEmailAsync(user.Email, "Reset Password Verification Code", emailBody);
                Console.WriteLine("[Email Service] Email sent successfully.");

                response.Success = true; // Explicitly set success
                response.Data = new SendVerificationCodeResponse
                {
                    IsSuccess = true,
                    Message = "Verification code sent to your email."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Service] ERROR: Failed to send email. Details: {ex.Message}");
                Console.WriteLine($"[Email Service] StackTrace: {ex.StackTrace}");
                
                response.Success = false;
                response.Error = "Failed to send email. Check backend logs for details. Error: " + ex.Message;
                response.ErrorCode = 500;
                // In production, log the exception
            }

            return response;
        }

        public async Task<BaseResponse<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest request)
        {
            BaseResponse<ResetPasswordResponse> response = new();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                response.Error = "User not found.";
                response.ErrorCode = (int)ErrorCode.UserNotFound;
                return response;
            }

            // Verify Code
            if (user.ResetPasswordToken != request.Code)
            {
                response.Error = "Invalid verification code.";
                response.ErrorCode = (int)ErrorCode.WrongPassword; // Or specific error code
                return response;
            }

            // Verify Expiration
            if (user.ResetPasswordExpires < DateTime.Now)
            {
                response.Error = "Verification code expired.";
                response.ErrorCode = (int)ErrorCode.UserNotFound; // Or specific error code
                return response;
            }

            CreatePasswordHash(request.NewPassword, out byte[] newHash, out byte[] newSalt);

            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;
            user.ResetPasswordToken = null; // Clear token after use
            user.ResetPasswordExpires = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = new ResetPasswordResponse
            {
                IsSuccess = true,
                Message = "Your password has been reset successfully."
            };

            return response;
        }
        public async Task<BaseResponse<ProfileResponse>> GetProfile()
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = (int)ErrorCode.UserNotFound
                };
            }

            var profileResponse = _mapper.Map<ProfileResponse>(user);

            return new BaseResponse<ProfileResponse>
            {
                Success = true,
                Data = profileResponse
            };
        }

        public async Task<BaseResponse<ProfileResponse>> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new BaseResponse<ProfileResponse>
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = (int)ErrorCode.UserNotFound
                };
            }

            user.FullName = request.FullName;
            user.Username = request.Username;
            user.BirthDate = new DateOnly(request.BirthYear, request.BirthMonth, request.BirthDay);
            user.Height = request.Height;
            user.Weight = request.Weight;
            user.Gender = request.Gender;
            user.ProfilePhoto = request.ProfilePhoto;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var profileResponse = _mapper.Map<ProfileResponse>(user);

            return new BaseResponse<ProfileResponse>
            {
                Success = true,
                Message = "Profile updated successfully.",
                Data = profileResponse
            };
        }

        public async Task<BaseResponse<string>> RequestDeleteAccount()
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new BaseResponse<string> { Success = false, Error = "User not found.", ErrorCode = (int)ErrorCode.UserNotFound };
            }

            // Generate Code
            var code = new Random().Next(100000, 999999).ToString();
            user.DeleteAccountToken = code;
            user.DeleteAccountExpires = DateTime.Now.AddMinutes(15);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Send Email
            var emailBody = $"<h3>Account Deletion Request</h3><p>Use the following code to PERMANENTLY delete your account: <b>{code}</b></p><p>This code is valid for 15 minutes. If you did not request this, please ignore this email.</p>";
            await _emailService.SendEmailAsync(user.Email, "Delete Account Verification Code", emailBody);

            return new BaseResponse<string> { Success = true, Message = "Verification code sent to your email." };
        }

        public async Task<BaseResponse<string>> ConfirmDeleteAccount(ConfirmDeleteRequest request)
        {
            var userId = _currentUserService.UserId;
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new BaseResponse<string> { Success = false, Error = "User not found.", ErrorCode = (int)ErrorCode.UserNotFound };
            }

            if (user.DeleteAccountToken != request.Code)
            {
                return new BaseResponse<string> { Success = false, Error = "Invalid verification code.", ErrorCode = (int)ErrorCode.WrongPassword };
            }

            if (user.DeleteAccountExpires < DateTime.Now)
            {
                return new BaseResponse<string> { Success = false, Error = "Verification code expired.", ErrorCode = (int)ErrorCode.UserNotFound };
            }

            // --- CASCADE DELETE LOGIC ---
            // Manually delete related data to ensure clean cleanup
            
            // 1. Friendships (UserId or FriendId)
            var friendships = await _context.Friendships.Where(f => f.UserId == userId || f.FriendId == userId).ToListAsync();
            _context.Friendships.RemoveRange(friendships);

            // 2. Addictions
            var addictions = await _context.Addictions.Where(a => a.UserId == userId).ToListAsync();
            _context.Addictions.RemoveRange(addictions);

            // 3. DailyTaskLogs
            var logs = await _context.DailyTaskLogs.Where(l => l.UserId == userId).ToListAsync();
            _context.DailyTaskLogs.RemoveRange(logs);

            // 4. UserHealthRecords
            var records = await _context.UserHealthRecords.Where(r => r.UserId == userId).ToListAsync();
            _context.UserHealthRecords.RemoveRange(records);

            // 5. ChatHistories
            var chats = await _context.ChatHistories.Where(c => c.UserId == userId).ToListAsync();
            _context.ChatHistories.RemoveRange(chats);

            // 6. WeeklyDietPlans
            var plans = await _context.WeeklyDietPlans.Where(p => p.UserId == userId).ToListAsync();
            _context.WeeklyDietPlans.RemoveRange(plans);
            
            // Finally: Delete User
            _context.Users.Remove(user);
            
            await _context.SaveChangesAsync();

            return new BaseResponse<string> { Success = true, Message = "Account deleted successfully." };
        }
    }
}
