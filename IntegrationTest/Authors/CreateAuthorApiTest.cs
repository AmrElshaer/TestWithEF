using System.Net;
using System.Text;
using Newtonsoft.Json;
using TestWithEF.Extensions;
using TestWithEF.Models;

namespace IntegrationTest.Authors;

public class CreateAuthorApiTest : BaseTestFixture
{
    [Test]
    public async Task CreateAuthor_SuccessOK()
    {
        // Arrange
        var createAuthor = new CreateAuthor
        {
            Name = "Test Author",
            Street = "Test Street",
            City = "Test City",
            Postcode = "Test Postcode",
            Country = "Test Country",
            Phone = "Test Phone"
        };

        var jsonContent = JsonConvert.SerializeObject(createAuthor);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        // Act
        var response = await Client.PostAsync("/api/author", httpContent);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var id = stringResponse.FromJson<Guid>();
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        id.Should().NotBe(Guid.Empty);
    }
}
