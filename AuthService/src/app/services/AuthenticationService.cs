using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AuthService.Models;
using AuthService.Managers;

namespace AuthService.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IOptions<Settings> _settings;
        private readonly IDBManager _manager;

        public AuthenticationService(IOptions<Settings> settings, IDBManager manager)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        private User GetUser(string name)
        {
            var users = _manager.GetUsers();
            var user = users.FirstOrDefault(u => u.Name == name);

            if (user == null)
            {
                throw new AuthenticationException("User not found.");
            }

            if (!user.Roles.Any())
            {
                throw new AuthenticationException("Roles not found.");
            }

            return user;
        }

        private string ComputeHash(string user, string key, string timeStamp)
        {
            var byteKey = Encoding.UTF8.GetBytes(key);
            using (var hmac = new HMACSHA256(byteKey))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{user}:{timeStamp}"));
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

        private void HashValidate(string user, string key, string timeStamp, string hash)
        {
            var reproduceHash = ComputeHash(user, key, timeStamp);
            if (reproduceHash != hash)
            {
                throw new AuthenticationException("Invalid hash.");
            }
        }

        private void TokenValidate(string token, string key)
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };

                jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            }
            catch
            {
                throw new AuthenticationException("Invalid token.");
            }
        }

        public User BasicAuthenticate(string userName, string password)
        {
            var user = GetUser(userName);
            if (user.Password != password)
            {
                throw new AuthenticationException("Invalid password.");
            }
            return user;
        }

        public User HMACAuthenticate(string userName, string timeStamp, string hash)
        {
            var user = GetUser(userName);
            TimeStampValidate(timeStamp);
            HashValidate(userName, user.Password, timeStamp, hash);
            return user;
        }

        public User BearerAuthenticate(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var userName = jwtSecurityToken?.Claims?.FirstOrDefault(claim => claim.Type == "user")?.Value;
            var user = GetUser(userName);
            TokenValidate(token, user.Password);
            return user;
        }
    }
}

