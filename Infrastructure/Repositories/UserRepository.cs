using System.Text.Json;
using Infrastructure.Models;
using Microsoft.Extensions.Options;
using X.PagedList;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private List<User> _users;

        public UserRepository(IOptions<DataOptions> dataOptions)
        {
            // preload the set of users from file.
            var json = System.IO.File.ReadAllText(dataOptions.Value.JsonFilePath);
            _users = JsonSerializer.Deserialize<IEnumerable<User>>(json)
                .ToList();
        }

        public async Task<IEnumerable<User>> QueryAll()
        {
            return _users;
        }

        public async Task<IPagedList<User>> QueryAll(int pageNumber, int pageSize)
        {
            return _users.ToPagedList(pageNumber, pageSize);
        }

        public void Remove(long id)
        {
            var user = _users.Find(u => u.Id == id);
            _users.Remove(user);
        }

    }
}
