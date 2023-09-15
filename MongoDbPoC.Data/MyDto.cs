﻿using System.Text.Json.Serialization;

namespace MongoDbPoC.Data
{
    public class MyDto
    {
        public Guid Id { get; set; }
        public int Locator { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public byte[] Image { get; set; } = new byte[0];
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TimeSpan Duration { get; set; }
        public char CharValue { get; set; }
        public short ShortValue { get; set; }
        public long LongValue { get; set; }
        public float FloatValue { get; set; }

        [JsonIgnore]
        public string JsonObject { get; set; } = string.Empty;

        public static string GetCollectionName => "MyDtoCollection";
        public static Dictionary<string, string> GetIndexes => new()
        {
            { "Locator", "LocatorAscIndex" },
            { "Name", "NameAscIndex" },
            { "DateOfBirth", "DateOfBirthAscIndex" },
            { "IsActive", "IsActiveAscIndex" }
        };

        public static List<MyDto> GenerateRandomDTOs(int quantity)
        {
            var random = new Random();
            var dtos = new List<MyDto>();

            for (int i = 0; i < quantity; i++)
            {
                var dto = new MyDto
                {
                    Id = Guid.NewGuid(),
                    Locator = random.Next(1000000, 9999999),
                    Name = Path.GetRandomFileName(),
                    DateOfBirth = DateTime.Now.AddDays(-random.Next(365 * 100)),
                    Salary = (decimal)random.NextDouble() * 10000,
                    IsActive = random.Next(2) == 0,
                    Image = new byte[100],
                    Latitude = random.NextDouble() * 180 - 90,
                    Longitude = random.NextDouble() * 360 - 180,
                    Duration = TimeSpan.FromSeconds(random.NextDouble() * 3600),
                    CharValue = (char)random.Next(65, 91),
                    ShortValue = (short)random.Next(short.MinValue, short.MaxValue),
                    LongValue = random.NextInt64(long.MinValue, long.MaxValue),
                    FloatValue = (float)random.NextDouble() * 10000,
                };

                random.NextBytes(dto.Image);
                dto.JsonObject = dto.ToJson(JsonExtension.GetApplicationDefaultOptions());

                dtos.Add(dto);
            }

            return dtos;
        }
    }
}
