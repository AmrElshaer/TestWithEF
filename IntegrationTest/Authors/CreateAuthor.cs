using System.Net;
using System.Text;
using IntegrationTest.TestData;
using Newtonsoft.Json;
using TestWithEF.Entities;
using static IntegrationTest.Testing;
namespace IntegrationTest.Authors;

internal class CreateAuthor : BaseTestFixture
{
    [Test]
    public async Task CreateAuthor_SuccessOK()
    {
        // Arrange
        var createAuthorCommand =DataGenerator.CreateAuthorRequest()
            .Generate();
        // Act
        var authorId = await SendAsync(createAuthorCommand);
        var author = await FindAsync<Author>(authorId.Value);
        // Assert
        author.Should().NotBeNull();
        author!.Name.Should().Be(createAuthorCommand.Name);
        author.Id.Should().Be(authorId.Value);

    }
}
