using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ffxivList.Controllers;
using ffxivList.Data;
using ffxivList.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ffxivList.Tests
{
    public class AllUserCraftsUnitTest
    {
        [Theory]
        [InlineData(1, "Distilled Water", 1, "auth0|58fcf8431504b634f54742cb", 1, true)]
        [InlineData(2, "Quicksilver", 1, "auth0|58fcf8431504b634f54742cb", 2, true)]
        [InlineData(3, "Animal Glue", 2, "auth0|58fcf8431504b634f54742cb", 3, true)]
        [InlineData(4, "Growth Formula Alpha", 3, "auth0|58fcf8431504b634f54742cb", 4, true)]
        public async Task IndexTest_SaveChanges(int craftId, string craftName, int craftLevel, string userId, int userCraftId, bool isComplete)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new FfListContext(options))
                {
                    context.Database.EnsureCreated();
                    DbInitializer.Initialize(context);
                }

                using (var context = new FfListContext(options))
                {
                    AllUserCraftsController allUserCraftsController = new AllUserCraftsController(context);

                    await allUserCraftsController.Index(new List<AllUserCraft>
                    {
                        new AllUserCraft()
                        {
                            CraftId = craftId,
                            CraftName = craftName,
                            CraftLevel = craftLevel,
                            UserId = userId,
                            UserCraftId = userCraftId,
                            IsComplete = isComplete
                        }
                    });
                }
                
                using (var context = new FfListContext(options))
                {
                    Assert.Contains(context.AllUserCraft, a => a.UserCraftId == userCraftId && a.IsComplete);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
