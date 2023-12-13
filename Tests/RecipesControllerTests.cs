using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Xml;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using MyNutritionist.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using CsvHelper;
using System.Globalization;

namespace Tests
{
    [TestClass]
    public class RecipesControllerTests
    {
        private  ApplicationDbContext _context;
        private RecipesController _controller;
        private  UserManager<ApplicationUser> _userManager;
        private Mock<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ApplicationDbContext> _mockDbContext;
        public static IEnumerable<object[]> ReadRecipesXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Data/RecipesControllerXML.xml");

            foreach (XmlNode recipeNode in doc.DocumentElement.ChildNodes)
            {
                string name = recipeNode.SelectSingleNode("Name")?.InnerText.Trim();
                string recipeLink = recipeNode.SelectSingleNode("RecipeLink")?.InnerText.Trim();
                int calories;
                if (int.TryParse(recipeNode.SelectSingleNode("Calories")?.InnerText.Trim(), out calories))
                {
                    yield return new object[]
                    {
                name,
                recipeLink,
                calories
                    };
                }
            }
        }

        public static IEnumerable<object[]> RecipesXML
        {
            get
            {
                return ReadRecipesXML();
            }
        }

        public static IEnumerable<object[]> ReadRecipesCSV()
        {
            using (var reader = new StreamReader("Data/RecipesController.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rows = csv.GetRecords<dynamic>();
                foreach (var row in rows)
                {
                    yield return new object[] {
                row.Name,
                row.RecipeLink,
                Convert.ToInt32(row.Calories)
            };
                }
            }
        }

        public static IEnumerable<object[]> RecipesCSV
        {
            get
            {
                return ReadRecipesCSV();
            }
        }



        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
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

            _controller = new RecipesController(_mockDbContext.Object, _mockUserManager.Object);
            _context = new ApplicationDbContext(options);

        }





        [TestMethod]
        public void Create_Get_ReturnsViewResult()
        {
            // Arrange, using the controller instance set up in the test
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task Create_Post_ValidRecipe_RedirectsToIndex()
        {
            // Arrange
            var loggedInNutritionist = new ApplicationUser { Id = "userId" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(loggedInNutritionist);

            var nutritionist = new Nutritionist { Id = "userId" };
            _mockDbContext.Setup(x => x.Nutritionist.FindAsync("userId")).ReturnsAsync(nutritionist);

            var recipeViewModel = new RecipeViewModel
            {
                input = new Recipe{NameOfRecipe = "Test Recipe",RecipeLink = "https://www.spendwithpennies.com/classic-chicken-salad-recipe/",TotalCalories = 500}
            };

            // Act
            var result = await _controller.Create(recipeViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        public async Task Create_Post_NutritionistNotFound_ReturnsNotFound()
        {
            // Arrange
            var loggedInNutritionist = new ApplicationUser { Id = "userId" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(loggedInNutritionist);

            // Nutritionist not found in the database
            _mockDbContext.Setup(x => x.Nutritionist.FindAsync("userId")).ReturnsAsync((Nutritionist)null);

            var recipeViewModel = new RecipeViewModel
            {
                input = new Recipe{ NameOfRecipe = "Test Recipe", RecipeLink = "https://example.com",TotalCalories = 500}
            };

            // Act
            var result = await _controller.Create(recipeViewModel) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Nutritionist not found.", result.Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }
        [TestMethod]
        public async Task Create_Get_ContextAndRecipeNotNull_ReturnsViewResult()
        {
            // Arrange
            var recipeViewModel = new RecipeViewModel();
            var recipesList = new List<Recipe>
            {
                new Recipe {  NameOfRecipe = "Recipe 1", RecipeLink = "Link 1", TotalCalories = 300 },
                new Recipe {  NameOfRecipe = "Recipe 2", RecipeLink = "Link 2", TotalCalories = 500 }
            };

            _mockDbContext.Setup(x => x.Recipe).Returns(MockDbSet(recipesList));

            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);

            var resultModel = result.Model as RecipeViewModel;
            Assert.IsNotNull(resultModel);

            CollectionAssert.AreEqual(recipesList, resultModel.recipesToDisplay.ToList());
        }

        [TestMethod]
        public async Task Create_Get_ContextOrRecipeNull_ReturnsViewResultWithEmptyList()
        {
            // Arrange
            var recipeViewModel = new RecipeViewModel();
            _mockDbContext.Setup(x => x.Recipe).Returns(MockDbSet<Recipe>(null));

            // Act
            var result =_controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);

            var resultModel = result.Model as RecipeViewModel;
            Assert.IsNotNull(resultModel);

            CollectionAssert.AreEqual(new List<Recipe>(), resultModel.recipesToDisplay.ToList());
        }

        // Helper method for creating a mock DbSet
        private static DbSet<T> MockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data?.AsQueryable() ?? Enumerable.Empty<T>().AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet.Object;
        }
        [TestMethod]
        [DataRow("Test Recipe 1", "https://www.example.com/recipe1", 500)]
        [DataRow("Test Recipe 2", "https://www.example.com/recipe2", 600)]
        [DataRow("Test Recipe 3", "https://www.example.com/recipe3", 700)]
        public async Task Create_Post_ValidRecipe_RedirectsToIndex_Data(string name, string recipeLink, int calories)
        {
            // Arrange
            var loggedInNutritionist = new ApplicationUser { Id = "userId" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(loggedInNutritionist);

            var nutritionist = new Nutritionist { Id = "userId" };
            _mockDbContext.Setup(x => x.Nutritionist.FindAsync("userId")).ReturnsAsync(nutritionist);

            var recipeViewModel = new RecipeViewModel
            {
                input = new Recipe
                {
                    NameOfRecipe = name,
                    RecipeLink = recipeLink,
                    TotalCalories = calories
                }
            };

            // Act
            var result = await _controller.Create(recipeViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        [DynamicData("RecipesXML")]
        public async Task Create_Post_ValidRecipe_RedirectsToIndex_DynamicData(string name, string recipeLink, int calories)
        {
            // Arrange
            var loggedInNutritionist = new ApplicationUser { Id = "userId" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(loggedInNutritionist);

            var nutritionist = new Nutritionist { Id = "userId" };
            _mockDbContext.Setup(x => x.Nutritionist.FindAsync("userId")).ReturnsAsync(nutritionist);

            var recipeViewModel = new RecipeViewModel
            {
                input = new Recipe{NameOfRecipe = name, RecipeLink = recipeLink, TotalCalories = calories}
            };

            // Act
            var result = await _controller.Create(recipeViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        [DynamicData("RecipesCSV")]
        public async Task Create_Post_ValidRecipe_RedirectsToIndex_DynamicDataCSV(string name, string recipeLink, int calories)
        {
            // Arrange
            var loggedInNutritionist = new ApplicationUser { Id = "userId" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(loggedInNutritionist);

            var nutritionist = new Nutritionist { Id = "userId" };
            _mockDbContext.Setup(x => x.Nutritionist.FindAsync("userId")).ReturnsAsync(nutritionist);

            var recipeViewModel = new RecipeViewModel
            {
                input = new Recipe
                {
                    NameOfRecipe = name,
                    RecipeLink = recipeLink,
                    TotalCalories = calories
                }
            };

            // Act
            var result = await _controller.Create(recipeViewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        public void RecipesToDisplay_IsEmptyList_WhenContextAndRecipeNull()
        {
            // Arrange
            _mockDbContext.Setup(x => x.Recipe).Returns(MockDbSet<Recipe>(null));

            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);

            var resultModel = result.Model as RecipeViewModel;
            Assert.IsNotNull(resultModel);

            // RecipesToDisplay should be an empty list when both _context and _context.Recipe are null
            CollectionAssert.AreEqual(new List<Recipe>(), resultModel.recipesToDisplay.ToList());
        }


    }
}


    

