var lines = File.ReadLines("D:/Niklas/repos/aoc2024/data/input1.txt").Select(ParseLine).ToList();

var testData = """
               3   4
               4   3
               2   5
               1   3
               3   9
               3   3
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(ParseLine).ToList();


Console.WriteLine(Solve1(testData));
Console.WriteLine(Solve2(testData));

Console.WriteLine(Solve1(lines));
Console.WriteLine(Solve2(lines));

(int, int) ParseLine(string line)
{
    var vals = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    var a = int.Parse(vals[0]);
    var b = int.Parse(vals[1]);
    return (a, b);
}

int Solve1(List<(int, int)> input)
{
    var first = input.Select(p => p.Item1).ToArray();
    var second = input.Select(p => p.Item2).ToArray();
    
    Array.Sort(first);
    Array.Sort(second);

    var result = first.Zip(second).Select(p => Math.Abs(p.First - p.Second)).Sum();
    return result;
}

int Solve2(List<(int, int)> input)
{
    var first = input.Select(p => p.Item1);
    var groups = input.GroupBy(n => n.Item2, n => n.Item2, 
        (i, ints) => new KeyValuePair<int, int>(i, ints.Count())).ToDictionary();
    
    var result = first.Select(i =>
    {
        groups.TryGetValue(i, out var count);
        return count * i;
    }).Sum();
    return result;
}
