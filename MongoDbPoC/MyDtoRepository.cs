using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbPoC
{
    internal class MyDtoRepository : MongoDbRepository<MyDto>
    {
        public override Dictionary<string, string> Indexes => new()
        {
            { "Locator", "LocatorAscIndex" },
            { "Name", "NameAscIndex" },
            { "DateOfBirth", "DateOfBirthAscIndex" },
            { "IsActive", "IsActiveAscIndex" }
        };

        // Should always be created with the parameters informed via DI. The default values should not exists.
        public MyDtoRepository(string host = "mongodb://localhost:3017", string database = "MongoDbLab", string collection = "MyDtoCollection") : base(host, database, collection)
        {

        }
    }
}
