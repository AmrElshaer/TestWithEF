using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestWithEF.Features.Authors.Commands.CreateAuthor;
using TestWithEF.Features.Authors.Commands.UpdateAuthor;
using TestWithEF.Features.Authors.Queries.GetAuthorById;
using TestWithEF.Features.Authors.Queries.GetAuthors;

namespace TestWithEF.Controllers
{
    public class AuthorController : ApiControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAuthors()
            => Ok(await Mediator.Send(new GetAuthorsQuery()));

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateAuthorCommand command)
            => await Mediator.Send(command).ToActionResult();

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUser(Guid id)
            => await Mediator.Send(new GetAuthorByIdQuery(id)).ToActionResult();

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateAuthorCommand updateAuthor)
            => await Mediator.Send(updateAuthor with
            {
                Id = id
            }).ToActionResult();
    }
}
