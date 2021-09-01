using DemoTest.Api;
using DemoTest.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DemoTest.Tests
{
    public class DemoTest
    {
        [Fact]
        public void Test1()
        {
            Assert.True(1 == 1);
        }

        [Fact]
        public async Task CustomerIntegrationTest()
        {
            // Create DB
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
            var context = new CustomerContext(optionsBuilder.Options);

            // Deleta all existing Customers in DB (or drop DB)
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Create Controller
            var controller = new CustomersController(context);

            // Add Customer
            await controller.Add(new Customer { CustomerName = "Fox Mulder" });

            // Check: Does GetAll return the added Customer?
            //var orgRes = await controller.GetAll();
            var actionResult = (OkObjectResult)await controller.GetAll();
            var result = ((IEnumerable<Customer>)actionResult.Value).ToArray();
            Assert.Single(result);
            Assert.Equal("Fox Mulder", result[0].CustomerName);
        }
    }
}
