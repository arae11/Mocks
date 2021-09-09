using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using NorthwindBusiness;
using NorthwindData;
using NorthwindData.Services;
using Microsoft.EntityFrameworkCore;

namespace NorthwindTests
{
    public class CustomerManagerShould
    {
        //private CustomerManager sut;

        [Test]
        public void BeAbleToBeConstructed()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();

            // Act
            var sut = new CustomerManager(mockCustomerService.Object);

            // Assert
            Assert.That(sut, Is.InstanceOf<CustomerManager>());
        }

        [Test]
        public void ReturnTrue_WhenUpdateIsCalled_WithValidId()
        {
            // Arrange
            
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer
            {
                CustomerId = "ROCK"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(originalCustomer);
            var sut = new CustomerManager(mockCustomerService.Object);

            // Act
            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.That(result);
        }

        [Test]
        public void UpdateSelectedCustomer_WhenUpdateIsCalled_WithValidId()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(originalCustomer);
            var sut  = new CustomerManager(mockCustomerService.Object);

            // Act
            var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);

            // Assert
            Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
            Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
            Assert.That(sut.SelectedCustomer.Country, Is.EqualTo("UK"));
            Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Chester"));
        }

        [Test]
        public void ReturnFalse_WhenUpdateIsCalled_WithValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer
            {
                CustomerId = "ROCK"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns((Customer)null);
            var sut  = new CustomerManager(mockCustomerService.Object);

            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void NotUpdateSelectedCustomer_WhenUpdateIsCalled_WithInvalidId()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns((Customer)null);
            var sut = new CustomerManager(mockCustomerService.Object);
            sut.SelectedCustomer = originalCustomer;

            // Act
            var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);

            // Assert
            Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
            Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
            Assert.That(sut.SelectedCustomer.Country, Is.Null);
            Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Telford"));
        }

        [Test]
        public void ReturnFalse_WhenUpdateIsCalled_AndADatabaseExceptionIsThrown()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer();
            mockCustomerService.Setup(cs => cs.GetCustomerById(It.IsAny<string>())).Returns(originalCustomer);
            mockCustomerService.Setup(cs => cs.SaveCustomerChanges()).Throws<DbUpdateConcurrencyException>();

            var sut = new CustomerManager(mockCustomerService.Object);

            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.That(result, Is.False);
        }

        [Test]
        public void NotChangeTheSelectedCustomer_WhenUpdateIsCalled_AndADatabaseExceptionIsThrown()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(originalCustomer);
            mockCustomerService.Setup(cs => cs.SaveCustomerChanges()).Throws<DbUpdateConcurrencyException>();

            var sut = new CustomerManager(mockCustomerService.Object);
            sut.SelectedCustomer = new Customer
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };

            // Act
            var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);

            // Assert
            Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
            Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
            Assert.That(sut.SelectedCustomer.Country, Is.Null);
            Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Telford"));
        }

        [Test]
        public void DeleteSelectedCustomer_WhenDeleteIsCalled_WithValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(originalCustomer);
            var sut  = new CustomerManager(mockCustomerService.Object);
            var result = sut.Delete("ROCK");
            Assert.That(sut.SelectedCustomer, Is.Null);
        }

        [Test]
        public void NotDeleteAUser_WhenDeleteIsCalled_WithAnInvalidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns((Customer)null);
            var sut  = new CustomerManager(mockCustomerService.Object);
            var result = sut.Delete("ROCK");
            Assert.That(result, Is.False);
        }

        [Test]
        public void SaveCustomerChanges_WhenUpdateIsCalled_WithValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(new Customer());
            var sut = new CustomerManager(mockCustomerService.Object);

            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            mockCustomerService.Verify(cs => cs.SaveCustomerChanges(), Times.Once);
        }

        [Test]
        public void LetsSeeWhatHappens_WhenUpdateIsCalled_AndAllInvocationsAreNotSetUp()
        {
            var mockCustomerService = new Mock<ICustomerService>(MockBehavior.Strict);
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(new Customer());
            mockCustomerService.Setup(cs => cs.SaveCustomerChanges());
            var sut = new CustomerManager(mockCustomerService.Object);

            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            Assert.That(result);
        }
    }
}
