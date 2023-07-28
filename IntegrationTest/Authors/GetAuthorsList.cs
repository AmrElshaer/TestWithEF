using System.Net;
using FluentAssertions;
using TestWithEF.Dtos;
using TestWithEF.Extensions;

namespace IntegrationTest.Authors;

public class GetAuthorsList : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GetAuthorsList(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuthors_EmptyAuthors_SuccessOK()
    {
        // Act
        var response = await _client.GetAsync("/api/author");
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<AuthorDto[]>();
        // Assert
        model.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().BeOfType<AuthorDto>();
    }
}
