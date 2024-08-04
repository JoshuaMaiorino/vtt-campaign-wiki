using System.Linq.Expressions;

namespace vtt_campaign_wiki.Server.Features.Shared.Services
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> GetByIdAsync( int id );
        Task<IEnumerable<T>> GetAllAsync( Expression<Func<T, bool>> filter = null );
        Task<(IEnumerable<T> Items, int ItemsLength)> GetAllAsync( PaginationParameter options, Expression<Func<T, bool>> filter = null );
        Task AddAsync( T entity );
        Task UpdateAsync( T entity );
        Task DeleteAsync( int id );
    }
}
