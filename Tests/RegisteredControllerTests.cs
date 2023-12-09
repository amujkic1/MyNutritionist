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

            _controller = new RegisteredUserController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object, null);
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
    }

}
