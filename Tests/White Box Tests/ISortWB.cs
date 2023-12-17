using Microsoft.AspNetCore.Routing;
using MyNutritionist.Models;
using MyNutritionist.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.White_Box_Tests
{
    [TestClass()]
    public class ISortWB
    {
        [TestMethod]
        public void Sort_NullInput_ReturnsEmptyList()
        {
            // Arrange
            var sorter = new ISort();

            // Act
            List<PremiumUser> result = sorter.Sort(null, (x, y) => x.Id.CompareTo(y.Id));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Sort_EmptyList_ReturnsSameList()
        {
            // Arrange
            var sorter = new ISort();
            List<PremiumUser> users = new List<PremiumUser>();

            // Act
            List<PremiumUser> result = sorter.Sort(users, (x, y) => x.Id.CompareTo(y.Id));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(users.Count, result.Count);
        }

        [TestMethod]
        public void Sort_SortsUsersById()
        {
            // Arrange
            var sorter = new ISort();
            List<PremiumUser> users = new List<PremiumUser>
        {
            new PremiumUser { Weight = 70 },
            new PremiumUser { Weight = 63 },
            new PremiumUser { Weight = 5 }
        };

            // Act
            List<PremiumUser> result = sorter.Sort(users, (x, y) => x.Weight.CompareTo(y.Weight));

            // Assert
            Assert.AreEqual(5, result[0].Weight);
            Assert.AreEqual(63, result[1].Weight);
            Assert.AreEqual(70, result[2].Weight);
        }

    }
}
