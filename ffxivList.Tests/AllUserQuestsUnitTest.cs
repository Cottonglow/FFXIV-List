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
    public class AllUserQuestsUnitTest
    {
        [Theory]
        [InlineData(1, "Seer Folly", 30, "auth0|58fcf8431504b634f54742cb", 1, true)]
        [InlineData(2, "Only You Can Prevent Forest Ire", 35, "auth0|58fcf8431504b634f54742cb", 2, true)]
        [InlineData(3, "O Brother, Where Art Thou", 40, "auth0|58fcf8431504b634f54742cb", 3, true)]
        [InlineData(4, "Yearn for the Urn", 45, "auth0|58fcf8431504b634f54742cb", 4, true)]
        public async Task IndexTest_SaveChanges(int questId, string questName, int questLevel, string userId, int userQuestId, bool isComplete)
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
                    AllUserQuestsController allUserQuestsController = new AllUserQuestsController(context);

                    await allUserQuestsController.Index(new List<AllUserQuest>
                    {
                        new AllUserQuest()
                        {
                            QuestId = questId,
                            QuestName = questName,
                            QuestLevel = questLevel,
                            UserId = userId,
                            UserQuestId = userQuestId,
                            IsComplete = isComplete
                        }
                    });
                }
                
                using (var context = new FfListContext(options))
                {
                    Assert.Contains(context.AllUserQuest, a => a.UserQuestId == userQuestId && a.IsComplete);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
