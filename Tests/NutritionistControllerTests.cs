using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyNutritionist.Controllers.Tests
{
    [TestClass()]
    public class NutritionistControllerTests
    {
        private ApplicationDbContext _context;
        private NutritionistController _controller;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
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

            _controller = new NutritionistController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object);
        }


        [TestMethod]
        public async Task Index_WhenNutritionistIsNull_ReturnsNotFound()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Index();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Index_ReturnsRightUser()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser()} },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser() } }
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var user = result.Model as Nutritionist;
            Assert.IsNotNull(user);
            Assert.AreEqual("userId", user.Id);
            Assert.AreEqual(3, user.PremiumUsers.Count);
        }

        [TestMethod]
        public async Task Edit_WhenNutritionistIsNull_ReturnsNotFound()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditHttp_WhenNutritionistIsNull_ReturnsNotFound()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditHttp_WhenUpdatedNutritionistIsNull_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(null);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditHttp_WhenNutritionistExists_ReturnsRedirectToAction()
        {
            // Arrange
            var userId = "userId";
            var nutritionistList = new List<Nutritionist>
            {
                  new Nutritionist { Id = userId },
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit(new Nutritionist
            {
                FullName = "New FullName",
                Email = "newemail@example.com",
                NutriUsername = "newNutriUsername",
                Image = "image.jpg"
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Verify that the user's information was updated
            var updatedUser = _mockDbContext.Object.Nutritionist.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("New FullName", updatedUser.FullName);
            Assert.AreEqual("newemail@example.com", updatedUser.Email);
            Assert.AreEqual("newNutriUsername", updatedUser.NutriUsername);
            Assert.AreEqual("image.jpg", updatedUser.Image);
        }

        [TestMethod]
        public async Task EditHttp_WhenNutritionistDataDoesntChange_ReturnsRedirectToAction()
        {
            // Arrange
            var userId = "userId";
            var nutritionistList = new List<Nutritionist>
            {
                  new Nutritionist { Id = userId, FullName ="Nutritionist1", Email="email@example.com", NutriUsername = "nutritionist123", Image="image.jpg"},
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit(new Nutritionist());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Verify that the user's information was updated
            var updatedUser = _mockDbContext.Object.Nutritionist.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("Nutritionist1", updatedUser.FullName);
            Assert.AreEqual("email@example.com", updatedUser.Email);
            Assert.AreEqual("nutritionist123", updatedUser.NutriUsername);
            Assert.AreEqual("image.jpg", updatedUser.Image);
        }
    }

}