using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestWithEF.Dtos;
using TestWithEF.Extensions;

namespace TestWithEf.IntegrationTest.Authors;

[TestClass]
public class GetAuthorsList
{ 
    [TestMethod]
    public async Task GetAuthors_Success()
    {
        // Arrange
        var client = ProgramTest.NewClient;
        // Act
        var response = await client.GetAsync("/api/author");
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<AuthorDto[]>();
        // Assert
        Assert.IsNotNull(model);
        Assert.AreEqual(response.StatusCode, response.StatusCode);
    }
}
