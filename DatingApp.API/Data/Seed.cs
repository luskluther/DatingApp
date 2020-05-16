using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        // you deserialize a json into user objects to match models ( usermodel class )

        public static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any()) {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json"); // we are reading the text from the data file
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach(var user in users)
                {   
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password",out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.UserName = user.UserName.ToLower();
                    context.Users.Add(user);
                }
                context.SaveChanges();
            }
        }

        // all methods need to be static because we made the first one static
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Easy technique straightforward
            // Using for disposal
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}