using System.Threading.Channels;
using CSharpFunctionalExtensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IAuthorRepository authorRepository;
        private readonly TestContext _context;
        private readonly Channel<SendEmailChannel> sendEmailChannel;
        private readonly Channel<UserUpdatedChannel> userUpdateChannel;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController
        (
            TestContext context,
            Channel<SendEmailChannel> sendEmailChannel,
            Channel<UserUpdatedChannel> userUpdateChannel,
            ILogger<AuthorController> logger,
            IAuthorRepository authorRepository
        )
        {
            this._context = context;
            this.sendEmailChannel = sendEmailChannel;
            this.userUpdateChannel = userUpdateChannel;
            _logger = logger;
            this.authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAuthors()
        {
            var authors = await _context.Authors.ProjectToType<AuthorDto>().ToListAsync();
            _logger.LogInformation("Get all authors {0}", authors.Count());

            return Ok(authors);
        }

        [HttpGet]
        [Route("GetAuthorsByName")]
        public async Task<ActionResult> GetAuthorsByName(string name)
        {
            var spec = new AuthorBySpecification(name);
            var authors = await authorRepository.GetAsync(spec);

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
            await _context.AddAsync(author);
            await _context.SaveChangesAsync();
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
            var author = await authorRepository.GetByIdAsync(id);

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

            var author = await authorRepository.GetByIdAsync(id);

            if (author == null)
                return NotFound();

            author = author.UpdateAuthor(authorName.Value, contactDetailsResult.Value);
            authorRepository.Update(author);
            await _context.SaveChangesAsync();

            await userUpdateChannel.Writer.WriteAsync(new UserUpdatedChannel
            {
                Name = author.Name
            });

            return Ok(updateAuthor);
        }
    }
}
