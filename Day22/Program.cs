var data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input22.txt");

var testData = """
               1
               10
               100
               2024 
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

List<int> ParseData(string[] data)
{
    return data.Select(x => int.Parse(x)).ToList();
}

long OneRound(long n)
{
    int c = 16777216;
    var x = ((n * 64) ^ n) % c;
    var y = ((x / 32) ^ x) % c;
    var z = ((y * 2048) ^ y) % c;
    return z;
}

long Next(long n, int times)
{
    long val = n;
    for (int i = 0; i < times; i++)
    {
        val = OneRound(val);
    }
    return val;
}

List<int> Prices(long n, out Dictionary<(int, int, int, int), int> deltas)
{
    List<int> result = [(int)n % 10];
    deltas = new();
    for (int i = 0; i < 2000; i++)
    {
        n = OneRound(n);
        int x = (int)n % 10;
        if (i > 2)
        {
            var steps = (result[i-2] - result[i-3], result[i-1] - result[i-2],
                                 result[i] - result[i-1], x - result[i]);
            if (!deltas.ContainsKey(steps))
            {
                deltas.Add(steps, x);
            }
        }
        result.Add(x);
    }

    return result;
}

long Solve(string[] data)
{
    var nums = ParseData(data);
    return nums.Select(x => Next(x, 2000)).Sum();
}

long Solve2(string[] data)
{
    var nums = ParseData(data);
    Dictionary<(int, int, int, int), long> bananas = new();
    foreach (var n in nums)
    {
        var p = Prices(n, out var deltas);
        foreach (var (seq, val) in deltas)
        {
            bananas.TryGetValue(seq, out var old);
            bananas[seq] = old + val;
        }
    }

    return bananas.Values.Max();
}

Console.WriteLine(Solve(testData));
Console.WriteLine(Solve(data));
Console.WriteLine(Solve2(["1","2", "3", "2024"]));
Console.WriteLine(Solve2(data));