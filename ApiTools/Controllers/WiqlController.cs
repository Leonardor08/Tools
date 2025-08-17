using Application.Services.Wiql;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using static Domain.Enums;

namespace ApiTools.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WiqlController(GetQueryById getQueryById, GetQueryByRawWiql getQueryByRawWiql) : ControllerBase
    {
        private readonly GetQueryById _getQueryById = getQueryById;
        private readonly GetQueryByRawWiql _getQueryByRawWiql = getQueryByRawWiql;

        [HttpGet]
        [Route("GetQueryById")]
        public async Task<Query> GetQueryByIdAsync(string organization, string project, string idQuery)
            => await _getQueryById.ObtainQueryResultById(organization, project, idQuery);

        [HttpGet]
        [Route("GetQueryByUser")]
        public async Task<Query> GetQueryByIdAsync([FromQuery] Devs developer, string organization, string project)
            => await _getQueryByRawWiql.ObtainQueryByRawWiql(organization, project, developer);
    }

}
