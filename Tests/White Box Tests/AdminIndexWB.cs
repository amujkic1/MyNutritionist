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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests.White_Box_Tests
{
    [TestClass]
    public class AdminIndexWB
    {
        private AdminController _controller;
        private Mock<ApplicationDbContext> _mockDbContext;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;



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



            _controller = new AdminController(_mockDbContext.Object, _mockUserManager.Object);

        }

        [TestMethod]
        public async Task Index_ReturnsFilteredPremiumUsers()
        {
            var premiumuser1 = new PremiumUser();
            var premiumuser2 = new PremiumUser();
            var premiumuser3 = new PremiumUser();

            var nutritionists = new List<Nutritionist>
        {
            new Nutritionist { PremiumUsers = new List<PremiumUser> { premiumuser1 } },
            new Nutritionist { PremiumUsers = new List<PremiumUser>() }
        };

            var premiumUsersWithoutNutritionist = new List<PremiumUser>
        {
           premiumuser1,
           premiumuser2,
           premiumuser3
        };

            _mockDbContext.Setup(c => c.Nutritionist)
                .ReturnsDbSet(nutritionists);

            _mockDbContext.Setup(c => c.PremiumUser)
              .ReturnsDbSet(premiumUsersWithoutNutritionist);

            // Act
            var result = await _controller.Index();
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<PremiumUser>));
            var model = viewResult.Model as List<PremiumUser>;

            Assert.AreEqual(2, model.Count()); // Verify the correct number of premium users are returned
        }

        [TestMethod]
        public async Task Index_ReturnsEmptyView_WhenNoPremiumUsers()
        {

            var nutritionists = new List<Nutritionist>
        {
            new Nutritionist { PremiumUsers = new List<PremiumUser>() }
        };


            _mockDbContext.Setup(c => c.Nutritionist)
              .ReturnsDbSet(nutritionists);

            _mockDbContext.Setup(c => c.PremiumUser)
              .ReturnsDbSet(new List<PremiumUser>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsTrue(viewResult.ViewData.Model == null || !((IEnumerable<PremiumUser>)viewResult.Model).Any());
            var model = viewResult.Model as List<PremiumUser>;

            Assert.AreEqual(0, model.Count());
        }

        [TestMethod]
        public async Task Index_ReturnsCorrectView_WhenNoNutritionists()
        {

            var nutritionists = new List<Nutritionist>();

            var premiumuser1 = new PremiumUser();
            var premiumuser2 = new PremiumUser();
            var premiumuser3 = new PremiumUser();

            var premiumUsersWithoutNutritionist = new List<PremiumUser>
        {
           premiumuser1,
           premiumuser2,
           premiumuser3
        };

            _mockDbContext.Setup(c => c.Nutritionist)
              .ReturnsDbSet(nutritionists);

            _mockDbContext.Setup(c => c.PremiumUser)
                .ReturnsDbSet(premiumUsersWithoutNutritionist);

            var result = await _controller.Index();
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<PremiumUser>));
            var model = viewResult.Model as List<PremiumUser>;

            Assert.AreEqual(3, model.Count());
        }



    }
}
