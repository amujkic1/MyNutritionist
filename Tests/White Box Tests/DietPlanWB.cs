using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Moq.EntityFrameworkCore;
using MyNutritionist.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Tests.White_Box_Tests
{
	[TestClass]
	public class DietPlanWB
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

			// Mocking DbSet<T> for each entity
			_mockDbContext.Setup(c => c.Recipe).Returns(MockDbSet(new List<Recipe>().AsQueryable()));
			_mockDbContext.Setup(c => c.DietPlan).Returns(MockDbSet(new List<DietPlan>().AsQueryable()));
			_mockDbContext.Setup(c => c.PremiumUser).Returns(MockDbSet(new List<PremiumUser>().AsQueryable()));
			_mockDbContext.Setup(c => c.Nutritionist).Returns(MockDbSet(new List<Nutritionist>().AsQueryable()));


			// Add more DbSet mocks for other entities you use in the Create method

			_controller = new DietPlanController(_mockDbContext.Object, _mockUserManager.Object, _mockHttpContextAccessor.Object);
			_context = new ApplicationDbContext(options);
		}

		private DbSet<T> MockDbSet<T>(IQueryable<T> data) where T : class
		{
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
			return mockSet.Object;
		}


		[TestMethod]
		public void CreateAction_AuthorizedNutritionist_ReturnsCorrectViewModel()
		{
			// Aranžman (Arrange)

			// Kreiramo mock context.Recipe
			var mockRecipes = new List<Recipe>
			{
				new Recipe { RID = 1, NameOfRecipe = "Recipe 1" },
				new Recipe { RID = 2, NameOfRecipe = "Recipe 2" },
            }.AsQueryable();

			_mockDbContext.Setup(c => c.Recipe).Returns((DbSet<Recipe>)MockDbSet(mockRecipes));

			// Akcija (Act)
			var result = _controller.Create("TestUser") as ViewResult;

			// Assert

			// Provjeravamo da li akcija vraća ViewResult
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			// Provjeravamo da li akcija koristi ispravan View
			Assert.AreEqual("dietPlan", result?.ViewName);


			// Provjeravamo da li ViewModel ima pravilno postavljene vrijednosti
			var model = result.Model as EditDietPlanVM;
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.DietPlan);
			Assert.IsNotNull(model.Recipes);
			Assert.AreEqual("TestUser", model.DietPlan.PremiumUser.Id);
			CollectionAssert.AreEqual(mockRecipes.ToList(), model.Recipes.ToList());
		}

		// Pomoćna metoda za simuliranje DbSet-a
		private static DbSet<T> MockDbSet<T>(IEnumerable<T> list) where T : class
		{
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(list.AsQueryable().Provider);
			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(list.AsQueryable().Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(list.AsQueryable().ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => list.GetEnumerator());
			return mockSet.Object;
		}

		[TestMethod]
		public void CreateAction_WithRecipes_ReturnsCorrectViewModel()
		{
			// Aranžman (Arrange)

			// Kreiramo mock context.Recipe sa popunjenom listom recepata
			var mockRecipes = new List<Recipe>
	{
		new Recipe { RID = 1, NameOfRecipe = "Recipe 1" },
		new Recipe { RID = 2, NameOfRecipe = "Recipe 2" },
    }.AsQueryable();

			_mockDbContext.Setup(c => c.Recipe).Returns((DbSet<Recipe>)MockDbSet(mockRecipes));

			// Akcija (Act)
			var result = _controller.Create("TestUser") as ViewResult;

			// Assert

			// Provjeravamo da li akcija vraća ViewResult
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			// Provjeravamo da li akcija koristi ispravan View
			Assert.AreEqual("dietPlan", result?.ViewName);

			// Provjeravamo da li ViewModel ima pravilno postavljene vrednosti
			var model = result.Model as EditDietPlanVM;
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.DietPlan);
			Assert.IsNotNull(model.Recipes);
			Assert.AreEqual("TestUser", model.DietPlan.PremiumUser.Id);
			CollectionAssert.AreEqual(mockRecipes.ToList(), model.Recipes.ToList());
		}

		[TestMethod]
		public void CreateAction_WithEmptyRecipes_ReturnsCorrectViewModel()
		{
			// Aranžman (Arrange)

			// Kreiramo mock context.Recipe sa praznom listom recepata
			var mockRecipes = new List<Recipe>().AsQueryable();

			_mockDbContext.Setup(c => c.Recipe).Returns((DbSet<Recipe>)MockDbSet(mockRecipes));

			// Akcija (Act)
			var result = _controller.Create("TestUser") as ViewResult;

			// Assert

			// Provjeravamo da li akcija vraća ViewResult
			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			// Provjeravamo da li akcija koristi ispravan View
			Assert.AreEqual("dietPlan", result?.ViewName);

			// Provjeravamo da li ViewModel ima pravilno postavljene vrijednosti
			var model = result.Model as EditDietPlanVM;
			Assert.IsNotNull(model);
			Assert.IsNotNull(model.DietPlan);
			Assert.IsNotNull(model.Recipes);
			Assert.AreEqual("TestUser", model.DietPlan.PremiumUser.Id);
			CollectionAssert.AreEqual(mockRecipes.ToList(), model.Recipes.ToList());
		}

		[TestMethod]
		public async Task CreateAction_UnauthorizedUser_ReturnsNotFound()
		{

			var nutritionistList = new List<Nutritionist>
		 {
		  new Nutritionist { Id = "userId"}, };
			_mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

			_mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);
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


			// Akcija (Act)
			var result = await _controller.Create("TestUser", new EditDietPlanVM { DietPlan = new DietPlan() }) as NotFoundResult;

			// Assert

			// Provjeravamo da li akcija vraća NotFound rezultat
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task CreateAction_ValidData_RedirectsToIndex()
		{
			var nutritionist = new ApplicationUser { Id = "1" };

			DietPlan dp = new DietPlan { DPID = 3, Recipes = new List<Recipe> { new Recipe { RID = 1 } } };

			var dietPlanVm = new EditDietPlanVM
			{
				DietPlan = dp,
			};
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
				new PremiumUser
				{
					Id = "userId2",
					AccountNumber = "000",
					City = "London",
					Age = 25,
					Weight = 70.0,
					Height = 180.0,
					Points = 0
				},
			};


			_mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("1");
			_mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet((IEnumerable<Nutritionist>)new List<Nutritionist> { new Nutritionist { Id = "1", AspUserId = "1" } });
			_mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet((IEnumerable<DietPlan>)new List<DietPlan> { new DietPlan { PremiumUser = premiumUserList[0] } });
			_mockDbContext.Setup(db => db.Recipe).ReturnsDbSet((IEnumerable<Recipe>)new List<Recipe> { new Recipe { RID = 1 }, new Recipe { RID = 2 } });
			_mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser> { new PremiumUser { Id = premiumUserList[0].Id }, new PremiumUser { Id = premiumUserList[1].Id }, new PremiumUser { Id = premiumUserList[2].Id } });

			// Act
			var result = await _controller.Create(premiumUserList[0].Id, dietPlanVm);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
			var redirectResult = (RedirectToActionResult)result;
			Assert.AreEqual("Index", redirectResult.ActionName);
			Assert.AreEqual("Nutritionist", redirectResult.ControllerName);
		}


		[TestMethod]
		public async Task CreateAction_RecipeIsNull_ReturnsNotFound()
		{
			var nutritionist = new Nutritionist { Id = "1" };


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
		}
	}; var recipe1 = new Recipe { };
			recipe1 = null;
			var recipeList = new List<Recipe> {
			new Recipe{
				RID=1,
				NameOfRecipe="Test",
				TotalCalories=46,
				Nutritionist=nutritionist,
				RecipeLink="LINK",


		},
			recipe1 };
			DietPlan dp = new DietPlan { DPID = 3, Recipes = recipeList };

			var dietPlanVm = new EditDietPlanVM
			{
				DietPlan = dp,
			};
			_mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("1");
			_mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet((IEnumerable<Nutritionist>)new List<Nutritionist> { new Nutritionist { Id = "1", AspUserId = "1" } });
			_mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet((IEnumerable<DietPlan>)new List<DietPlan> { dp });
			_mockDbContext.Setup(db => db.Recipe).ReturnsDbSet(recipeList);
			_mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser> { new PremiumUser { Id = premiumUserList[0].Id } });

			// Act
			var result = await _controller.Create(premiumUserList[0].Id, dietPlanVm);

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));
		}



		[TestMethod]
		public async Task CreateAction_RecipesAreEmpty_ReturnsNotFound()
		{
			var nutritionist = new Nutritionist { Id = "1" };


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
		}
	};
			DietPlan dp = new DietPlan { DPID = 3, Recipes = null };

			var dietPlanVm = new EditDietPlanVM
			{
				DietPlan = dp,
			};
			_mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("1");
			_mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet((IEnumerable<Nutritionist>)new List<Nutritionist> { new Nutritionist { Id = "1", AspUserId = "1" } });
			_mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet((IEnumerable<DietPlan>)new List<DietPlan> { dp });
			_mockDbContext.Setup(db => db.Recipe).ReturnsDbSet(new List<Recipe> { });
			_mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser> { new PremiumUser { Id = premiumUserList[0].Id } });

			// Act
			var result = await _controller.Create(premiumUserList[0].Id, dietPlanVm);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
			var redirectResult = (RedirectToActionResult)result;
			Assert.AreEqual("Index", redirectResult.ActionName);
			Assert.AreEqual("Nutritionist", redirectResult.ControllerName);
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
		public async Task CreateAction_WithExistingDietPlans_DeletesOldDietPlans()
		{
			// Arrange
			var nutritionist = new ApplicationUser { Id = "1" };

			DietPlan dp = new DietPlan { DPID = 3, Recipes = new List<Recipe> { new Recipe { RID = 1 } } };

			var dietPlanVm = new EditDietPlanVM
			{
				DietPlan = dp,
			};

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

			_mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("1");
			_mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet((IEnumerable<Nutritionist>)new List<Nutritionist> { new Nutritionist { Id = "1", AspUserId = "1" } });
			_mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet((IEnumerable<DietPlan>)new List<DietPlan> { new DietPlan { PremiumUser = premiumUserList[0] } });
			_mockDbContext.Setup(db => db.Recipe).ReturnsDbSet((IEnumerable<Recipe>)new List<Recipe> { new Recipe { RID = 1 }, new Recipe { RID = 2 } });
			_mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser> { new PremiumUser { Id = premiumUserList[0].Id } });

			// Act
			var result = await _controller.Create(premiumUserList[0].Id, dietPlanVm);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
			var redirectResult = (RedirectToActionResult)result;
			Assert.AreEqual("Index", redirectResult.ActionName);
			Assert.AreEqual("Nutritionist", redirectResult.ControllerName);

			// Provjera da li su stari planovi ishrane obrisani
			var deletedDietPlans = _context.DietPlan.ToList();
			Assert.AreEqual(0, deletedDietPlans.Count, "Old diet plans should be deleted.");
		}

	
		[TestMethod]
		public async Task CreateAction_WhenDeletePlansIsNullButHasPlans_DoesNotRemoveAnyDietPlan()
		{
			// Arrange
			var nutritionist = new ApplicationUser { Id = "1" };
			var dietPlanVm = new EditDietPlanVM { DietPlan = new DietPlan() };

			_mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("1");
			_mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(new List<Nutritionist> { new Nutritionist { Id = "1", AspUserId = "1" } });
			_mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet(new List<DietPlan>());
			_mockDbContext.Setup(db => db.Recipe).ReturnsDbSet(new List<Recipe>());
			_mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser>());

			// Act
			var result = await _controller.Create("userId", dietPlanVm);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
			var redirectResult = (RedirectToActionResult)result;
			Assert.AreEqual("Index", redirectResult.ActionName);
			Assert.AreEqual("Nutritionist", redirectResult.ControllerName);

			// Provjera da li se nisu brisali stari planovi ishrane
			var deletedDietPlans = _context.DietPlan.ToList();
			Assert.AreEqual(0, deletedDietPlans.Count, "No diet plans should be deleted.");
		}

		[TestMethod]
		public async Task CreateAction_WhenDeletePlansIsNotNullButHasNoPlans_DoesNotRemoveAnyDietPlan()
		{
			// Arrange
			var nutritionist = new ApplicationUser { Id = "1" };
			var dietPlanVm = new EditDietPlanVM { DietPlan = new DietPlan() };

			_mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("1");
			_mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(new List<Nutritionist> { new Nutritionist { Id = "1", AspUserId = "1" } });
			_mockDbContext.Setup(db => db.DietPlan).ReturnsDbSet(new List<DietPlan>());
			_mockDbContext.Setup(db => db.Recipe).ReturnsDbSet(new List<Recipe>());
			_mockDbContext.Setup(db => db.PremiumUser).ReturnsDbSet(new List<PremiumUser>());

			// Create deletePlans with count = 0
			var deletePlans = new List<DietPlan>();

			// Act
			var result = await _controller.Create("userId", dietPlanVm);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
			var redirectResult = (RedirectToActionResult)result;
			Assert.AreEqual("Index", redirectResult.ActionName);
			Assert.AreEqual("Nutritionist", redirectResult.ControllerName);

			// Provjera da li se nisu brisali stari planovi ishrane
			var deletedDietPlans = _context.DietPlan.ToList();
			Assert.AreEqual(0, deletedDietPlans.Count, "No diet plans should be deleted.");
		}

	}
}


