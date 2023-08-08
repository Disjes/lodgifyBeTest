using Infrastructure.Models;
using X.PagedList;

namespace Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User> FindById(int id);
    Task<IEnumerable<User>> QueryAll();
    Task<IPagedList<User>> QueryAll(int pageNumber, int pageSize);
    bool Remove(long id);
}