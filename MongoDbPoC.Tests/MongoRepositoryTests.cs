using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MongoDbPoC.Data;
using MongoDbPoC.Data.Repository;

namespace MongoDbPoC.Tests
{
    public class MongoRepositoryTests
    {
        private readonly IMongoRepository<MyDto> _repository;

        public MongoRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            var host = configuration.GetSection("MongoSettings").GetSection("MongoDBHost").Value;
            var dbName = configuration.GetSection("MongoSettings").GetSection("MongoDBName").Value;

            _repository = new MongoRepository<MyDto>(host!, dbName!, MyDto.GetCollectionName, MyDto.GetIndexes);
        }

        [Fact]
        public async Task CreateMany()
        {
            var records = MyDto.GenerateRandomDTOs(1000);
            await _repository.CreateAsync(records);
        }

        [Fact]
        public async Task CreateOne()
        {
            var dto = MyDto.GenerateRandomDTOs(1).First();
            await _repository.CreateAsync(dto);
        }

        [Fact]
        public async Task GetAll()
        {
            var response = await _repository.GetAllAsync();
            response.Should().HaveCountGreaterThanOrEqualTo(1);
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

            var response = await _repository.GetByFieldAsync(field, value!);

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

        private async Task<MyDto> CreateNewRecord()
        {
            var record = MyDto.GenerateRandomDTOs(1).Single();
            await _repository.CreateAsync(record);

            return record;
        }
    }
}