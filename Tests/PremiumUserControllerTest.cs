using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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

        public static IEnumerable<object[]> ReadCSV()
        {
            using (var reader = new StreamReader("Data/PremiumUserControllerCSV.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rows = csv.GetRecords<dynamic>();
                foreach (var row in rows)
                {
                    yield return new object[] {
                    row.Id,
                    row.City,
                    int.Parse(row.Age),  
                    double.Parse(row.Weight),
                    double.Parse(row.Height),
                    row.Name,
                    row.Email,
                    row.NutriUsername,
                    row.NutriPassword
                    };
                }
            }
        }

        static IEnumerable<object[]> PremiumUserCSV
        {
            get
            {
                return ReadCSV();
            }
        }

        public static IEnumerable<object[]> ReadXML()
        {
            var xmlDoc = XDocument.Load("Data/PremiumUserControllerXML.xml");
            var testItems = xmlDoc.Descendants("TestItem");

            foreach (var item in testItems)
            {
                yield return new object[]
                {
                    item.Element("UserId").Value,
                    item.Element("City").Value,
                    int.Parse(item.Element("Points").Value)
                };
            }

        }

        static IEnumerable<object[]> PremiumUserXML
        {
            get
            {
                return ReadXML();
            }
        }


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

			var nutritionTips = new List<NutritionTipsAndQuotes>
			{
				new NutritionTipsAndQuotes
				{
					NTAQId= 1,
					QuoteText="abcdefghijk",
				},
				new NutritionTipsAndQuotes
				{
					NTAQId = 2,
					QuoteText="ijklljmnsjnvc",
				}
			};

			_mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });


            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

			_mockDbContext.Setup(db => db.NutritionTipsAndQuotes).ReturnsDbSet(nutritionTips);

			// Act
			//var result = await _controller.Index() as ViewResult;

			// Assert
			//Assert.IsNotNull(result);
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

            //var result = await _controller.Index();

            //Assert.IsInstanceOfType(result, typeof(NotFoundResult));

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
        public async Task EditHttp_WhenPremiumUserIsNull_ReturnsNotFound()
        {
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.Edit(new PremiumUser());

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
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
        public async Task DeleteConfirmed_WhenValidCredentials_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userId = "userId";
            var username = "validUsername";
            var password = "validPassword";

            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = userId, UserName = username, NutriPassword = password },
            };

            var progressList = new List<Progress>
            {
                new Progress { PRId = 1, PremiumUser = premiumUserList[0] },
                new Progress { PRId = 2, PremiumUser = premiumUserList[0] },
            };

            var dietPlanList = new List<DietPlan>
            {
                new DietPlan { DPID = 1, TotalCalories = 2000, PremiumUser = premiumUserList[0], Recipes = new List<Recipe>() },
            };

            var cardList = new List<Card>
            {
                new Card { CId = 1, CardNumber = 12345, PremiumUserId = userId, PremiumUser = premiumUserList[0], Balance = 1100.5 },
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);
            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(progressList);
            _mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet(dietPlanList);
            _mockDbContext.Setup(db => db.Card).ReturnsDbSet(cardList);
            _mockDbContext.Setup(db => db.PremiumUser.Remove(It.IsAny<PremiumUser>()))
            .Callback<PremiumUser>(user => premiumUserList.Remove(user));

            _mockSignInManager.Setup(s => s.SignOutAsync())
             .Returns(Task.FromResult(0));

            // Act
            var result = await _controller.DeleteConfirmed(username, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);

            // Assert that the user is no longer in the database
            Assert.AreEqual(0, premiumUserList.Count());
        }

        [TestMethod]
        public async Task DeleteConfirmed_WhenDbContextIsNull_ReturnsProblem()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");
            _mockDbContext.Setup(db => db.PremiumUser).Returns((DbSet<PremiumUser>)null);

            // Act
            var result = await _controller.DeleteConfirmed("username", "password");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(500, objectResult.StatusCode);

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
            var username = "validUsername";
            var password = "validPassword";

            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser { Id = userId, UserName = "differentUsername", NutriPassword = "differentPassword" },
            };

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

        [DynamicData("PremiumUserCSV")]
        [TestMethod]
        public async Task EditHttp_WhenPremiumUserExists_ReturnsRedirectToAction(string userId, string city, int age, double weight, double height, string name, string email, string nutriUsername, string nutriPassword)
        {
            // Arrange
            var premiumUserList = new List<PremiumUser>
            {
                  new PremiumUser { Id = userId, FullName = name, Email = email, NutriUsername = nutriUsername, City = city, Age = age, Weight = weight, Height = height},
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);

            // Act
            var result = await _controller.Edit(new PremiumUser());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Verify that the user's information was updated
            var updatedUser = _mockDbContext.Object.PremiumUser.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(name, updatedUser.FullName);
            Assert.AreEqual(email, updatedUser.Email);
            Assert.AreEqual(nutriUsername, updatedUser.NutriUsername);
            Assert.AreEqual(city, updatedUser.City);
            Assert.AreEqual(age, updatedUser.Age);
            Assert.AreEqual(weight, updatedUser.Weight);
            Assert.AreEqual(height, updatedUser.Height);

        }


        [TestMethod]
        [DynamicData("PremiumUserXML")]
        public async Task Leaderboard_ShouldUpdateLeaderboardData(string userId, string city, int points)
        {
            // Arrange
            var premiumUserList = new List<PremiumUser>
            {
                new PremiumUser
                {
                    Id = userId, City = city, Points = points
                },
            };

            var currentUser = new ApplicationUser { Id = userId };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(currentUser);

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now.AddDays(-6), ConsumedCalories = 1500, BurnedCalories = 200, PremiumUser = premiumUserList[0] },
            });

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(premiumUserList);
            // Act
            var result = await _controller.Leaderboard();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult));

            var viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(Leaderboard));

            var leaderboard = (Leaderboard)viewResult.Model;
            Assert.AreEqual(premiumUserList[0].City, leaderboard.City);

        }
    }

}