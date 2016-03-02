namespace OracleWrapper.Api.Test
{
    using System;
    using Api;

    public class Program
    {
        public static void Main(string[] args)
        {
            Database<int, int> database = new Database<int, int>("file.oracle");

            database.Add(1, 2);

            Console.WriteLine(database.ContainsKey(2));
            Console.WriteLine(database[1]);
            Console.WriteLine(database.Count);
        }
    }
}
