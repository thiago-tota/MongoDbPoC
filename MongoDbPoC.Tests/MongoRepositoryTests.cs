using System.Linq.Expressions;
using FluentAssertions;
using MongoDB.Driver;
using MongoDbPoC.Data.Repository;

namespace MongoDbPoC.Tests
{
    public class MongoRepositoryTests : IClassFixture<MongoRepositoryFixture>
    {
        private readonly IMongoRepository<TestEntity> _repository;

        public MongoRepositoryTests(MongoRepositoryFixture mongoRepositoryFixture)
        {
            var configuration = mongoRepositoryFixture.GetConfiguration();

            var host = configuration.GetSection("MongoSettings").GetSection("Host").Value;
            var databaseName = configuration.GetSection("MongoSettings").GetSection("DatabaseName").Value;

            _repository = new MongoRepository<TestEntity>(host!, databaseName!, TestEntity.GetCollectionName, TestEntity.GetIndexes);
            mongoRepositoryFixture.ResetDb(host!, databaseName!, TestEntity.GetCollectionName);
        }

        [Fact]
        public async Task CreateMany()
        {
            var records = TestEntity.GenerateRandomDTOs(1000);
            await _repository.CreateAsync(records);
        }

        [Fact]
        public async Task CreateOne()
        {
            var dto = TestEntity.GenerateRandomDTOs(1).First();
            await _repository.CreateAsync(dto);
        }

        [Fact]
        public async Task GetAll()
        {
            await CreateMany();
            await Task.Delay(200);

            var response = await _repository.GetAllAsync();
            response.Should().HaveCountGreaterThanOrEqualTo(1000);
        }

        [Fact]
        public async Task GetById()
        {
            var record = await CreateNewRecord();
            await Task.Delay(200);

            var response = await _repository.GetByIdAsync(record.Id);
            response.Should().NotBeNull();
        }

        [Theory]
        [InlineData("Locator")]
        [InlineData("Name")]
        public async Task GetByField(string field)
        {
            var record = await CreateNewRecord();
            await Task.Delay(200);

            var value = record.GetType().GetProperty(field)?.GetValue(record);
            var param = Expression.Parameter(typeof(TestEntity), "name");
            var prop = Expression.Property(param, field);
            var constant = Expression.Constant(value);
            var equal = Expression.Equal(prop, constant);
            var lambda = Expression.Lambda<Func<TestEntity, bool>>(equal, param);

            var response = await _repository.GetByFieldAsync(lambda);

            response.Should().NotBeNull();
            response.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task CheckIndexes()
        {
            await _repository.CheckIndexes();
        }

        [Fact]
        public async Task Update()
        {
            var record = await CreateNewRecord();
            await Task.Delay(200);

            record.Name = "Changed Name";
            var response = await _repository.UpdateAsync(record.Id, record);
            response.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Delete()
        {
            var record = await CreateNewRecord();
            await Task.Delay(200);

            var response = await _repository.DeleteAsync(record.Id);
            response.Should().BeGreaterThan(0);
        }

        private async Task<TestEntity> CreateNewRecord()
        {
            var record = TestEntity.GenerateRandomDTOs(1).Single();
            await _repository.CreateAsync(record);

            return record;
        }
    }
}