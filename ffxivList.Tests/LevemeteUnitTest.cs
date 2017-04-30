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
    public class LevemeteUnitTest
    {
        private FfListContext context;

        public LevemeteUnitTest()
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
        public async Task Details_CheckReturnedLevemete_Pass(int id)
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
                    LevemetesController levemetesController = new LevemetesController(context);

                    var levemeteFromResult = await levemetesController.Details(id);
                    var levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);

                    var viewResult = levemeteFromResult as ViewResult;
                    var model = viewResult.Model;

                    Assert.Equal(model, levemeteFromDb);
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
        public async Task Details_CheckReturnedLevemete_Fail(int id)
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
                    LevemetesController levemetesController = new LevemetesController(context);

                    var levemeteFromResult = await levemetesController.Details(id);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(levemeteFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
                    Assert.Null(levemeteFromDb);
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
        public async Task Create_AddLevemete_Pass(int id, string name, int level)
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

                Levemete newLevemete = new Levemete()
                {
                    LevemeteId = id,
                    LevemeteName = name,
                    LevemeteLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    LevemetesController levemetesController = new LevemetesController(context);

                    await levemetesController.Create(newLevemete);
                }

                using (var context = new FfListContext(options))
                {
                    Levemete levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
                    Assert.NotNull(levemeteFromDb);
                    Assert.Equal(newLevemete.LevemeteId, levemeteFromDb.LevemeteId);
                    Assert.Equal(newLevemete.LevemeteName, levemeteFromDb.LevemeteName);
                    Assert.Equal(newLevemete.LevemeteLevel, levemeteFromDb.LevemeteLevel);
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
        public async Task Create_AddLevemete_Fail(int id, string name, int level)
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

                Levemete newLevemete = new Levemete()
                {
                    LevemeteId = id,
                    LevemeteName = name,
                    LevemeteLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    LevemetesController levemetesController = new LevemetesController(context);

                    var levemeteFromResult = await levemetesController.Create(newLevemete);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(levemeteFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
                    Assert.Null(levemeteFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1, "TestLevemete1", 20)]
        [InlineData(2, "TestLevemete2", 30)]
        [InlineData(3, "TestLevemete3", 40)]
        [InlineData(4, "TestLevemete4", 50)]
        public async Task Edit_EditLevemete_Pass(int id, string name, int level)
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

                Levemete newLevemete = new Levemete()
                {
                    LevemeteId = id,
                    LevemeteName = name,
                    LevemeteLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    LevemetesController levemetesController = new LevemetesController(context);

                    await levemetesController.Edit(id, newLevemete);
                }

                using (var context = new FfListContext(options))
                {
                    Levemete levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
                    Assert.NotNull(levemeteFromDb);
                    Assert.Equal(newLevemete.LevemeteId, levemeteFromDb.LevemeteId);
                    Assert.Equal(newLevemete.LevemeteName, levemeteFromDb.LevemeteName);
                    Assert.Equal(newLevemete.LevemeteLevel, levemeteFromDb.LevemeteLevel);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData(1, "TestLevemete10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", 19)]
        [InlineData(2, "TestLevemete2", -1)]
        [InlineData(3, "TestLevemete3", 0)]
        [InlineData(4, "TestLevemete4", 70)]
        public async Task Edit_EditLevemete_Fail(int id, string name, int level)
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

                Levemete newLevemete = new Levemete()
                {
                    LevemeteId = id,
                    LevemeteName = name,
                    LevemeteLevel = level
                };

                using (var context = new FfListContext(options))
                {
                    LevemetesController levemetesController = new LevemetesController(context);

                    var levemeteFromResult = await levemetesController.Edit(id, newLevemete);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(levemeteFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
                    Assert.Equal(newLevemete.LevemeteId, levemeteFromDb.LevemeteId);
                    Assert.NotEqual(newLevemete.LevemeteName, levemeteFromDb.LevemeteName);
                    Assert.NotEqual(newLevemete.LevemeteLevel, levemeteFromDb.LevemeteLevel);
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
        public async Task Delete_RemoveLevemete_Pass(int id)
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
                    LevemetesController levemetesController = new LevemetesController(context);

                    await levemetesController.DeleteConfirmed(id);
                }

                using (var context = new FfListContext(options))
                {
                    Levemete levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
                    Assert.Null(levemeteFromDb);
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
        public async Task Delete_RemoveLevemete_Fail(int id)
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
                    LevemetesController levemetesController = new LevemetesController(context);

                    var levemeteFromResult = await levemetesController.DeleteConfirmed(id);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(levemeteFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("IndexAdmin", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    Levemete levemeteFromDb = await context.Levemetes.SingleOrDefaultAsync(m => m.LevemeteId == id);
                    Assert.Null(levemeteFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
