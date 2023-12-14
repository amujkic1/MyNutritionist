using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Data;
using MyNutritionist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNutritionist.Controllers.Tests
{
    [TestClass()]
    public class TrainingTests
    {
        [TestMethod]
        public async Task Training_ReturnsCorrectModel()
        {

            var _mockLogger = new Mock<ILogger<HomeController>>();
            var _controller = new HomeController(_mockLogger.Object);

            var result = await _controller.Training() as ViewResult;

            Assert.IsNotNull(result);
            var trainingList = result.Model as List<Training>;
            Assert.IsNotNull(trainingList);
        }
    }
}
