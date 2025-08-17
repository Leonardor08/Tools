using Application.DTO;
using Application.Services.Revisions;
using Microsoft.AspNetCore.Mvc;

namespace ApiTools.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevisionController(GetDevelopersInRevisionQueryHandler handler) : ControllerBase
    {
        private readonly GetDevelopersInRevisionQueryHandler _handler = handler;

        [HttpGet]
        [Route("DeveloperListInWorkItem")]
        public async Task<List<AssignedDetailDTO>> GetDevelopers(string organization, string project, string workItemId, CancellationToken cancellationToken)
        {
            List<AssignedDetailDTO> revisions = await _handler.Handle(organization, project, workItemId, cancellationToken);

            return revisions;
        }
    }
}
