using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankDirectoryApi.IntegrationTestss
{
    public class BankControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;


        public BankControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetBanks_ShouldReturnSuccess()
        {
            // Act
            var response = await _client.GetAsync("/api/Card/GetAllCardes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            // Assert the content
            string content = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON array
            JsonDocument jsonDocument = JsonDocument.Parse(content);
            JsonElement root = jsonDocument.RootElement;

            // Assert that the array has exactly one element
            root.GetArrayLength().Should().Be(2);
        }
    }

}
