using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using XtraUpload.Domain.Infra;

namespace XtraUpload.ServerApp.Common
{
    /// <summary>
    /// Jwt Configuration option
    /// </summary>
    public class JwtIssuerOptions
    {
        /// <summary>
        /// 4.1.1.  "iss" (Issuer) Claim - The "iss" (issuer) claim identifies the principal that issued the JWT.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 4.1.2.  "sub" (Subject) Claim - The "sub" (subject) claim identifies the principal that is the subject of the JWT.
        /// </summary>
        [JsonIgnore]
        public string Subject { get; set; }

        /// <summary>
        /// 4.1.3.  "aud" (Audience) Claim - The "aud" (audience) claim identifies the recipients that the JWT is intended for.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 4.1.4.  "exp" (Expiration Time) Claim - The "exp" (expiration time) claim identifies the expiration time on or after which the JWT MUST NOT be accepted for processing.
        /// </summary>
        [JsonIgnore]
        public DateTime Expiration => IssuedAt.AddDays(ValidFor);

        /// <summary>
        /// 4.1.5.  "nbf" (Not Before) Claim - The "nbf" (not before) claim identifies the time before which the JWT MUST NOT be accepted for processing.
        /// </summary>
        [JsonIgnore]
        public DateTime NotBefore => DateTime.UtcNow;

        /// <summary>
        /// 4.1.6.  "iat" (Issued At) Claim - The "iat" (issued at) claim identifies the time at which the JWT was issued.
        /// </summary>
        [JsonIgnore]
        public DateTime IssuedAt => DateTime.UtcNow;

        /// <summary>
        /// Set the timespan the token will be valid for (default is 1 day)
        /// </summary>
        [Range(1, int.MaxValue)]
        public double ValidFor { get; set; }

        /// <summary>
        /// "jti" (JWT ID) Claim (default ID is a GUID)
        /// </summary>
        [JsonIgnore]
        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());

        /// <summary>
        /// The signing key to use when generating tokens.
        /// </summary>
        [JsonIgnore]
        public SigningCredentials SigningCredentials { get; set; }

        /// <summary>
        /// Secret key to generate the jwt signing key
        /// </summary>
        [Required, StringLength(33)]
        public string SecretKey { get; set; }
    }
}
