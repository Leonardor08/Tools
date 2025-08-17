using Application.Interfaces;
using Domain.Models;

namespace Application.Services.Wiql;

public class GetQueryById(IWiqlRepository wiqlRepository)
{
    private readonly IWiqlRepository _wiqlRepository = wiqlRepository;

    public async Task<Query> ObtainQueryResultById(string organization, string project, string idQuery)
        => await _wiqlRepository.GetQueryByIdAsync(organization, project, idQuery);
}
