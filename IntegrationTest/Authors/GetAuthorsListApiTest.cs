using System.Net;
using System.Text;
using IntegrationTest.TestData;
using Newtonsoft.Json;
using TestWithEF.Dtos;
using TestWithEF.Extensions;
using TestWithEF.Models;

namespace IntegrationTest.Authors;

public class GetAuthorsListApiTest : BaseTestFixture
{
    [Test]
    public async Task GetAuthors_EmptyAuthors_SuccessOK()
    {
        // Act
        var response = await Client.GetAsync("/api/author");
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<AuthorDto[]>();
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        model.Should().NotBeNull();
        model.Should().BeOfType<AuthorDto[]>();
    }

    [Test]
    public async Task GetAuthors_HaveAuthors_SuccessOK()
    {
        // Arrange
        var createAuthor = DataGenerator.CreateAuthorRequest()
            .Generate();

        var jsonContent = JsonConvert.SerializeObject(createAuthor);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        await Client.PostAsync("/api/author", httpContent);
        // Act
        var response = await Client.GetAsync("/api/author");
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<AuthorDto[]>();
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        model.Should().NotBeNull();
        model.Should().BeOfType<AuthorDto[]>();
        model.Should().HaveCount(1);
        model!.First().Name.Should().Be(createAuthor.Name);
    }
}
