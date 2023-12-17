using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ApplicationDbContext> _mockDbContext;
        private Mock<ILogger<HomeController>> _mockLogger;
        private HomeController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _mockDbContext = new Mock<ApplicationDbContext>(options);

            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object, _mockDbContext.Object);
        }


        [TestMethod]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Error_ReturnsViewResultWithModel()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "some_trace_identifier";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsNotNull(result);
            
        }

        [TestMethod]
        public async Task Training_ReturnsViewResultWithTrainings()
        {
            // Arrange
            var trainingsData = new List<Training>
            {
                new Training { TID = 1, nameOfTraining="Training", image="image.png", link="", duration = 10},
                new Training { TID = 2, nameOfTraining="Training", image="image.png", link="", duration = 40},
                new Training { TID = 3, nameOfTraining="Training", image="image.png", link="", duration = 30},
                new Training { TID = 6, nameOfTraining="Training", image="image.png", link="", duration = 20}
            };

            _mockDbContext.Setup(c => c.Training).ReturnsDbSet(trainingsData);

            // Act
            var result = await _controller.Training();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(List<Training>));
            var model = (List<Training>)viewResult.Model;
            CollectionAssert.AreEqual(trainingsData, model);
        }


    }
}
