using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Tests
{
    [TestClass]
    public class RegisteredControllerTests
    {
        private RegisteredUserController _controller;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<HttpContextAccessor> _httpContextAccessorMock;
        private Mock<ApplicationDbContext> _contextMock;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _userManagerMock = MockUserManager<ApplicationUser>();
            _httpContextAccessorMock = new Mock<HttpContextAccessor>();
            _contextMock = new Mock<ApplicationDbContext>();

            _controller = new RegisteredUserController(_contextMock.Object, _httpContextAccessorMock.Object, _userManagerMock.Object, null);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenRegisteredUserIsNull()
        {
            // Arrange
            var userId = "testUserId";
            _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _contextMock.Setup(c => c.RegisteredUser.FirstOrDefaultAsync(It.IsAny<Expression<Func<RegisteredUser, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisteredUser)null);

            // Act
            var result = await _controller.Delete();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var userStoreMock = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        }
    }
}