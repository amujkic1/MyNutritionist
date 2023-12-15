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
namespace Tests.White_Box_Tests
{
	[TestClass]
	public  class DietPlanWB
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
		// Testiranje autorizacije
		// Dodajte reference na Moq i druge potrebne biblioteke

		// Testiranje autorizacije sa Moq
		/*[TestMethod]
		public void CreateAction_AuthorizedNutritionist_ReturnsView_WithMoq()
		{
			// Priprema mock objekta za kontroler, UserManager i HttpContextAccessor
			var mockController = new Mock<NutritionistController>();
			var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
			var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

			// Simulacija autorizovanog nutritionista
			mockUserManager.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
			mockHttpContextAccessor.Setup(a => a.HttpContext.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
		new Claim(ClaimTypes.Role, "Nutritionist")
			})));

			mockController.Object.UserManager = mockUserManager.Object;
			mockController.Object.ControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContextAccessor.Object.HttpContext
			};

			// Poziv funkcije
			var result = mockController.Object.Create("TestUser") as ViewResult;

			// Provera da li je rezultat View
			Assert.IsNotNull(result);
			Assert.AreEqual("Create", result.ViewName);
		}*/

	}
}
