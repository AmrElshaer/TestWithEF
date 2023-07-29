using System.Net;
using TestWithEF.Dtos;
using TestWithEF.Extensions;
using static IntegrationTest.Testing;

namespace IntegrationTest.Authors;

public class GetAuthorsList : BaseTestFixture
{
    [Test]
    public async Task GetAuthors_SuccessOK()
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
}
