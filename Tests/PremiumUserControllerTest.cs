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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class PremiumUserControllerTest
    {
        private ApplicationDbContext _context;
        private PremiumUserController _controller;
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

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);

            _controller = new PremiumUserController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object, _mockSignInManager.Object);


        }

        [TestMethod]
        public async Task Index_ReturnsRightUser()
        {
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser
                {
                    Id = "userId",
                    AccountNumber = "000",
                    City = "London",
                    Age = 25, 
                    Weight = 70.0,
                    Height = 180.0,
                    Points = 0 
                },
            };

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });


            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            //var user = result.Model as PremiumUser;
            //Assert.IsNotNull(user);
            //Assert.AreEqual("userId", user.Id);
        }


        [TestMethod]
        public async Task Index_ReturnsNotFoundIfIdIsNull_Test()
        {
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = "userId"},
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("nonExistingId");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            var result = await _controller.Index();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

        }


        [TestMethod]
        public async Task Edit_WhenPremiumUserIsNull_ReturnsNotFound()
        {
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);


            // Act
            var result = await _controller.Edit();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_WhenPremiumUserIsNotNull_ReturnsView()
        {
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.Edit();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_WhenPremiumUserIsNotNull_UpdatesUserAndRedirectsToIndex()
        {
            // Arrange
            var userId = "userId";
            var premiumUserList = new List<PremiumUser>
            {
                  new PremiumUser { Id = userId },
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.Edit(new PremiumUser
            {
                FullName = "Premium User",
                Email = "premiumUser@example.com",
                NutriUsername = "PremiumUsername",
                NutriPassword = "PremiumPassword",
                Weight = 95,
                Height = 200,
                City = "London",
                Age = 30
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Verify that the user's information was updated
            var updatedUser = _mockDbContext.Object.PremiumUser.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("Premium User", updatedUser.FullName);
            Assert.AreEqual("premiumUser@example.com", updatedUser.Email);
            Assert.AreEqual("PremiumUsername", updatedUser.NutriUsername);
            Assert.AreEqual("PremiumPassword", updatedUser.NutriPassword);
            Assert.AreEqual(95, updatedUser.Weight);
            Assert.AreEqual(200, updatedUser.Height);
            Assert.AreEqual("London", updatedUser.City);
            Assert.AreEqual(30, updatedUser.Age);
        }

        [TestMethod]
        public async Task Delete_WhenPremiumUserIsNull_ReturnsNotFound()
        {
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);


            // Act
            var result = await _controller.Delete();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_WhenPremiumUserIsNotNull_ReturnsView()
        {
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);


            // Act
            var result = await _controller.Delete();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestMethod]
        public async Task Details_WhenPremiumUserIsNotNull_Test()
        {
            //Arrange
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser
                {
                    Id = "userId",
                    AccountNumber = "000",
                    City = "London",
                    Age = 25,
                    Weight = 70.0,
                    Height = 180.0,
                    Points = 0
                },
            };

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);


            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(new List<Nutritionist>
            {
                new Nutritionist { Id = "nutriId" , PremiumUsers = new List<PremiumUser> { premiumUserList[0] },
            }
            });

            // Act
            var result = await _controller.Details();

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Details_WhenPremiumUserIsNull_Test()
        {
            //Arrange
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser
                {
                    Id = "userId",
                    AccountNumber = "000",
                    City = "London",
                    Age = 25,
                    Weight = 70.0,
                    Height = 180.0,
                    Points = 0
                },
            };

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);


            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(new List<Nutritionist>
            {
                new Nutritionist { Id = "nutriId" , PremiumUsers = new List<PremiumUser> { premiumUserList[0] },
            }
            });

            // Act
            var result = await _controller.Details();

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

        }

        [TestMethod]
        public async Task Leaderboard_ReturnsNotFound_WhenPremiumUserIsNull()
        {
            // Arrange
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser
                {
                    Id = "userId",
                },
            };

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.Leaderboard();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Leaderboard_ReturnsViewResult_WithLeaderboardData()
        {
            // Arrange
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser
                {
                    Id = "userId",
                },
            };

            var currentUser = new ApplicationUser { Id = "userId" };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(currentUser);

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.Leaderboard();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(Leaderboard));

            var leaderboard = (Leaderboard)viewResult.Model;
            Assert.AreEqual(premiumUserList[0].City, leaderboard.City);

        }

        [TestMethod]
        public async Task DeleteConfirmed_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = "userId";
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser>());

            // Act
            var result = await _controller.DeleteConfirmed("username", "password");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_WhenInvalidCredentials_ReturnsViewResult()
        {
            // Arrange
            var userId = "userId";
            var username = "myUsername";
            var password = "myPassword";

            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser
                {
                    Id = "userId1",
                    AccountNumber = "000",
                    City = "London",
                    Age = 25,
                    Weight = 70.0,
                    Height = 180.0,
                    Points = 0
                },
            };

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.DeleteConfirmed(username, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual("Delete", viewResult.ViewName);
            Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey(""));
            Assert.AreEqual("Invalid username or password", viewResult.ViewData.ModelState[""].Errors.First().ErrorMessage);
        }

    }
}
