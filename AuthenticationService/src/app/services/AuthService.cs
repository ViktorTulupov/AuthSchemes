using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AuthenticationService.Models;
using AuthenticationService.Managers;
using System.Security.Claims;

namespace AuthenticationService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IOptions<Settings> _settings;
        private readonly IDBManager _manager;

        public AuthService(IOptions<Settings> settings, IDBManager manager)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        private User GetUser(string login)
        {
            var users = _manager.GetUsers();
            var user = users.FirstOrDefault(u => u.Login == login);

            if (user == null)
            {
                throw new AuthenticationException("User not found.");
            }

            if (string.IsNullOrEmpty(user?.Role))
            {
                throw new AuthenticationException("Role not found.");
            }

            return user;
        }

        private string ComputeHash(string login, string password, string timeStamp)
        {
            var byteKey = Encoding.UTF8.GetBytes(password);
            using (var hmac = new HMACSHA256(byteKey))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{login}:{timeStamp}"));
                //var hex = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void TimeStampValidate(string timeStamp)
        {
            var nowSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (long.TryParse(timeStamp, out var stampSeconds))
            {
                if (nowSeconds > stampSeconds && nowSeconds <= stampSeconds + _settings.Value.HashValidSeconds)
                {
                    return;
                }
            }
            throw new AuthenticationException("Invalid timeStamp.");
        }

        private void HashValidate(string login, string password, string timeStamp, string hash)
        {
            var reproduceHash = ComputeHash(login, password, timeStamp);
            if (reproduceHash != hash)
            {
                throw new AuthenticationException("Invalid hash.");
            }
        }

        private void TokenValidate(string token, string password)
        {
            try
            {
                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = false,
                    ValidAudience = _settings.Value.ValidAudience,
                    ValidIssuers = null,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(password))
                };

                jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            }
            catch
            {
                throw new AuthenticationException("Invalid token.");
            }
        }

        public User BasicAuthenticate(string login, string password)
        {
            var user = GetUser(login);
            if (user.Password != password)
            {
                throw new AuthenticationException("Invalid password.");
            }
            return user;
        }

        public User HMACAuthenticate(string login, string timeStamp, string hash)
        {
            var user = GetUser(login);
            TimeStampValidate(timeStamp);
            HashValidate(login, user.Password, timeStamp, hash);
            return user;
        }

        public string HashGenerate(string login)
        {
            var user = GetUser(login);
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var hash = ComputeHash(user.Login, user.Password, timeStamp.ToString());
            return hash;
        }

        public User BearerAuthenticate(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var login = jwtSecurityToken?.Claims?.FirstOrDefault(claim => claim.Type == "user")?.Value;
            var user = GetUser(login);
            TokenValidate(token, user.Password);
            return user;
        }

        public string TokenGenerate(string login)
        {
            var user = GetUser(login);
            var key = Encoding.UTF8.GetBytes(user.Password);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = new JwtSecurityToken(
                claims: new Claim[] { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.Role) },
                audience: _settings.Value.ValidAudience,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddSeconds(_settings.Value.HashValidSeconds),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            return token;
        }
    }
}

