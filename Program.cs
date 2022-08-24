using System;

namespace LinkedListSerializator;

class Program
{
    static void Main(string[] args)
    {
        var path = @"Temp.txt";

        var listRand = new ListRand().AddRange(
            new string[]
            {
                "First",
                "Second",
                "Real data",
                "NullReferenceException",
                "[Концептуализация]"
            }
        );
        var otherList = new ListRand();

        using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
        {
            listRand.Serialize(fileStream);
        }

        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            otherList.Deserialize(fileStream);
        }

        var rand = listRand.Head;
        var other = otherList.Head;
        var isWork = true;
        for (int i = 0; i < listRand.Count && isWork; i++)
        {
            isWork = 
                other.Data == rand.Data 
                && other.Prev?.Data == rand.Prev?.Data
                && other.Next?.Data == rand.Next?.Data
                && other.Rand?.Data == rand.Rand?.Data;

            rand = rand.Next;
            other = other.Next;
        }

        Console.WriteLine($"Is Work: {isWork}");
        if (isWork == false)
        {
            Console.WriteLine();
        }
    }
}