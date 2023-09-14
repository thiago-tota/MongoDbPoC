using System.Diagnostics;

namespace MongoDbPoC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            var totalRecords = 1000;

            Console.WriteLine("Welcome to MongoDb PoC!");
            var records = MyDto.GenerateRandomDTOs(totalRecords);
            var repo = new MyDtoRepository();

            Console.WriteLine($"Checking database indexes");
            stopwatch.Restart();
            repo.CheckIndexes().Wait();
            stopwatch.Stop();
            Console.WriteLine($"Finished checking database indexes {stopwatch.Elapsed}");

            Console.WriteLine($"Inserting total of {totalRecords} records");
            stopwatch.Restart();
            repo.Save(records);
            stopwatch.Stop();
            Console.WriteLine($"Finished inserting total of {totalRecords} records in {stopwatch.Elapsed}");

            Console.WriteLine($"Searching by Locator 8115749");
            stopwatch.Restart();
            repo.SearchByField("Locator", 8115749);
            stopwatch.Stop();
            Console.WriteLine($"Finished searching by Locator 8115749 in {stopwatch.Elapsed}");

            Console.WriteLine($"Searching by Locator 6439343");
            stopwatch.Restart();
            repo.SearchByField("Locator", 6439343);
            stopwatch.Stop();
            Console.WriteLine($"Finished searching by Locator 6439343 in {stopwatch.Elapsed}");

            Console.WriteLine($"Searching by Locator 9995593");
            stopwatch.Restart();
            repo.SearchByField("Locator", 9995593);
            stopwatch.Stop();
            Console.WriteLine($"Finished searching by Locator 9995593 in {stopwatch.Elapsed}");

            Console.WriteLine($"Searching by Locator 7325121");
            stopwatch.Restart();
            repo.SearchByField("Locator", 7325121);
            stopwatch.Stop();
            Console.WriteLine($"Finished searching by Locator 7325121 in {stopwatch.Elapsed}");

            Console.WriteLine("End!");
        }
    }
}