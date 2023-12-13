using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using MyNutritionist.Utilities;

namespace Tests
{
    public class DatabaseContextHelper
    {
        public static void ExecuteSqlRaw(DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            databaseFacade.ExecuteSqlRaw(sql, parameters);
        }
    }


    [TestClass]
    public class DietPlanControllerTests
    {
        private Mock<ApplicationDbContext> _mockDbContext;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private DietPlanController _controller;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private ApplicationDbContext _context;


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
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);


            _controller = new DietPlanController(_mockDbContext.Object, _mockUserManager.Object, _mockHttpContextAccessor.Object);
            _context = new ApplicationDbContext(options);

        }

        [TestMethod]
        public async Task Index_WhenUserIsNull_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Index_WhenDietPlanIsNull_ReturnsNotFound()
        {
            // Arrange
            var user = new PremiumUser { AspUserId = "1" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
            mockHttpContext.Setup(x => x.Request).Returns(mockHttpRequest.Object);


            // Set up DietPlan DbSet
            _mockDbContext.Setup(db => db.DietPlan)
                .ReturnsDbSet((IEnumerable<DietPlan>)new List<DietPlan>());


            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }


        [TestMethod]
        public async Task Index_WhenUserIsPremium_ReturnsView()
        {
            var user = new PremiumUser { AspUserId = "1" };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var dietPlan = new DietPlan { PremiumUser = user };

            // Set up DietPlan DbSet
            _mockDbContext.Setup(db => db.DietPlan)
                .ReturnsDbSet((IEnumerable<DietPlan>)new List<DietPlan> { dietPlan });

            // Set up Recipe DbSet
            _mockDbContext.Setup(db => db.Recipe)
                .ReturnsDbSet((IEnumerable<Recipe>)new List<Recipe>()); // You can add data as needed

            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }



        [TestMethod]
        public async Task Create_WhenNutritionistNotLoggedIn_ReturnsNotFound()
        {
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser)null);

            _mockDbContext.Setup(x => x.Nutritionist).ReturnsDbSet(new List<Nutritionist>());


            // Arrange
            var dietPlanVm = new EditDietPlanVM { DietPlan = new DietPlan() };

            // Act
            var result = await _controller.Create("userId", dietPlanVm);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task Create_WhenRecipeNotFound_ReturnsRedirect()
        {
            // Arrange
            var loggedInNutritionist = new ApplicationUser { Id = "userId" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(loggedInNutritionist);

            var nutritionist = new Nutritionist { Id = "userId" };
            _mockDbContext.Setup(x => x.Nutritionist).ReturnsDbSet(new List<Nutritionist> { nutritionist });

            var user = new PremiumUser { Id = "premium" };
            _mockDbContext.Setup(db => db.PremiumUser)
                .ReturnsDbSet((IEnumerable<PremiumUser>)new List<PremiumUser> { user });

            var dietPlan = new DietPlan();
            dietPlan.Recipes = null;
            var dietPlanVm = new EditDietPlanVM {};
            dietPlanVm.DietPlan = dietPlan;

            var dietPlans = new List<DietPlan>
            {
                new DietPlan { PremiumUser = new PremiumUser() { Id = "userId1"}}
            };
            _mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet(dietPlans);
            var result = await _controller.Create(user.Id, dietPlanVm);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_WhenDietPlanCreationSuccess_ReturnsRedirectToNutritionistIndex()
        {
            // Arrange
            var nutritionist = new ApplicationUser { Id = "1" };

            DietPlan dp = new DietPlan { DPID = 3, Recipes = new List<Recipe> { new Recipe { RID = 1 } } };

            var dietPlanVm = new EditDietPlanVM
            {
                DietPlan = dp,
            };


            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("1");
            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet((IEnumerable<Nutritionist>)new List<Nutritionist> { new Nutritionist { Id = "1", AspUserId = "1" } });
            _mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet((IEnumerable<DietPlan>)new List<DietPlan> { new DietPlan { PremiumUser = new PremiumUser { Id = "userId" } } });
            _mockDbContext.Setup(db => db.Recipe).ReturnsDbSet((IEnumerable<Recipe>)new List<Recipe> { new Recipe { RID = 1 }, new Recipe { RID = 2 } });
            _mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser> { new PremiumUser { Id = "userId" } });
            
            // Act
            var result = await _controller.Create("userId", dietPlanVm);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Nutritionist", redirectResult.ControllerName);
        }

        [TestMethod]
        public void Create_ReturnsViewWithEditDietPlanVM()
        {
            // Arrange
            var regUser = "userId";
            var recipeList = new List<Recipe>
            {
                new Recipe { RID = 1 },
                new Recipe { RID = 2 },
            };

            _mockDbContext.Setup(db => db.Recipe).ReturnsDbSet(recipeList);

            // Act
            var result = _controller.Create(regUser) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(EditDietPlanVM));

            var model = result.Model as EditDietPlanVM;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.DietPlan);
            Assert.AreEqual(regUser, model.DietPlan.PremiumUser.Id);
            CollectionAssert.AreEqual(recipeList, model.Recipes);
        }
    }
}