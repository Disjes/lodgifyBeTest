using Infrastructure.Models;
using X.PagedList;

namespace Infrastructure.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> QueryAll();
    Task<IPagedList<User>> QueryAll(int pageNumber, int pageSize);
    void Remove(long id);
}