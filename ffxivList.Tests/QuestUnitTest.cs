using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ffxivList.Controllers;
using ffxivList.Data;
using ffxivList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ffxivList.Tests
{
    public class QuestUnitTest
    {
        private FfListContext context;

        public QuestUnitTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public async Task Details_CheckReturnedQuest_Pass(int id)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    var questFromResult = await questsController.Details(id);
                    var questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);

                    var viewResult = questFromResult as ViewResult;
                    var model = viewResult.Model;

                    Assert.Equal(model, questFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(14)]
        [InlineData(30)]
        [InlineData(50)]
        [InlineData(70)]
        public async Task Details_CheckReturnedQuest_Fail(int id)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    var questFromResult = await questsController.Details(id);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(questFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
                    Assert.Null(questFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(14, "Test1", 10)]
        [InlineData(11, "Test2", 20)]
        [InlineData(12, "Test3", 30)]
        [InlineData(13, "Test4", 40)]
        public async Task Create_AddQuest_Pass(int id, string name, int level)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                Quest newQuest = new Quest()
                {
                    QuestId = id,
                    QuestName = name,
                    QuestLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    await questsController.Create(newQuest);
                }

                using (var context = new FfListContext(options))
                {
                    Quest questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
                    Assert.NotNull(questFromDb);
                    Assert.Equal(newQuest.QuestId, questFromDb.QuestId);
                    Assert.Equal(newQuest.QuestName, questFromDb.QuestName);
                    Assert.Equal(newQuest.QuestLevel, questFromDb.QuestLevel);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(22, "Test3", 70)]
        [InlineData(23, "Test1234567891012345678920123456789301234567894012345678950", 40)]
        public async Task Create_AddQuest_Fail(int id, string name, int level)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                Quest newQuest = new Quest()
                {
                    QuestId = id,
                    QuestName = name,
                    QuestLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    var questFromResult = await questsController.Create(newQuest);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(questFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
                    Assert.Null(questFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1, "TestQuest1", 20)]
        [InlineData(2, "TestQuest2", 30)]
        [InlineData(3, "TestQuest3", 40)]
        [InlineData(4, "TestQuest4", 50)]
        public async Task Edit_EditQuest_Pass(int id, string name, int level)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                Quest newQuest = new Quest()
                {
                    QuestId = id,
                    QuestName = name,
                    QuestLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    await questsController.Edit(id, newQuest);
                }

                using (var context = new FfListContext(options))
                {
                    Quest questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
                    Assert.NotNull(questFromDb);
                    Assert.Equal(newQuest.QuestId, questFromDb.QuestId);
                    Assert.Equal(newQuest.QuestName, questFromDb.QuestName);
                    Assert.Equal(newQuest.QuestLevel, questFromDb.QuestLevel);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1, "TestQuest10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 19)]
        [InlineData(2, "TestQuest2", -1)]
        [InlineData(3, "TestQuest3", 0)]
        [InlineData(4, "TestQuest4", 70)]
        public async Task Edit_EditQuest_Fail(int id, string name, int level)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                Quest newQuest = new Quest()
                {
                    QuestId = id,
                    QuestName = name,
                    QuestLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    var questFromResult = await questsController.Edit(id, newQuest);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(questFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
                    Assert.Equal(newQuest.QuestId, questFromDb.QuestId);
                    Assert.NotEqual(newQuest.QuestName, questFromDb.QuestName);
                    Assert.NotEqual(newQuest.QuestLevel, questFromDb.QuestLevel);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task Delete_RemoveQuest_Pass(int id)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    await questsController.DeleteConfirmed(id);
                }

                using (var context = new FfListContext(options))
                {
                    Quest questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
                    Assert.Null(questFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(20)]
        [InlineData(30)]
        [InlineData(40)]
        public async Task Delete_RemoveQuest_Fail(int id)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<FfListContext>()
                    .UseSqlite(connection)
                    .Options;

                context = new FfListContext(options);
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);

                using (var context = new FfListContext(options))
                {
                    QuestsController questsController = new QuestsController(context);

                    var questFromResult = await questsController.DeleteConfirmed(id);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(questFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    Quest questFromDb = await context.Quest.SingleOrDefaultAsync(m => m.QuestId == id);
                    Assert.Null(questFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
