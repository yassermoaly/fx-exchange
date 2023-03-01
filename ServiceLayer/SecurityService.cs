using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace ServiceLayer
{
    public class SecurityService : ISecurityService
    {
        private readonly IConfiguration _configuration;
        public SecurityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SecurityKey SecurityKey
        {
            get
            {
                return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityConfig:SecureKey"] ??throw new Exception("SecurityConfig=>SecureKey is not configured")));
            }
        }
        public Dictionary<string,string> LoadSecure(string token, out bool IsExpired)
        {
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["SecurityConfig:TokenIssuer"],

                ValidateAudience = true,
                ValidAudience = _configuration["SecurityConfig:TokenAudience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SecurityKey,

                RequireExpirationTime = true,
                ValidateLifetime = false,

                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityConfig:SecureKey"] ?? throw new Exception("SecurityConfig=>SecureKey is not configured"))),
            };
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal ClaimsPrincipal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken SecurityToken);
            IsExpired = now.CompareTo(SecurityToken.ValidTo) > 0;
            Dictionary<string, string> result = new();
            if (ClaimsPrincipal != null)
            {
                foreach (Claim Claim in ClaimsPrincipal.Claims.ToList())
                {
                    if(Claim.Type.StartsWith("___"))
                    result.Add(Claim.Type[3..], Claim.Value);
                }

            }
            return result;
        }
        public string GenerateSecure(Dictionary<string, string> Claims, int lifetimeInMinutes)
        {
            var claimsIdentity = new ClaimsIdentity();
            foreach (string Key in Claims.Keys)
            {
                claimsIdentity.AddClaim(new Claim($"___{Key}", Claims[Key]));
            }
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptingCredentials = new EncryptingCredentials(SecurityKey, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _configuration["SecurityConfig:TokenIssuer"],
                Audience = _configuration["SecurityConfig:TokenAudience"],
                SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256),
                IssuedAt = now,
                Expires = now.AddMinutes(lifetimeInMinutes),
                EncryptingCredentials = encryptingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return HttpUtility.UrlDecode(tokenHandler.WriteToken(token));
        }

        public string GenerateSecureObj<T>(T Obj, int lifetimeInMinutes)
        {
            Dictionary<string, string> ObjClaims = new();
            if (Obj != null)
            {
                foreach (var Property in Obj.GetType().GetProperties())
                {
                    var PropertyValue = Property.GetValue(Obj)?.ToString();
                    if (!string.IsNullOrEmpty(PropertyValue))
                        ObjClaims.Add(Property.Name, PropertyValue);
                }
                return GenerateSecure(ObjClaims, lifetimeInMinutes);
            }
            return string.Empty;
        }
        public T? LoadSecureObj<T>(string Token, out bool IsExpired)
        {
            var ClaimsDictonary = LoadSecure(Token, out IsExpired);
            if (IsExpired)
                return default;

            var Obj = (T?)Activator.CreateInstance(typeof(T));
            foreach (KeyValuePair<string, string> Claim in ClaimsDictonary)
            {
                var Property = typeof(T).GetProperty(Claim.Key);
                if (Property != null)
                {
                    if(Property.PropertyType==typeof(Guid))
                        Property.SetValue(Obj, Guid.Parse(Claim.Value));
                    else if(Property.PropertyType.IsEnum)
                        Property.SetValue(Obj, Enum.Parse(Property.PropertyType, Claim.Value));
                    
                    else
                        Property.SetValue(Obj, Convert.ChangeType(Claim.Value, Property.PropertyType));                                       
                }
            }
            return Obj;

        }
    }
}
