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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.White_Box_Tests
{
	[TestClass]
	public class RegisteredUserIndexWBTests
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

		// Prepare object for data driven test
		public static IEnumerable<object[]> ProgressData
		{
			get
			{
				var nutritionTips = new List<NutritionTipsAndQuotes>
				{
					new NutritionTipsAndQuotes
					{
						NTAQId= 1,
						QuoteText="Quote1",
					},
					new NutritionTipsAndQuotes
					{
						NTAQId = 2,
						QuoteText="Quote2",
					}
				};

				var registeredUser = new RegisteredUser { Id = "userId" };

				return new[]
				{
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-6), ConsumedCalories = 1500, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-5), BurnedCalories = -200, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-5), RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-5), ConsumedCalories = 1500, BurnedCalories = 400, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-5), ConsumedCalories = 1500, BurnedCalories = -200, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-4), ConsumedCalories = 1500, BurnedCalories = 50, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-3), ConsumedCalories = -500, BurnedCalories = 200, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-2), ConsumedCalories = -500, BurnedCalories = -50, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-2), ConsumedCalories = -500, BurnedCalories = 50, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(-1), ConsumedCalories = 50, BurnedCalories = 200, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(0), ConsumedCalories = 50, BurnedCalories = -50, RegisteredUser = registeredUser } },
					new object[] { nutritionTips, registeredUser, new Progress { Date = DateTime.Now.Date.AddDays(0), ConsumedCalories = 50, BurnedCalories = 50, RegisteredUser = registeredUser } },
				};
			}
		}

		[TestMethod]
		[DynamicData("ProgressData")]
		public async Task Index_ReturnsViewResultForDifferentValuesOfProgress_WithCorrectModelAndProgressData(List<NutritionTipsAndQuotes> nutritionTips, 
			RegisteredUser registeredUser, Progress progressData)
		{

			_mockDbContext.Setup(c => c.Progress).ReturnsDbSet(new List<Progress>());
			_mockDbContext.Setup(db => db.NutritionTipsAndQuotes).ReturnsDbSet(nutritionTips);

			// Act
			var result = await _controller.SetProgressData(registeredUser);

			// Assert
			Assert.IsNotNull(result);
			// Check if lists of consumed and burnt calories are both filled with data
			Assert.AreEqual(2, result.Count);

		}

		[TestMethod]
		public async Task Index_ReturnsViewResult_WithCorrectModelAndProgressData()
		{
			// Arrange
			var userId = "userId";
			var registeredUserList = new List<RegisteredUser>
			{
				new RegisteredUser { Id = "userId"},
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


			_mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);

			_mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
			_mockDbContext.Setup(c => c.Progress).ReturnsDbSet(new List<Progress>());

			_mockDbContext.Setup(db => db.NutritionTipsAndQuotes).ReturnsDbSet(nutritionTips);

			// Act
			var result = await _controller.Index();

			// Assert
			if (result is ViewResult viewResult)
			{
				var consumedCaloriesProgressData = viewResult.ViewData["ConsumedCaloriesProgressData"];
				var burnedCaloriesProgressData = viewResult.ViewData["BurnedCaloriesProgressData"];

				Assert.IsNotNull(consumedCaloriesProgressData);
				Assert.IsNotNull(burnedCaloriesProgressData);
			}
			else
			{
				Assert.Fail("Unexpected result type");
			}
		}

		[TestMethod]
		public async Task Index_ReturnsNotFoundWhenRegisteredUserIsNull()
		{
			// Arrange
			var userId = "userId";
			var registeredUserList = new List<RegisteredUser>
			{
				new RegisteredUser { Id = "userId1"},
			 };


			_mockDbContext.Setup(db => db.RegisteredUser).ReturnsDbSet(registeredUserList);

			_mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
			_mockDbContext.Setup(c => c.Progress).ReturnsDbSet(new List<Progress>());

			// Act
			var result = await _controller.Index();

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));

		}

	}
}