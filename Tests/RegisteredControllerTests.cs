using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Castle.Core.Resource;
using MyNutritionist.Utilities;
using Moq.EntityFrameworkCore;

namespace Tests
{
    [TestClass]
    public class RegisteredControllerTests
    {
        private ApplicationDbContext _context;
        private RegisteredUserController _controller;
        private Mock<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ApplicationDbContext> _mockDbContext;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Set up HttpContext User
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "userId")
            }, "mock");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

            _controller = new RegisteredUserController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object, null);
        }


        [TestMethod]
        public async Task Delete_WhenRegisteredUserIsNull_ReturnsNotFound()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            // Act
            var result = await _controller.Delete();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_WhenRegisteredUserIsNotNull_ReturnsView()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            // Act
            var result = await _controller.Delete();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_WhenRegisteredUserIsNull_ReturnsNotFound()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            // Act
            var result = await _controller.Edit();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_WhenRegisteredUserIsNotNull_ReturnsView()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            // Act
            var result = await _controller.Edit();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        // ... (existing code)

        [TestMethod]
        public async Task Edit_WhenRegisteredUserIsNotNull_UpdatesUserAndRedirectsToIndex()
        {
            // Arrange
            var userId = "userId";
            var registeredUserList = new List<RegisteredUser>
            {
                  new RegisteredUser { Id = userId },
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);

            // Act
            var result = await _controller.Edit(new RegisteredUser
            {
                FullName = "New FullName",
                Email = "newemail@example.com",
                NutriUsername = "newNutriUsername",
                NutriPassword = "newPassword",
                Weight = 70,
                Height = 180,
                City = "New City",
                Age = 25
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Verify that the user's information was updated
            var updatedUser = _mockDbContext.Object.RegisteredUser.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("New FullName", updatedUser.FullName);
            Assert.AreEqual("newemail@example.com", updatedUser.Email);
            Assert.AreEqual("newNutriUsername", updatedUser.NutriUsername);
            Assert.AreEqual("newPassword", updatedUser.NutriPassword);
            Assert.AreEqual(70, updatedUser.Weight);
            Assert.AreEqual(180, updatedUser.Height);
            Assert.AreEqual("New City", updatedUser.City);
            Assert.AreEqual(25, updatedUser.Age);
        }

    }

}
