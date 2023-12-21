using IntegrationTest.TestData;
using TestWithEF.Entities;
using TestWithEF.Features.Authors.Queries.GetAuthors;
using TestWithEF.ValueObjects;
using static IntegrationTest.Testing;
namespace IntegrationTest.Authors;

internal class GetAuthorsList : BaseTestFixture
{
    [Test]
    public async Task GetAuthors_EmptyAuthors_SuccessOK()
    {
        // Arrange
        var getAllAuthorsQuery = new GetAuthorsQuery();
        // Act
        var authors =await SendAsync(getAllAuthorsQuery);
        // Assert
        
        authors.Should().NotBeNull();
        authors.Should().BeOfType<IReadOnlyList<GetAllAuthorsDto>>();
        
    }

    [Test]
    public async Task GetAuthors_HaveAuthors_SuccessOK()
    {
        // Arrange
        var author = DataGenerator.CreateAuthor();
        var getAuthors = new GetAuthorsQuery();
        // Act
        await AddAsync(author);
        var authors = await SendAsync(getAuthors);
        // Assert
        authors.Should().NotBeNull();
        authors.Should().HaveCount(1);
    }
}
