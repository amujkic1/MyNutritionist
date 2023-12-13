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


namespace Tests
{
    [TestClass]
  public  class PremiumUserQuoteTests
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
        public async Task Index_WritesQuote()
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
            var user = result.Model as PremiumUser;
            Assert.IsNotNull(user);
            Assert.AreEqual("userId", user.Id);
			var htmlContent = result.ViewData["quoteMessage"] as string;
			Assert.IsFalse(string.IsNullOrWhiteSpace(htmlContent), "HTML sadržaj je prazan.");
		}
        [TestMethod]
		public async Task Index_WritesQuote_MultipleCalls()
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
			var result1 = await _controller.Index() as ViewResult;
			var result2 = await _controller.Index() as ViewResult;
			var result3 = await _controller.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result1);
			Assert.IsNotNull(result2);
			Assert.IsNotNull(result3);

			var user1 = result1.Model as PremiumUser;
			var user2 = result2.Model as PremiumUser;
			var user3 = result3.Model as PremiumUser;

			Assert.IsNotNull(user1);
			Assert.IsNotNull(user2);
			Assert.IsNotNull(user3);

			Assert.AreEqual("userId", user1.Id);
			Assert.AreEqual("userId", user2.Id);
			Assert.AreEqual("userId", user3.Id);

			var htmlContent1 = result1.ViewData["quoteMessage"] as string;
			var htmlContent2 = result2.ViewData["quoteMessage"] as string;
			var htmlContent3 = result3.ViewData["quoteMessage"] as string;

			Assert.IsFalse(string.IsNullOrWhiteSpace(htmlContent1), "HTML sadržaj za prvi poziv je prazan.");
			Assert.IsFalse(string.IsNullOrWhiteSpace(htmlContent2), "HTML sadržaj za drugi poziv je prazan.");
			Assert.IsFalse(string.IsNullOrWhiteSpace(htmlContent3), "HTML sadržaj za treći poziv je prazan.");

			// Proverite da li se svi citati za sva tri poziva Index metode upoređuju
			Assert.AreEqual(htmlContent1, htmlContent2);
			Assert.AreEqual(htmlContent2, htmlContent3);
		}

	}

}
