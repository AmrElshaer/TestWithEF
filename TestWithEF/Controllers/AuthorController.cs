using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Threading.Channels;
using TestWithEF.Channels;
using TestWithEF.Entities;
using TestWithEF.Models;

namespace TestWithEF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly TestContext context;
        private readonly Channel<SendEmailChannel> sendEmailChannel;
        private readonly Channel<UserUpdatedChannel> userUpdateChannel;

        public AuthorController(TestContext context,
            Channel<SendEmailChannel> sendEmailChannel,
            Channel<UserUpdatedChannel> userUpdateChannel)
        {
            this.context = context;
            this.sendEmailChannel = sendEmailChannel;
            this.userUpdateChannel = userUpdateChannel;
        }
        [HttpGet]
        public async Task<ActionResult> GetAuthors()
        {
            var authors = await context.Authors.AsNoTracking().ToListAsync();
            return Ok(authors);
        }
        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateAuthor createAuthor)
        {
            var addressResult = Address.CreateAddress(createAuthor.Street, createAuthor.City, createAuthor.Postcode, createAuthor.Country);
            if (addressResult.IsFailure) return BadRequest(addressResult.Error);
            var contactDetailsResult = ContactDetails.CreateContactDetails(createAuthor.Phone, addressResult.Value);
            var authorName= AuthorName.CreateAuthorName(createAuthor.Name);
            var res= Result.Combine(authorName, contactDetailsResult);
            if (res.IsFailure) return BadRequest(res.Error);
            var author = Author.CreateAuthor(authorName.Value, contactDetailsResult.Value);
            await context.Authors.AddAsync(author);
            await context.SaveChangesAsync();
            await  sendEmailChannel.Writer.WriteAsync(new SendEmailChannel
            {
                Email = createAuthor.Postcode,
                Name = createAuthor.Name
            });
          
            return Ok(author);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(UpdateAuthor updateAuthor)
        {
            var addressResult = Address.CreateAddress(updateAuthor.Street, updateAuthor.City, updateAuthor.Postcode, updateAuthor.Country);
            if (addressResult.IsFailure) return BadRequest(addressResult.Error);
            var contactDetailsResult = ContactDetails.CreateContactDetails(updateAuthor.Phone, addressResult.Value);
            var authorName = AuthorName.CreateAuthorName(updateAuthor.Name);
            var res = Result.Combine(authorName, contactDetailsResult);
            if (res.IsFailure) return BadRequest(res.Error);
            var author = await context.Authors.FirstOrDefaultAsync(a=>a.Id==updateAuthor.Id);
            if (author == null) return NotFound();
            author= author.UpdateAuthor(authorName.Value,contactDetailsResult.Value);
            await context.SaveChangesAsync();
            await userUpdateChannel.Writer.WriteAsync(new UserUpdatedChannel
            {
                Name = author.Name
            });
            return Ok(author);
        }
    }
}
