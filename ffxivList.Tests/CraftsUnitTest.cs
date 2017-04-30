using System;
using System.Collections.Generic;
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
    public class CraftsUnitTest
    {
        private FfListContext context;

        public CraftsUnitTest()
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
        public async Task Details_CheckReturnedCraft_Pass(int id)
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
                    CraftsController craftsController = new CraftsController(context);

                    var craftFromResult = await craftsController.Details(id);
                    var craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);

                    var viewResult = craftFromResult as ViewResult;
                    var model = viewResult.Model;

                    Assert.Equal(model, craftFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        [InlineData(50)]
        [InlineData(70)]
        public async Task Details_CheckReturnedCraft_Fail(int id)
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
                    CraftsController craftsController = new CraftsController(context);

                    var craftFromResult = await craftsController.Details(id);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(craftFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);
                    Assert.Null(craftFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(10, "Test1", 10)]
        [InlineData(11, "Test2", 20)]
        [InlineData(12, "Test3", 30)]
        [InlineData(13, "Test4", 40)]
        public async Task Create_AddCraft_Pass(int id, string name, int level)
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

                Craft newCraft = new Craft()
                {
                    CraftId = id,
                    CraftName = name,
                    CraftLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    CraftsController craftsController = new CraftsController(context);

                    await craftsController.Create(newCraft);
                }

                using (var context = new FfListContext(options))
                {
                    Craft craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);
                    Assert.NotNull(craftFromDb);
                    Assert.Equal(newCraft.CraftId, craftFromDb.CraftId);
                    Assert.Equal(newCraft.CraftName, craftFromDb.CraftName);
                    Assert.Equal(newCraft.CraftLevel, craftFromDb.CraftLevel);
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
        public async Task Create_AddCraft_Fail(int id, string name, int level)
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

                Craft newCraft = new Craft()
                {
                    CraftId = id,
                    CraftName = name,
                    CraftLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    CraftsController craftsController = new CraftsController(context);

                    var craftFromResult = await craftsController.Create(newCraft);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(craftFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);
                    Assert.Null(craftFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1, "TestCraft1", 20)]
        [InlineData(2, "TestCraft2", 30)]
        [InlineData(3, "TestCraft3", 40)]
        [InlineData(4, "TestCraft4", 50)]
        public async Task Edit_EditCraft_Pass (int id, string name, int level)
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

                Craft newCraft = new Craft()
                {
                    CraftId = id,
                    CraftName = name,
                    CraftLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    CraftsController craftsController = new CraftsController(context);

                    await craftsController.Edit(id, newCraft);
                }
                
                using (var context = new FfListContext(options))
                {
                    Craft craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);
                    Assert.NotNull(craftFromDb);
                    Assert.Equal(newCraft.CraftId, craftFromDb.CraftId);
                    Assert.Equal(newCraft.CraftName, craftFromDb.CraftName);
                    Assert.Equal(newCraft.CraftLevel, craftFromDb.CraftLevel);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1, "TestCraft10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 20)]
        [InlineData(2, "TestCraft2", -1)]
        [InlineData(3, "TestCraft3", 0)]
        [InlineData(4, "TestCraft4", 70)]
        public async Task Edit_EditCraft_Fail(int id, string name, int level)
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

                Craft newCraft = new Craft()
                {
                    CraftId = id,
                    CraftName = name,
                    CraftLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    CraftsController craftsController = new CraftsController(context);

                    var craftFromResult = await craftsController.Edit(id, newCraft);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(craftFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }
                
                using (var context = new FfListContext(options))
                {
                    var craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);
                    Assert.Equal(newCraft.CraftId, craftFromDb.CraftId);
                    Assert.NotEqual(newCraft.CraftName, craftFromDb.CraftName);
                    Assert.NotEqual(newCraft.CraftLevel, craftFromDb.CraftLevel);
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
        public async Task Delete_RemoveCraft_Pass(int id)
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
                    CraftsController craftsController = new CraftsController(context);

                    await craftsController.DeleteConfirmed(id);
                }
                
                using (var context = new FfListContext(options))
                {
                    Craft craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);
                    Assert.Null(craftFromDb);
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
        public async Task Delete_RemoveCraft_Fail(int id)
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
                    CraftsController craftsController = new CraftsController(context);

                    var craftFromResult = await craftsController.DeleteConfirmed(id);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(craftFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    Craft craftFromDb = await context.Craft.SingleOrDefaultAsync(m => m.CraftId == id);
                    Assert.Null(craftFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
