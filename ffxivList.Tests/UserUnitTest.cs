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
    public class UserUnitTest
    {
        private FfListContext context;

        public UserUnitTest()
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
        [InlineData("auth0|58fcf8431504b634f54742cb")]
        [InlineData("auth0|5904ebbf5d4f881ecedb18ad")]
        public async Task Details_CheckReturnedUser_Pass(string id)
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
                    UsersController usersController = new UsersController(context);

                    var userFromResult = await usersController.Details(id);
                    var userFromDb = await context.Users.SingleOrDefaultAsync(m => m.UserId == id);

                    var viewResult = userFromResult as ViewResult;
                    var model = viewResult.Model;

                    Assert.Equal(model, userFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory]
        [InlineData("UserID1")]
        [InlineData("UserID2")]
        public async Task Details_CheckReturnedUser_Fail(string id)
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
                    UsersController usersController = new UsersController(context);

                    var userFromResult = await usersController.Details(id);

                    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(userFromResult);
                    Assert.Null(redirectToActionResult.ControllerName);
                    Assert.Equal("Index", redirectToActionResult.ActionName);
                }

                using (var context = new FfListContext(options))
                {
                    var userFromDb = await context.Users.SingleOrDefaultAsync(m => m.UserId == id);
                    Assert.Null(userFromDb);
                }
            }
            finally
            {
                connection.Close();
            }
        }
        
        [Theory]
        [InlineData("auth0|58fcf8431504b634f54742cb", "User", "admin2", 0, 0, 0)]
        [InlineData("auth0|5904ebbf5d4f881ecedb18ad", "Admin", "user", 0, 0, 0)]
        public async Task Edit_EditUser_Pass(string id, string role, string username, int questComplete, int leveComplete, int craftComplete)
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

                User newUser = new User()
                {
                    UserId = id,
                    UserName = username,
                    UserQuestsCompleted = questComplete,
                    UserLevemetesCompleted = leveComplete,
                    UserCraftsCompleted = craftComplete,
                    UserRole = role
                };

                using (var context = new FfListContext(options))
                {
                    UsersController usersController = new UsersController(context);

                    await usersController.Edit(id, newUser);
                }

                using (var context = new FfListContext(options))
                {
                    User userFromDb = await context.Users.SingleOrDefaultAsync(m => m.UserId == id);
                    Assert.NotNull(userFromDb);
                    Assert.Equal(newUser.UserId, userFromDb.UserId);
                    Assert.Equal(newUser.UserName, userFromDb.UserName);
                    Assert.Equal(newUser.UserRole, userFromDb.UserRole);
                    Assert.Equal(newUser.UserQuestsCompleted, userFromDb.UserQuestsCompleted);
                    Assert.Equal(newUser.UserCraftsCompleted, userFromDb.UserCraftsCompleted);
                    Assert.Equal(newUser.UserLevemetesCompleted, userFromDb.UserLevemetesCompleted);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
