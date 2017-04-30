using System.Collections.Generic;
using System.Threading.Tasks;
using ffxivList.Controllers;
using ffxivList.Data;
using ffxivList.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ffxivList.Tests
{
    public class AllUserLevemetesUnitTest
    {
        [Theory]
        [InlineData(1, "Throw the Book at Him", 20, "auth0|58fcf8431504b634f54742cb", 1, true)]
        [InlineData(2, "Victory Is Mine, Not Yours", 20, "auth0|58fcf8431504b634f54742cb", 2, true)]
        [InlineData(3, "The Third Prize Is That You're Slain", 25, "auth0|58fcf8431504b634f54742cb", 3, true)]
        [InlineData(4, "Bridges of Qiqirn Country", 30, "auth0|58fcf8431504b634f54742cb", 4, true)]
        public async Task IndexTest_SaveChanges(int levemeteId, string levemeteName, int levemeteLevel, string userId, int userLevemeteId, bool isComplete)
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
                    AllUserLevemetesController allUserLevemetesController = new AllUserLevemetesController(context);

                    await allUserLevemetesController.Index(new List<AllUserLevemete>
                    {
                        new AllUserLevemete()
                        {
                            LevemeteId = levemeteId,
                            LevemeteName = levemeteName,
                            LevemeteLevel = levemeteLevel,
                            UserId = userId,
                            UserLevemeteId = userLevemeteId,
                            IsComplete = isComplete
                        }
                    });
                }
                
                using (var context = new FfListContext(options))
                {
                    Assert.Contains(context.AllUserLevemete, a => a.UserLevemeteId == userLevemeteId && a.IsComplete);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
