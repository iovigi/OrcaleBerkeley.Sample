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
            database.Clear();

            Console.WriteLine(database.ContainsKey(2));
            database[1] = 3;
            Console.WriteLine(database[1]);
            Console.WriteLine(database.Count);
        }
    }
}
