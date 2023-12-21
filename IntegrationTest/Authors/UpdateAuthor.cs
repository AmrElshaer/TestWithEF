using IntegrationTest.TestData;
using TestWithEF.Entities;
using static IntegrationTest.Testing;
namespace IntegrationTest.Authors;

internal class UpdateAuthor: BaseTestFixture
{
    [Test]
    public async Task UpdateAuthor_SuccessOK()
    {
        // arrange
        var entity = DataGenerator.CreateAuthor();
        var  updateAuthorRequest = DataGenerator.UpdateAuthorRequest(entity.Id).Generate();
        // act 
        await AddAsync(entity);
        var authorId=await SendAsync(updateAuthorRequest);
        var author = await FindAsync<Author>(entity.Id);
        // Assert
        author.Should().NotBeNull();
        author!.Name.Should().Be(updateAuthorRequest.Name);
        author.Id.Should().Be(authorId.Value);

    }
}