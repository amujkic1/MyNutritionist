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
using NuGet.ContentModel;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authentication;
using CsvHelper;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, Mock.Of < IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);


            _controller = new RegisteredUserController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object, _mockSignInManager.Object);
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

        [TestMethod]
        public async Task DeleteConfirmed_WhenValidCredentials_ReturnsRedirectToActionResult()
        {
            // Arrange
            var userId = "userId";
            var username = "validUsername";
            var password = "validPassword";

            var registeredUserList = new List<RegisteredUser>
    {
        new RegisteredUser { Id = userId, UserName = username, NutriPassword = password },
    };

            var progressList = new List<Progress>
    {
        new Progress { PRId = 1, RegisteredUser = registeredUserList.First() },
        new Progress { PRId = 2, RegisteredUser = registeredUserList.First() },
    };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);
            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(progressList);
            _mockDbContext.Setup(db => db.RegisteredUser.Remove(It.IsAny<RegisteredUser>()))
            .Callback<RegisteredUser>(user => registeredUserList.Remove(user));

            _mockSignInManager.Setup(s => s.SignOutAsync())
             .Returns(Task.FromResult(0));

            // Act
            var result = await _controller.DeleteConfirmed(username, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            // You can further assert the properties of the RedirectToActionResult if needed
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);

            // Assert that the user is no longer in the database
            Assert.AreEqual(0,registeredUserList.Count());
        }

        [TestMethod]
        public async Task DeleteConfirmed_WhenDbContextIsNull_ReturnsProblem()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");
            _mockDbContext.Setup(db => db.RegisteredUser).Returns((DbSet<RegisteredUser>)null);

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
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(new List<RegisteredUser>());

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

            var registeredUserList = new List<RegisteredUser>
        {
            new RegisteredUser { Id = userId, UserName = "differentUsername", NutriPassword = "differentPassword" },
        };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);

            // Act
            var result = await _controller.DeleteConfirmed(username, password);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual("Delete", viewResult.ViewName);
            Assert.IsTrue(viewResult.ViewData.ModelState.ContainsKey(""));
            Assert.AreEqual("Invalid username or password", viewResult.ViewData.ModelState[""].Errors.First().ErrorMessage);
        }

        [TestMethod]
        public void DailyFoodAndActivity_ReturnsViewWithModel()
        {
            // Arrange
            var ingredientList = new List<Ingredient>
        {
            new Ingredient { IId = 1, FoodName = "Ingredient1", Calories = 100 },
            new Ingredient { IId = 2, FoodName = "Ingredient2", Calories = 200 },
            // Add more ingredients as needed
        };

            _mockDbContext.Setup(db => db.Ingredient).ReturnsDbSet(ingredientList);

            // Act
            var result = _controller.DailyFoodAndActivity() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(EnterActivityAndFoodViewModel));

            var model = result.Model as EnterActivityAndFoodViewModel;

            // Additional assertions based on your model and data
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Ingredients);
            CollectionAssert.AreEqual(ingredientList, model.Ingredients.ToList());
        }

        public static IEnumerable<object[]> ReadCSV()
        {
            using (var reader = new StreamReader("Data/RegisteredUserControllerCSV.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rows = csv.GetRecords<dynamic>();
                foreach (var row in rows)
                {
                    var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), row.ActivityType, true);

                    yield return new object[] {
                Int32.Parse(row.Breakfast),
                Int32.Parse(row.BreakfastQuantity),
                Int32.Parse(row.Lunch),
                Int32.Parse(row.LunchQuantity),
                Int32.Parse(row.Dinner),
                Int32.Parse(row.DinnerQuantity),
                Int32.Parse(row.Snacks),
                Int32.Parse(row.SnacksQuantity),
                Int32.Parse(row.PAID),
                activityType,
                Int32.Parse(row.Duration),
                Int32.Parse(row.NumberOfPoints),
                Int32.Parse(row.Points)
            };
                }
            }
        }

        static IEnumerable<object[]> RegisteredUserCSV
        {
            get
            {
                return ReadCSV();
            }
        }

        [TestMethod]
        [DynamicData("RegisteredUserCSV")]
        public async Task Save_RegisteredUser(
    int breakfast, int breakfastQuantity, int lunch, int lunchQuantity,
    int dinner, int dinnerQuantity, int snacks, int snacksQuantity,
    int paId, ActivityType activityType, int duration, int numberOfPoints, int Points)
        {
            // Mock the UserManager
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");

            var ingredients = new List<Ingredient>
    {
        new Ingredient { FoodName = "Breakfast", Calories = breakfast },
        new Ingredient { FoodName = "Lunch", Calories = lunch },
        new Ingredient { FoodName = "Dinner", Calories = dinner },
        new Ingredient { FoodName = "Snacks", Calories = snacks }
    };

            _mockDbContext.Setup(c => c.Ingredient).ReturnsDbSet(ingredients);

            // Mock PremiumUser and RegisteredUser lists
            var mockPremiumUsers = new List<PremiumUser> { new PremiumUser { Id = "userId" } };
            var mockRegisteredUsers = new List<RegisteredUser> { new RegisteredUser { Id = "userId", Points=0 } };
            _mockDbContext.Setup(c => c.PremiumUser).ReturnsDbSet(mockPremiumUsers);
            _mockDbContext.Setup(c => c.RegisteredUser).ReturnsDbSet(mockRegisteredUsers);
            _mockHttpContextAccessor.Setup(c => c.HttpContext.User.IsInRole("RegisteredUser")).Returns(true);
           
            var mockProgressSet = new List<Progress>();
            _mockDbContext.Setup(c => c.Progress).ReturnsDbSet(mockProgressSet);
            _mockDbContext
                .Setup(db => db.Progress.Add(It.IsAny<Progress>()))
                .Callback<Progress>(entity => mockProgressSet.Add(entity));


            var model = new EnterActivityAndFoodViewModel
            {
                Breakfast = new Ingredient { FoodName = "Breakfast" },
                Lunch = new Ingredient { FoodName = "Lunch" },
                Dinner = new Ingredient { FoodName = "Dinner" },
                Snacks = new Ingredient { FoodName = "Snacks" },
                BreakfastQuantity = breakfastQuantity,
                LunchQuantity = lunchQuantity,
                DinnerQuantity = dinnerQuantity,
                SnacksQuantity = snacksQuantity,
                PhysicalActivity = new PhysicalActivity
                {
                    PAID = paId,
                    ActivityType = activityType,
                    Duration = duration,
                    NumberOfPoints = numberOfPoints
                }
            };
            // Act
            var result = await _controller.Save(model);
           
            var registeredUserFromDb = mockRegisteredUsers.Find(u => u.Id == "userId");

            Assert.IsNotNull(mockProgressSet);
            Assert.AreEqual(1, mockProgressSet.Count);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);
            
            Assert.AreEqual(Points, registeredUserFromDb.Points);



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
            var result = await _controller.EditCard(new Card { Balance = 60});

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

            //_controller = new RegisteredUserController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object, _mockSignInManager.Object);
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
            var userId = "userId";
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
                  },
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);

            var progressList = new List<Progress>
            {
                new Progress { PRId = 1, Date = DateTime.Now, BurnedCalories = 1000, ConsumedCalories = 2000, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 2, Date = DateTime.Now.AddDays(-1), BurnedCalories = 1500, ConsumedCalories = 2300, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 3, Date = DateTime.Now.AddDays(-3), BurnedCalories = 400, ConsumedCalories = 2600, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 4, Date = DateTime.Now.AddDays(-4), BurnedCalories = 800, ConsumedCalories = 3000, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 5, Date = DateTime.Now.AddDays(-6), BurnedCalories = 400, ConsumedCalories = 2550, RegisteredUser = registeredUserList[0], PremiumUser = null},
                new Progress { PRId = 5, Date = DateTime.Now.AddDays(-6), BurnedCalories = 400, ConsumedCalories = 2550, RegisteredUser = new RegisteredUser { Id = "123" }, PremiumUser = null}
            };

            _mockDbContext.Setup(db => db.Progress).ReturnsDbSet(progressList);
            _mockDbContext.Setup(db => db.Progress.RemoveRange(It.IsAny<List<Progress>>()))
                .Callback<List<Progress>>(progressList => progressList.RemoveAll(x => x.RegisteredUser != null && x.RegisteredUser.Id == userId));
            _mockDbContext.Setup(db => db.RegisteredUser.Remove(It.IsAny<RegisteredUser>()))
                .Callback<RegisteredUser>(user => registeredUserList.Remove(user));
            _mockUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<RegisteredUser>(), "RegisteredUser"))
                .ReturnsAsync(IdentityResult.Success);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.EditCard(new Card
            {
                Balance = 1000,
                CardNumber = 123456789,
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Assert that the user is no longer in the database
            Assert.AreEqual(0, registeredUserList.Count());

            // Verify that the user's information was updated
           /* var updatedUser = _mockDbContext.Object.RegisteredUser.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("New FullName", updatedUser.FullName);
            Assert.AreEqual("newemail@example.com", updatedUser.Email);
            Assert.AreEqual("newNutriUsername", updatedUser.NutriUsername);
            Assert.AreEqual("newPassword", updatedUser.NutriPassword);
            Assert.AreEqual(70, updatedUser.Weight);
            Assert.AreEqual(180, updatedUser.Height);
            Assert.AreEqual("New City", updatedUser.City);
            Assert.AreEqual(25, updatedUser.Age);*/
        }

    }
  


}
