using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace XtraUpload.Domain.Infra
{
    public static class Helpers
    {
        /// <summary>
        /// Generate a hashed password combined with a salt
        /// </summary>
        /// <param name="password">The password to hash</param>
        public static string HashPassword(string password)
        {
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);
            // we store the hashed password and the salt in the same column to minimize round trip to db
            return hashedPassword + '$' + salt;
        }

        /// <summary>
        /// Check a plain password against a hashed password, the hashed password must contain the salt
        /// </summary>
        /// <param name="plainPassword">The plain password</param>
        /// <param name="hashedPassword">The hashed password with the salt</param>
        /// <returns>True if they much, false otherwise</returns>
        public static bool CheckPassword (string plainPassword, string hashedPassword)
        {
            string[] passArray = hashedPassword.Split('$');

            if (passArray.Length > 1)
            {
                return HashPassword(plainPassword, passArray[1]) == passArray[0];
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// generate a 128-bit salt using a secure PRNG
        /// </summary>
        /// <returns></returns>
        private static string GenerateSalt ()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private static string HashPassword(string password, string strSalt)
        {
            byte[] salt = Convert.FromBase64String(strSalt);
            byte[] hashed = KeyDerivation.Pbkdf2(
                                password: password,
                                salt: salt,
                                prf: KeyDerivationPrf.HMACSHA1,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hashed);
        }

        /// <summary>
        /// Generate a unique ID (used to display friendly id url)
        /// </summary>
        public static string GenerateUniqueId()
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));
           
            return builder.ToString();
        }

        /// <summary>
        /// Get file extension by it's mime type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static string GetFileExtension(string mimeType)
        {
            return new FileExtensionContentTypeProvider().Mappings.FirstOrDefault(s => s.Value == mimeType).Key;
        }

        /// <summary>
        /// Serialize an object to string
        /// </summary>
        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
        }

        /// <summary>
        /// Gets the user id of the requester
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal caller)
        {
            return caller.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        }

        /// <summary>
        /// Generate a default folder structure for the newly created user
        /// </summary>
        public static IEnumerable<FolderItem> GenerateDefaultFolders(string userId)
        {
            List<FolderItem> folders = new List<FolderItem>()
            {
                new FolderItem()
                {
                    Id = GenerateUniqueId(),
                    Name = "Documents",
                    Parentid = "root",
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                },
                new FolderItem()
                {
                    Id = GenerateUniqueId(),
                    Name = "Images",
                    Parentid = "root",
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                },
                new FolderItem()
                {
                    Id = GenerateUniqueId(),
                    Name = "Videos",
                    Parentid = "root",
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                }
            };

            return folders;
        }
        /// <summary>
        /// Extension method, allows to register config sections as writable
        /// </summary>
        public static void ConfigureWritable<T>(this IServiceCollection services, IConfigurationSection section, string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var environment = provider.GetService<IHostingEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment.ContentRootFileProvider, options, section.Key, file);
            });
        }

    }

    /// <summary>
    /// TimeSpans are not serialized consistently depending on what properties are present. So this 
    /// serializer will ensure the format is maintained no matter what.
    /// </summary>
    public class TimespanConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Format: Days.Hours:Minutes:Seconds:Milliseconds
        /// </summary>
        public const string TimeSpanFormatString = @"d\.hh\:mm\:ss\:FFF";

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            var timespanFormatted = $"{value.ToString(TimeSpanFormatString)}";
            writer.WriteValue(timespanFormatted);
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            TimeSpan.TryParseExact((string)reader.Value, TimeSpanFormatString, null, out TimeSpan parsedTimeSpan);
            return parsedTimeSpan;
        }
    }

}
