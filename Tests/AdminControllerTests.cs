using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Tests
{
    [TestClass]
    public class AdminControllerTests
    {
        private AdminController _controller;
        private ApplicationDbContext _context;
        private Mock<ApplicationDbContext> _mockDbContext;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null);
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

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _controller = new AdminController(_mockDbContext.Object, _mockUserManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            _context = new ApplicationDbContext(options);
        }

        [TestMethod]
        public async Task Index_ReturnsViewWithPremiumUsers()
        {
            // Arrange

            var user = new Admin { Id = "1" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var premiumUsers = new List<PremiumUser>
            {
                new PremiumUser { UserName = "user1" },
                new PremiumUser { UserName = "user2" }
            };

            _mockDbContext.Setup(x => x.PremiumUser).ReturnsDbSet((IEnumerable<PremiumUser>)premiumUsers);

            // Act
            var result = await _controller.Index();

            //Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<PremiumUser>));
            var model = viewResult.Model as List<PremiumUser>;
            Assert.AreEqual(premiumUsers.Count, model.Count);
        }

        [TestMethod]
        public async Task AssignNutritionist_ReturnsViewWithNutritionists()
        {
            // Arrange
            var user = new Admin { Id = "1" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Set up a non-null list of Nutritionist entities
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser()} },
                new Nutritionist { Id = "userId1" , PremiumUsers = new List<PremiumUser> { new PremiumUser() } },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser() } },
                new Nutritionist { Id = "userId3" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser(), new PremiumUser() } }
            };

            var premiumList = new List<PremiumUser>
            {
                new PremiumUser {UserName = "premiumUserName"}
            };

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumList);
            _mockUserManager.Setup(db => db.GetUsersInRoleAsync("Nutritionist")).ReturnsAsync(nutritionistList.ToArray());


            // Act
            var result = await _controller.AssignNutritionist("premiumUserName");

            // Assert
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<ApplicationUser>));

            var model = (IEnumerable<ApplicationUser>)viewResult.Model;
            Assert.IsNotNull(model);
            Assert.AreEqual(nutritionistList.Count, model.Count());

        }


        [TestMethod]
        public async Task UpgradeToPremium_WithValidData_RedirectsToIndex()
        {
            // Arrange

            var nutriUsername = "nutritionistUser";
            var premiumUsername = "user1";
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { NutriUsername = nutriUsername }
            };

            var premiumList = new List<PremiumUser>
            {
                new PremiumUser {UserName = premiumUsername}
            };

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumList);

            // Act
            var result = await _controller.UpgradeToPremium(nutriUsername, premiumUsername);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);

        }
    

        [TestMethod]
        public async Task UpgradeToPremium_WithEmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            string nutriUserName = "validNutriUserName";
            string premiumUserName = string.Empty;

            // Act
            var result = await _controller.UpgradeToPremium(nutriUserName, premiumUserName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.AreEqual("Invalid name.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UpgradeToPremium_WithEmptyNutriUsername_ReturnsBadRequest()
        {
            // Arrange
            string nutriUserName = string.Empty; 
            string premiumUserName = "validNutriUserName";

            // Act
            var result = await _controller.UpgradeToPremium(nutriUserName, premiumUserName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.AreEqual("Invalid name.", badRequestResult.Value);

        }

        

    }
}
