using System.Net;
using System.Text;
using IntegrationTest.TestData;
using Newtonsoft.Json;
using TestWithEF.Extensions;

namespace IntegrationTest.Authors;

public class CreateAuthorApiTest : BaseTestFixture
{
    [Test]
    public async Task CreateAuthor_SuccessOK()
    {
        // Arrange
        var createAuthor =DataGenerator.CreateAuthorRequest()
            .Generate();

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
