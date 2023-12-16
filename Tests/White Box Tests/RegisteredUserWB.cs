using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.White_Box_Tests
{
    [TestClass]
    public class RegisteredUserWB
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


            _controller = new RegisteredUserController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object, _mockSignInManager.Object);
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
        public async Task EditCardHttp_WhenRegisteredUserIsNull_ReturnsNotFound()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            // Act
            var result = await _controller.EditCard(new Card { Balance = 60 });

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task EditCardHttp_WhenBalanceIsLessThan50_Redirect()
        {
            var registeredUserList = new List<RegisteredUser>
            {
                new RegisteredUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);


            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            // Act
            var result = await _controller.EditCard(new Card { Balance = 30 });

            Assert.AreEqual("Your card balance has to be 50 or above to finish this transaction.", _controller.TempData["NotificationMessage"]);

            // Check if the action is redirected to the expected action
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public async Task EditCard_WhenRegisteredUserIsNotNull_UpdatesUserAndRedirectsToIndex()
        {
            // Arrange
            var userId = "1";
            var registeredUserList = new List<RegisteredUser>
            {
                  new RegisteredUser {
                      Id = userId,
                      FullName="Test",
                      Age = 21,
                      Points = 2000,
                      AspUserId = "1",
                      UserName = "test",
                      PasswordHash = "hashPassword",
                      NutriPassword = "test123!",
                      NutriUsername = "test",
                      EmailAddress = "test@example.com",
                      Email = "test@example.com",
                      EmailConfirmed = true,
                      Height = 180,
                      Weight = 80,
                      City = "Edinburgh"
                  }
            };
            var premiumUser = new PremiumUser
            {
                Id = userId,
                FullName = "Test",
                Age = 21,
                Points = 2000,
                AspUserId = "1",
                UserName = "test",
                PasswordHash = "hashPassword",
                NutriPassword = "test123!",
                NutriUsername = "test",
                EmailAddress = "test@example.com",
                Email = "test@example.com",
                EmailConfirmed = true,
                Height = 180,
                Weight = 80,
                City = "Edinburgh"
            };
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser {Id = userId + "1", AspUserId = userId + "1"},
                new PremiumUser {Id = userId + "2", AspUserId = userId + "2"},
                new PremiumUser {Id = userId + "3", AspUserId = userId + "3"}
            };
            var cards = new List<Card>();
            var progressList = new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now, BurnedCalories = 1000, ConsumedCalories = 2000, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 2, Date = DateTime.Now.AddDays(-1), BurnedCalories = 1500, ConsumedCalories = 2300, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 3, Date = DateTime.Now.AddDays(-3), BurnedCalories = 400, ConsumedCalories = 2600, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 4, Date = DateTime.Now.AddDays(-4), BurnedCalories = 800, ConsumedCalories = 3000, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 5, Date = DateTime.Now.AddDays(-6), BurnedCalories = 400, ConsumedCalories = 2550, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 6, Date = DateTime.Now.AddDays(-6), BurnedCalories = 400, ConsumedCalories = 2550, RegisteredUser = new RegisteredUser { Id = "123" }, PremiumUser = null}
            };
            var addedProgress = new List<Progress>();
            var mockDbSet = new Mock<DbSet<PremiumUser>>();
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.Provider).Returns(premiumUserList.AsQueryable().Provider);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.Expression).Returns(premiumUserList.AsQueryable().Expression);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.ElementType).Returns(premiumUserList.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.GetEnumerator()).Returns(() => premiumUserList.AsQueryable().GetEnumerator());
            mockDbSet.Setup(x => x.Add(It.IsAny<PremiumUser>()))
                .Callback<PremiumUser>(premiumUser => premiumUserList.Add(premiumUser))
                .Returns((PremiumUser _) => null);
            _mockDbContext.Setup(x => x.PremiumUser).Returns(mockDbSet.Object);

            var mockCardDbSet = new Mock<DbSet<Card>>();
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.Provider).Returns(cards.AsQueryable().Provider);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.Expression).Returns(cards.AsQueryable().Expression);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.ElementType).Returns(cards.AsQueryable().ElementType);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.GetEnumerator()).Returns(() => cards.AsQueryable().GetEnumerator());
            mockCardDbSet.Setup(x => x.Add(It.IsAny<Card>()))
                 .Returns((Card _) => null);
            _mockDbContext.Setup(x => x.Card).Returns(mockCardDbSet.Object);

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(progressList);
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);
            _mockDbContext
                .Setup(db => db.Progress.RemoveRange(It.IsAny<IEnumerable<Progress>>()))
                .Callback<IEnumerable<Progress>>(entities =>
                {
                    var entitiesToRemove = entities.ToList();

                    foreach (var entity in entitiesToRemove)
                    {
                        // Remove items where the RegisteredUser is not null and matches the provided userId
                        if (entity.RegisteredUser != null && entity.RegisteredUser.Id == userId)
                        {
                            progressList.Remove(entity);
                        }
                    }
                });

            _mockDbContext
                .Setup(db => db.Progress.Add(It.IsAny<Progress>()))
                .Callback<Progress>(entity => addedProgress.Add(entity));

            _mockDbContext.Setup(db => db.RegisteredUser.Remove(It.IsAny<RegisteredUser>()))
                .Callback<RegisteredUser>(user => registeredUserList.Remove(user));
            _mockUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<RegisteredUser>(), "RegisteredUser"))
                .ReturnsAsync(IdentityResult.Success);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            _mockDbContext.Setup(x => x.Card).ReturnsDbSet(cards);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<PremiumUser>(), "PremiumUser"))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(premiumUser);
            _mockSignInManager.Setup(s => s.SignOutAsync())
                .Returns(Task.FromResult(0));

            // Act
            var result = await _controller.EditCard(new Card
            {
                Balance = 1000,
                CardNumber = 123456789,
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            // Assert that the user is no longer in the database
            Assert.AreEqual(0, registeredUserList.Count());
            Assert.AreEqual(4, premiumUserList.Count());
            Assert.AreEqual(1, progressList.Count());
            Assert.AreEqual(5, addedProgress.Count());
            Assert.IsTrue(addedProgress.All(x => x.PremiumUser.UserName == premiumUserList[3].UserName));
            Assert.AreEqual("Test", premiumUserList[3].FullName);
            Assert.AreEqual(21, premiumUserList[3].Age);
            Assert.AreEqual(2000, premiumUserList[3].Points);
            Assert.AreEqual("1", premiumUserList[3].AspUserId);
            Assert.AreEqual("test", premiumUserList[3].UserName);
            Assert.AreEqual("hashPassword", premiumUserList[3].PasswordHash);
            Assert.AreEqual("test123!", premiumUserList[3].NutriPassword);
            Assert.AreEqual("test", premiumUserList[3].NutriUsername);
            Assert.AreEqual("test@example.com", premiumUserList[3].EmailAddress);
            Assert.AreEqual(180, premiumUserList[3].Height);
            Assert.AreEqual(80, premiumUserList[3].Weight);
            Assert.AreEqual("Edinburgh", premiumUserList[3].City);

            _mockSignInManager.Verify(m => m.SignOutAsync(), Times.Once);
            _mockUserManager.Verify(m => m.UpdateSecurityStampAsync(It.IsAny<PremiumUser>()), Times.Once);
            _mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<PremiumUser>(), false, null), Times.Once);
        }

        [TestMethod]
        public async Task EditCard_WhenRegisteredUserIsNotNullAndZeroProgress_UpdatesUserAndRedirectsToIndex()
        {
            // Arrange
            var userId = "1";
            var registeredUserList = new List<RegisteredUser>
            {
                  new RegisteredUser {
                      Id = userId,
                      FullName="Test",
                      Age = 21,
                      Points = 2000,
                      AspUserId = "1",
                      UserName = "test",
                      PasswordHash = "hashPassword",
                      NutriPassword = "test123!",
                      NutriUsername = "test",
                      EmailAddress = "test@example.com",
                      Email = "test@example.com",
                      EmailConfirmed = true,
                      Height = 180,
                      Weight = 80,
                      City = "Edinburgh"
                  }
            };
            var premiumUser = new PremiumUser
            {
                Id = userId,
                FullName = "Test",
                Age = 21,
                Points = 2000,
                AspUserId = "1",
                UserName = "test",
                PasswordHash = "hashPassword",
                NutriPassword = "test123!",
                NutriUsername = "test",
                EmailAddress = "test@example.com",
                Email = "test@example.com",
                EmailConfirmed = true,
                Height = 180,
                Weight = 80,
                City = "Edinburgh"
            };
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser {Id = userId + "1", AspUserId = userId + "1"},
                new PremiumUser {Id = userId + "2", AspUserId = userId + "2"},
                new PremiumUser {Id = userId + "3", AspUserId = userId + "3"}
            };
            var cards = new List<Card>();
            var progressList = new List<Progress>
            {
            };
            var addedProgress = new List<Progress>();
            var mockDbSet = new Mock<DbSet<PremiumUser>>();
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.Provider).Returns(premiumUserList.AsQueryable().Provider);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.Expression).Returns(premiumUserList.AsQueryable().Expression);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.ElementType).Returns(premiumUserList.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.GetEnumerator()).Returns(() => premiumUserList.AsQueryable().GetEnumerator());
            mockDbSet.Setup(x => x.Add(It.IsAny<PremiumUser>()))
                .Callback<PremiumUser>(premiumUser => premiumUserList.Add(premiumUser))
                .Returns((PremiumUser _) => null);
            _mockDbContext.Setup(x => x.PremiumUser).Returns(mockDbSet.Object);

            var mockCardDbSet = new Mock<DbSet<Card>>();
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.Provider).Returns(cards.AsQueryable().Provider);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.Expression).Returns(cards.AsQueryable().Expression);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.ElementType).Returns(cards.AsQueryable().ElementType);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.GetEnumerator()).Returns(() => cards.AsQueryable().GetEnumerator());
            mockCardDbSet.Setup(x => x.Add(It.IsAny<Card>()))
                 .Returns((Card _) => null);
            _mockDbContext.Setup(x => x.Card).Returns(mockCardDbSet.Object);

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(progressList);
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);
            _mockDbContext
                .Setup(db => db.Progress.RemoveRange(It.IsAny<IEnumerable<Progress>>()))
                .Callback<IEnumerable<Progress>>(entities =>
                {
                    var entitiesToRemove = entities.ToList();

                    foreach (var entity in entitiesToRemove)
                    {
                        // Remove items where the RegisteredUser is not null and matches the provided userId
                        if (entity.RegisteredUser != null && entity.RegisteredUser.Id == userId)
                        {
                            progressList.Remove(entity);
                        }
                    }
                });

            _mockDbContext
                .Setup(db => db.Progress.Add(It.IsAny<Progress>()))
                .Callback<Progress>(entity => addedProgress.Add(entity));

            _mockDbContext.Setup(db => db.RegisteredUser.Remove(It.IsAny<RegisteredUser>()))
                .Callback<RegisteredUser>(user => registeredUserList.Remove(user));
            _mockUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<RegisteredUser>(), "RegisteredUser"))
                .ReturnsAsync(IdentityResult.Success);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            _mockDbContext.Setup(x => x.Card).ReturnsDbSet(cards);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<PremiumUser>(), "PremiumUser"))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(premiumUser);
            _mockSignInManager.Setup(s => s.SignOutAsync())
                .Returns(Task.FromResult(0));

            // Act
            var result = await _controller.EditCard(new Card
            {
                Balance = 1000,
                CardNumber = 123456789,
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            // Assert that the user is no longer in the database
            Assert.AreEqual(0, registeredUserList.Count());
            Assert.AreEqual(4, premiumUserList.Count());
            Assert.AreEqual(0, progressList.Count());
            Assert.AreEqual(0, addedProgress.Count());
            Assert.IsTrue(addedProgress.All(x => x.PremiumUser.UserName == premiumUserList[3].UserName));
            Assert.AreEqual("Test", premiumUserList[3].FullName);
            Assert.AreEqual(21, premiumUserList[3].Age);
            Assert.AreEqual(2000, premiumUserList[3].Points);
            Assert.AreEqual("1", premiumUserList[3].AspUserId);
            Assert.AreEqual("test", premiumUserList[3].UserName);
            Assert.AreEqual("hashPassword", premiumUserList[3].PasswordHash);
            Assert.AreEqual("test123!", premiumUserList[3].NutriPassword);
            Assert.AreEqual("test", premiumUserList[3].NutriUsername);
            Assert.AreEqual("test@example.com", premiumUserList[3].EmailAddress);
            Assert.AreEqual(180, premiumUserList[3].Height);
            Assert.AreEqual(80, premiumUserList[3].Weight);
            Assert.AreEqual("Edinburgh", premiumUserList[3].City);

            _mockSignInManager.Verify(m => m.SignOutAsync(), Times.Once);
            _mockUserManager.Verify(m => m.UpdateSecurityStampAsync(It.IsAny<PremiumUser>()), Times.Once);
            _mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<PremiumUser>(), false, null), Times.Once);
        }

        [TestMethod]
        public async Task EditCard_WhenRegisteredUserIsNotNullAndOneProgress_UpdatesUserAndRedirectsToIndex()
        {
            // Arrange
            var userId = "1";
            var registeredUserList = new List<RegisteredUser>
            {
                  new RegisteredUser {
                      Id = userId,
                      FullName="Test",
                      Age = 21,
                      Points = 2000,
                      AspUserId = "1",
                      UserName = "test",
                      PasswordHash = "hashPassword",
                      NutriPassword = "test123!",
                      NutriUsername = "test",
                      EmailAddress = "test@example.com",
                      Email = "test@example.com",
                      EmailConfirmed = true,
                      Height = 180,
                      Weight = 80,
                      City = "Edinburgh"
                  }
            };
            var premiumUser = new PremiumUser
            {
                Id = userId,
                FullName = "Test",
                Age = 21,
                Points = 2000,
                AspUserId = "1",
                UserName = "test",
                PasswordHash = "hashPassword",
                NutriPassword = "test123!",
                NutriUsername = "test",
                EmailAddress = "test@example.com",
                Email = "test@example.com",
                EmailConfirmed = true,
                Height = 180,
                Weight = 80,
                City = "Edinburgh"
            };
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser {Id = userId + "1", AspUserId = userId + "1"},
                new PremiumUser {Id = userId + "2", AspUserId = userId + "2"},
                new PremiumUser {Id = userId + "3", AspUserId = userId + "3"}
            };
            var cards = new List<Card>();
            var progressList = new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now, BurnedCalories = 1000, ConsumedCalories = 2000, RegisteredUser = registeredUserList[0], PremiumUser = null},
            };
            var addedProgress = new List<Progress>();
            var mockDbSet = new Mock<DbSet<PremiumUser>>();
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.Provider).Returns(premiumUserList.AsQueryable().Provider);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.Expression).Returns(premiumUserList.AsQueryable().Expression);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.ElementType).Returns(premiumUserList.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<PremiumUser>>().Setup(m => m.GetEnumerator()).Returns(() => premiumUserList.AsQueryable().GetEnumerator());
            mockDbSet.Setup(x => x.Add(It.IsAny<PremiumUser>()))
                .Callback<PremiumUser>(premiumUser => premiumUserList.Add(premiumUser))
                .Returns((PremiumUser _) => null);
            _mockDbContext.Setup(x => x.PremiumUser).Returns(mockDbSet.Object);

            var mockCardDbSet = new Mock<DbSet<Card>>();
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.Provider).Returns(cards.AsQueryable().Provider);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.Expression).Returns(cards.AsQueryable().Expression);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.ElementType).Returns(cards.AsQueryable().ElementType);
            mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.GetEnumerator()).Returns(() => cards.AsQueryable().GetEnumerator());
            mockCardDbSet.Setup(x => x.Add(It.IsAny<Card>()))
                 .Returns((Card _) => null);
            _mockDbContext.Setup(x => x.Card).Returns(mockCardDbSet.Object);

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(progressList);
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);
            _mockDbContext
                .Setup(db => db.Progress.RemoveRange(It.IsAny<IEnumerable<Progress>>()))
                .Callback<IEnumerable<Progress>>(entities =>
                {
                    var entitiesToRemove = entities.ToList();

                    foreach (var entity in entitiesToRemove)
                    {
                        // Remove items where the RegisteredUser is not null and matches the provided userId
                        if (entity.RegisteredUser != null && entity.RegisteredUser.Id == userId)
                        {
                            progressList.Remove(entity);
                        }
                    }
                });

            _mockDbContext
                .Setup(db => db.Progress.Add(It.IsAny<Progress>()))
                .Callback<Progress>(entity => addedProgress.Add(entity));

            _mockDbContext.Setup(db => db.RegisteredUser.Remove(It.IsAny<RegisteredUser>()))
                .Callback<RegisteredUser>(user => registeredUserList.Remove(user));
            _mockUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<RegisteredUser>(), "RegisteredUser"))
                .ReturnsAsync(IdentityResult.Success);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);
            _mockDbContext.Setup(x => x.Card).ReturnsDbSet(cards);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<PremiumUser>(), "PremiumUser"))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(premiumUser);
            _mockSignInManager.Setup(s => s.SignOutAsync())
                .Returns(Task.FromResult(0));

            // Act
            var result = await _controller.EditCard(new Card
            {
                Balance = 1000,
                CardNumber = 123456789,
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            // Assert that the user is no longer in the database
            Assert.AreEqual(0, registeredUserList.Count());
            Assert.AreEqual(4, premiumUserList.Count());
            Assert.IsTrue(addedProgress.All(x => x.PremiumUser.UserName == premiumUserList[3].UserName));
            Assert.AreEqual("Test", premiumUserList[3].FullName);
            Assert.AreEqual(21, premiumUserList[3].Age);
            Assert.AreEqual(2000, premiumUserList[3].Points);
            Assert.AreEqual("1", premiumUserList[3].AspUserId);
            Assert.AreEqual("test", premiumUserList[3].UserName);
            Assert.AreEqual("hashPassword", premiumUserList[3].PasswordHash);
            Assert.AreEqual("test123!", premiumUserList[3].NutriPassword);
            Assert.AreEqual("test", premiumUserList[3].NutriUsername);
            Assert.AreEqual("test@example.com", premiumUserList[3].EmailAddress);
            Assert.AreEqual(180, premiumUserList[3].Height);
            Assert.AreEqual(80, premiumUserList[3].Weight);
            Assert.AreEqual("Edinburgh", premiumUserList[3].City);

            _mockSignInManager.Verify(m => m.SignOutAsync(), Times.Once);
            _mockUserManager.Verify(m => m.UpdateSecurityStampAsync(It.IsAny<PremiumUser>()), Times.Once);
            _mockSignInManager.Verify(m => m.SignInAsync(It.IsAny<PremiumUser>(), false, null), Times.Once);
        }

    }
}

