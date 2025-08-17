using Domain.Models;
using static Domain.Enums;

namespace Application.Interfaces;
public interface IWiqlRepository
{
    Task<Query> GetQueryByIdAsync(string organization, string project, string idQuery);
    Task<Query> GetWiqlAsync(string organization, string project, Devs Dev, string wiqlQuery);
}
