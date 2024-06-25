using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data.SeedData
{
    public class SeedUserData
    {
        public static async Task SeedUsers(DataContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (!await context.Users.AnyAsync())
                {
                    var userData = await File.ReadAllTextAsync("Data/SeedData/UserSeedData.json");
                    var users = JsonSerializer.Deserialize<List<User>>(userData);
                    if (users == null) { return; }
                    foreach (var item in users)
                    {
                        using var hmac = new HMACSHA512();
                        item.UserName = item.UserName.ToLower();
                       

                    }
                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<SeedUserData>();
                logger.LogError(ex.Message);
            }

        }
    }
}
