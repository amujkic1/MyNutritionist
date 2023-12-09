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
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            // Mock SignInManager
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                new HttpContextAccessor(),
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null);

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

        [TestMethod]
        public async Task EditCard_WhenRegisteredUserIsNull_ReturnsNotFound()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            // Act
            var result = await _controller.EditCard();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditCard_WhenRegisteredUserIsNotNull_ReturnsView()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            // Act
            var result = await _controller.EditCard();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void NutritionalValues_ReturnsViewWithIngredients()
        {
            // Arrange
            var ingredientList = new List<Ingredient>
    {
        new Ingredient { IId = 1, FoodName = "Ingredient1", Calories = 100, Carbs = 20, Protein = 10, Sugar = 5, Fat = 8, SaturatedFat = 3, VitaminA = 50, VitaminC = 30, Calcium = 15, Iron = 2, Sodium = 100 },
        new Ingredient { IId = 2, FoodName = "Ingredient2", Calories = 150, Carbs = 30, Protein = 15, Sugar = 7, Fat = 10, SaturatedFat = 4, VitaminA = 70, VitaminC = 40, Calcium = 20, Iron = 3, Sodium = 120 },
        // Add more ingredients...
    };

            _mockDbContext.Setup(db => db.Ingredient).ReturnsDbSet(ingredientList);

            // Act
            var result = _controller.NutritionalValues();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            if (result is ViewResult viewResult)
            {
                // Assert that the model passed to the view is a list of ingredients
                Assert.IsInstanceOfType(viewResult.Model, typeof(List<Ingredient>));

                var model = viewResult.Model as List<Ingredient>;

                // Add more specific assertions based on your expectations
                Assert.AreEqual(ingredientList.Count, model.Count);

                // Example: Assert that the first ingredient's name is as expected
                Assert.AreEqual("Ingredient1", model[0].FoodName);
            }
           
        }


    }

}
