using Microsoft.Extensions.Options;
using System.Linq;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Xunit;

namespace SuperPanel.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public void QueryAll_ShouldReturnEverything()
        {
            var r = new UserRepository(Options.Create<DataOptions>(new DataOptions()
            {
                JsonFilePath = "./../../../../data/users.json"
            }));

            var all = r.QueryAll();

            Assert.Equal(5000, all.Count());
        }
    }
}
