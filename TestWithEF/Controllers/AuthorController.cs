using System.Threading.Channels;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using TestWithEF.Channels;
using TestWithEF.Dtos;
using TestWithEF.Entities;
using TestWithEF.IRepositories;
using TestWithEF.Models;
using TestWithEF.Specifications;

namespace TestWithEF.Controllers
{
    public class AuthorController : ApiControllerBase
    {
        private readonly IAuthorRepository _authorRepo;
        private readonly Channel<SendEmailChannel> sendEmailChannel;
        private readonly Channel<UserUpdatedChannel> userUpdateChannel;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController
        (
            IAuthorRepository context,
            Channel<SendEmailChannel> sendEmailChannel,
            Channel<UserUpdatedChannel> userUpdateChannel,
            ILogger<AuthorController> logger
        )
        {
            this._authorRepo = context;
            this.sendEmailChannel = sendEmailChannel;
            this.userUpdateChannel = userUpdateChannel;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAuthors()
        {
            var authors = (await _authorRepo.GetAllAsync()).Select(a=>{AuthorDto authorDto = a; return authorDto;});
            _logger.LogInformation("Get all authors {0}", authors.Count());

            return Ok(authors);
        }

        [HttpGet]
        [Route("GetAuthorsByName")]
        public async Task<ActionResult> GetAuthorsByName(string name)
        {
            var spec = new AuthorBySpecification(name);
            var authors = await _authorRepo.GetAsync(spec);

            return Ok(authors);
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateAuthor createAuthor)
        {
            var addressResult = Address.CreateAddress(createAuthor.Street, createAuthor.City, createAuthor.Postcode, createAuthor.Country);

            if (addressResult.IsFailure)
                return BadRequest(addressResult.Error);

            var contactDetailsResult = ContactDetails.CreateContactDetails(createAuthor.Phone, addressResult.Value);
            var authorName = AuthorName.CreateAuthorName(createAuthor.Name);
            var res = Result.Combine(authorName, contactDetailsResult);

            if (res.IsFailure)
                return BadRequest(res.Error);

            var author = Author.CreateAuthor(authorName.Value, contactDetailsResult.Value);
            await _authorRepo.AddAsync(author);
            _logger.LogInformation("Create author Name :{@Author} ", author);

            await sendEmailChannel.Writer.WriteAsync(new SendEmailChannel
            {
                Email = createAuthor.Postcode,
                Name = createAuthor.Name
            });

            return Ok(author.Id);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Author>> GetUser(Guid id)
        {
            var author = await _authorRepo.GetByIdAsync(id);

            if (author == null)
                return NotFound();

            return Ok(author);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateUser(Guid id, UpdateAuthor updateAuthor)
        {
            var addressResult = Address.CreateAddress(updateAuthor.Street, updateAuthor.City, updateAuthor.Postcode, updateAuthor.Country);

            if (addressResult.IsFailure)
                return BadRequest(addressResult.Error);

            var contactDetailsResult = ContactDetails.CreateContactDetails(updateAuthor.Phone, addressResult.Value);
            var authorName = AuthorName.CreateAuthorName(updateAuthor.Name);
            var res = Result.Combine(authorName, contactDetailsResult);

            if (res.IsFailure)
                return BadRequest(res.Error);

            var author = await _authorRepo.GetByIdAsync(id);

            if (author == null)
                return NotFound();

            author = author.UpdateAuthor(authorName.Value, contactDetailsResult.Value);
            await _authorRepo.UpdateAsync(author);

            await userUpdateChannel.Writer.WriteAsync(new UserUpdatedChannel
            {
                Name = author.Name
            });

            return Ok(updateAuthor);
        }
    }
}
