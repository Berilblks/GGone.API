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

        public AuthService(GGoneDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
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

            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            var createdUser = _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Data = new RegisterResponse()
            {
                Id = createdUser.Entity.Id
            };

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
                new("UserId", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
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

        public async Task<BaseResponse<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request)
        {
            BaseResponse<ChangePasswordResponse> response = new();

            // Kullanıcıyı ID ile bulma
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

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

            var createdUser = _context.Users.Update(user);
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
    }
}
