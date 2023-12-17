using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Moq;
using Moq.EntityFrameworkCore;
using MyNutritionist.Controllers;
using MyNutritionist.Data;
using MyNutritionist.Models;
using MyNutritionist.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace MyNutritionist.Controllers.Tests
{
    [TestClass()]
    public class NutritionistControllerTests
    {
        private ApplicationDbContext _context;
        private NutritionistController _controller;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ApplicationDbContext> _mockDbContext;

        public static IEnumerable<object[]> ReadCSV()
        {
            using (var reader = new StreamReader("Data/NutritionistControllerCSV.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rows = csv.GetRecords<dynamic>();
                foreach (var row in rows)
                {
                    yield return new object[] { 
                    row.Id, 
                    row.FullName,
                    row.Email,
                    row.NutriUsername,
                    row.Image
                    };
                }
            }
        }

        static IEnumerable<object[]> NutritionistCSV
        {
            get
            {
                return ReadCSV();
            }
        }

        public static IEnumerable<object[]> ReadXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Data/NutritionistControllerXML.xml");

            foreach (XmlNode nutritionistNode in doc.DocumentElement.ChildNodes)
            {
                string userId = nutritionistNode.SelectSingleNode("userId")?.InnerText.Trim();

                List<PremiumUser> premiumUserPoints = new List<PremiumUser>();
                foreach (XmlNode premiumUserNode in nutritionistNode.SelectSingleNode("premiumUsers").ChildNodes)
                {
                    int points;
                    if (int.TryParse(premiumUserNode.SelectSingleNode("Points")?.InnerText.Trim(), out points))
                    {
                        premiumUserPoints.Add(new PremiumUser { Points = points });
                    }
                }

                yield return new object[]
                {
                    userId,
                    premiumUserPoints as List<PremiumUser>
                };
            }

        }

        static IEnumerable<object[]> NutritionistXML
        {
            get
            {
                return ReadXML();
            }
        }



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

            _controller = new NutritionistController(_mockDbContext.Object, _mockHttpContextAccessor.Object, _mockUserManager.Object);
        }


        [TestMethod]
        public async Task Index_WhenNutritionistIsNull_ReturnsNotFound()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Index();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Index_ReturnsRightUser()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser()} },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser() } }
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var user = result.Model as Nutritionist;
            Assert.IsNotNull(user);
            Assert.AreEqual("userId", user.Id);
            Assert.AreEqual(3, user.PremiumUsers.Count);
        }

        [TestMethod]
        public async Task Edit_WhenNutritionistIsNull_ReturnsNotFound()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit();

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsRightUser()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser()} },
                new Nutritionist { Id = "userId1" , PremiumUsers = new List<PremiumUser> { new PremiumUser() } },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser() } },
                new Nutritionist { Id = "userId3" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser(), new PremiumUser() } }
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId3");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var user = result.Model as Nutritionist;
            Assert.IsNotNull(user);
            Assert.AreEqual("userId3", user.Id);
            Assert.AreEqual(4, user.PremiumUsers.Count);
        }

        [TestMethod]
        public async Task EditHttp_WhenNutritionistIsNull_ReturnsNotFound()
        {
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId"},
             };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId1");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit(new Nutritionist());

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditHttp_WhenUpdatedNutritionistIsNull_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(null);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditHttp_WhenNutritionistExists_ReturnsRedirectToAction()
        {
            // Arrange
            var userId = "userId";
            var nutritionistList = new List<Nutritionist>
            {
                  new Nutritionist { Id = userId },
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit(new Nutritionist
            {
                FullName = "New FullName",
                Email = "newemail@example.com",
                NutriUsername = "newNutriUsername",
                Image = "image.jpg"
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Verify that the user's information was updated
            var updatedUser = _mockDbContext.Object.Nutritionist.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("New FullName", updatedUser.FullName);
            Assert.AreEqual("newemail@example.com", updatedUser.Email);
            Assert.AreEqual("newNutriUsername", updatedUser.NutriUsername);
            Assert.AreEqual("image.jpg", updatedUser.Image);
        }



        [DynamicData("NutritionistCSV")]
        [TestMethod]
        public async Task EditHttp_WhenNutritionistDataDoesntChange_ReturnsRedirectToAction(string userId, string fullName, string email, string username, string image)
        {
            // Arrange
            var nutritionistList = new List<Nutritionist>
            {
                  new Nutritionist { Id = userId, FullName = fullName, Email = email, NutriUsername = username, Image=image},
            };
            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);
            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.Edit(new Nutritionist());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", (result as RedirectToActionResult).ActionName);

            // Verify that the user's information was updated
            var updatedUser = _mockDbContext.Object.Nutritionist.SingleOrDefault(u => u.Id == userId);
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(fullName, updatedUser.FullName);
            Assert.AreEqual(email, updatedUser.Email);
            Assert.AreEqual(username, updatedUser.NutriUsername);
            Assert.AreEqual(image, updatedUser.Image);
        }

        [TestMethod]
        public async Task SortByNames_NutritionistIsNull_ReturnsNotFound()
        {
            //Arrange
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser()} },
                new Nutritionist { Id = "userId1" , PremiumUsers = new List<PremiumUser> { new PremiumUser() } },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser() } },
                new Nutritionist { Id = "userId3" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser(), new PremiumUser() } }
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId4");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.SortByNames();

            //Verify
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task SortByNames_NutritionistIsNotNull_ChecksSortedList()
        {
            //Arrange
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser { FullName = "Emily" }, new PremiumUser { FullName = "Liam" }, new PremiumUser { FullName = "Sophia" } } },
                new Nutritionist { Id = "userId1" , PremiumUsers = new List<PremiumUser> { new PremiumUser { FullName = "Ava" } } },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser { FullName = "Lucas" }, new PremiumUser { FullName = "Ava" } } },
                new Nutritionist { Id = "userId3" , PremiumUsers = new List<PremiumUser> { new PremiumUser { FullName = "Noah" }, new PremiumUser { FullName = "Olivia" }, new PremiumUser { FullName = "Mia" }, new PremiumUser { FullName = "Charlotte" } } }
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId3");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.SortByNames();

            //Verify
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual("Index", viewResult.ViewName); // Checks if the correct view is returned

            var userModel = (Nutritionist)viewResult.Model; // Get the model data
            Assert.AreEqual("userId3", userModel.Id);

            var sortedList = nutritionistList[3].PremiumUsers.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase).ToList();
            CollectionAssert.AreEqual(sortedList, userModel.PremiumUsers as ICollection, "List is not sorted");
        }


        [TestMethod]
        [DynamicData("NutritionistXML")]
        public async Task SortByPoints_NutritionistIsNotNull_ChecksSortedList(string userId, List<PremiumUser> premiumUsers)
        {
            //Arrange
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser { Points = 12}, new PremiumUser { Points = 11 }, new PremiumUser { Points = 13 } } },
                new Nutritionist { Id = "userId1" , PremiumUsers = new List<PremiumUser> { new PremiumUser { Points = 10 } } },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser { Points = 12 }, new PremiumUser { Points = 22 } } },
                new Nutritionist { Id = "userId3" , PremiumUsers = new List<PremiumUser> { new PremiumUser { Points = 44 }, new PremiumUser { Points = 13 }, new PremiumUser { Points = 90 }, new PremiumUser { Points = 122 } } },
                new Nutritionist { Id = userId, PremiumUsers = premiumUsers}
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns(userId);

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.SortByPoints();

            //Verify
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual("Index", viewResult.ViewName); // Checks if the correct view is returned
            
            var userModel = (Nutritionist)viewResult.Model; // Get the model data
            Assert.AreEqual(userId, userModel.Id);

            var sortedList = premiumUsers.OrderByDescending(x => x.Points).ToList();
            CollectionAssert.AreEqual(sortedList, userModel.PremiumUsers as ICollection, "List is not sorted");
        }

        [TestMethod]
        public async Task SortByPoints_NutritionistIsNull_ReturnsNotFound()
        {
            //Arrange
            var nutritionistList = new List<Nutritionist>
            {
                new Nutritionist { Id = "userId" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser()} },
                new Nutritionist { Id = "userId1" , PremiumUsers = new List<PremiumUser> { new PremiumUser() } },
                new Nutritionist { Id = "userId2" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser() } },
                new Nutritionist { Id = "userId3" , PremiumUsers = new List<PremiumUser> { new PremiumUser(), new PremiumUser(), new PremiumUser(), new PremiumUser() }}
            };

            _mockUserManager.Setup(um => um.GetUserId(_mockHttpContextAccessor.Object.HttpContext.User)).Returns("userId4");

            _mockDbContext.Setup(db => db.Nutritionist).ReturnsDbSet(nutritionistList);

            // Act
            var result = await _controller.SortByPoints();

            //Verify
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }

}