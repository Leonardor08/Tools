using Application.Interfaces;
using Application.Shared;
using Application.Shared.Resources;
using Domain.Models;
using static Domain.Enums;

namespace Application.Services.Wiql
{
    public class GetQueryByRawWiql(IWiqlRepository wiqlRepository)
    {
        private readonly IWiqlRepository _wiqlRepository = wiqlRepository;

        public async Task<Query> ObtainQueryByRawWiql(string organization, string project, Devs dev)
        {
            string rawWiql = string.Format(DynamicRawWiql.UserHistoryByDev, project, dev.GetAttributeMemberValue());

            return await _wiqlRepository.GetWiqlAsync(organization, project, dev, rawWiql);
        }
    }
}
