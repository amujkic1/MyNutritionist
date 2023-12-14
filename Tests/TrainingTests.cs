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
            var _mockDbContext = new Mock<ApplicationDbContext>();
            var _mockLogger = new Mock<ILogger<HomeController>>();
            var _controller = new HomeController(_mockLogger.Object, _mockDbContext.Object);

            var trainings = new List<Training> {
                new Training { TID = 1, nameOfTraining="Training", image="image.png", link="", duration = 10},
                new Training { TID = 2, nameOfTraining="Training", image="image.png", link="", duration = 40},
                new Training { TID = 3, nameOfTraining="Training", image="image.png", link="", duration = 30},
                new Training { TID = 4, nameOfTraining="Training", image="image.png", link="", duration = 20},
                new Training { TID = 5, nameOfTraining="Training", image="image.png", link="", duration = 10},
                new Training { TID = 6, nameOfTraining="Training", image="image.png", link="", duration = 20}
            };

            _mockDbContext.Setup(db => db.Training).ReturnsDbSet(trainings);

            var result = await _controller.Training() as ViewResult;

            Assert.IsNotNull(result);
            var trainingList = result.Model as List<Training>;
            Assert.IsNotNull(trainingList);
            Assert.AreEqual(0, trainingList.Count);
        }
    }
}
