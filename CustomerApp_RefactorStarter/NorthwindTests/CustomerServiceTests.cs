using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NorthwindData;
using NorthwindData.Services;

namespace NorthwindTests
{
    public class CustomerServiceTests
    {
        private CustomerService _sut;
        private NorthwindContext _context;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new DbContextOptionsBuilder<NorthwindContext>()
                .UseInMemoryDatabase(databaseName: "Example_DB").Options;
            _context = new NorthwindContext(options);
            _sut = new CustomerService(_context);

            // seed the database
            _sut.CreateCustomer(new Customer 
            { CustomerId = "MAND",
              ContactName = "Nish Mandal",
              CompanyName = "Sparta Global",
              City = "Paris"
            });
            _sut.CreateCustomer(new Customer 
            { CustomerId = "FREN", 
              ContactName = "Cathy French",
              CompanyName = "Sparta Global",
              City = "Ottawa" 
            });
            _sut.CreateCustomer(new Customer
            {
                CustomerId = "USER",
                ContactName = "Test User",
                CompanyName = "Sparta Global",
                City = "Rome"
            });
        }

        [Test]
        public void GivenAValidId_CorrectCustomerIsReturned()
        {
            var result = _sut.GetCustomerById("MAND");
            Assert.That(result, Is.TypeOf<Customer>());
            Assert.That(result.ContactName, Is.EqualTo("Nish Mandal"));
            Assert.That(result.CompanyName, Is.EqualTo("Sparta Global"));
            Assert.That(result.City, Is.EqualTo("Paris"));
        }

        [Test]
        public void GivenANewCustomer_CreateCustomerAddsItToTheDatabase()
        {
            // Arrange
            var numberOfCustomersBefore = _context.Customers.Count();
            var newCustomer = new Customer
            {
                CustomerId = "BEAR",
                ContactName = "Martin Beard",
                CompanyName = "Sparta Global",
                City = "London"
            };

            // Act
            _sut.CreateCustomer(newCustomer);
            var numberofCustomersAfter = _context.Customers.Count();
            var result = _sut.GetCustomerById("BEAR");

            // Assert
            Assert.That(numberOfCustomersBefore + 1, Is.EqualTo(numberofCustomersAfter));

            // Clean Up
            _context.Customers.Remove(newCustomer);
            _context.SaveChanges();
        }

        [Test]
        public void GivenACustomerHasBeenRemoved_RemoveCustomerFromTheDatabase()
        {
            var numberOfCustomersBefore = _context.Customers.Count();
            _sut.RemoveCustomer(_sut.GetCustomerById("USER"));
            var numberofCustomersAfter = _context.Customers.Count();
            Assert.That(numberOfCustomersBefore - 1, Is.EqualTo(numberofCustomersAfter));
        }

        [Test]
        public void GetCustomerList_Returns_CorrectCustomerList()
        {
            var customerList = _sut.GetCustomerList();
            Assert.That(customerList, Is.EqualTo(_context.Customers.ToList()));
        }
    }
}
