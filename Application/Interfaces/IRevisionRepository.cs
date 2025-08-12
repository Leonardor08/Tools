using Domain.Models;

namespace Application.Interfaces
{
    public interface IRevisionRepository
    {
        Task<Revision> GetRevisionsByWorkItem(string organization, string project, string workItemId, CancellationToken cancellationToken);    
    }
}
