using System.Collections.Immutable;

var data = File.ReadAllText("D:/Niklas/repos/aoc2024/data/input19.txt");

var testData = """
               r, wr, b, g, bwu, rb, gb, br
               
               brwrr
               bggr
               gbbr
               rrbgbr
               ubwu
               bwurrg
               brgr
               bbrgwb 
               """;

Console.WriteLine(Solve(testData));
Console.WriteLine(Solve(data));

(int, long) Solve(string data)
{
    Onsen onsen = ParseData(data);
    int sum = 0;
    long ways = 0;
    foreach (var design in onsen.Designs)
    {
        ImmutableDictionary<string,long> prefixes = ImmutableDictionary<string, long>.Empty.Add("", 1);
        long localSum = 0;
        while (prefixes.Count > 0)
        {
            ImmutableDictionary<string, long> nextSet = ImmutableDictionary<string, long>.Empty;
            foreach (var (prefix, count) in prefixes)
            {
                if (prefix == design)
                {
                    localSum += count;
                    continue;
                }

                string remaining = design.Substring(prefix.Length);
                for (int i = 1; i <= onsen.MaxStripes; i++)
                {
                    if (i <= remaining.Length)
                    {
                        var next = remaining.Substring(0, i);
                        if (onsen.Towels.Contains(next))
                        {
                            var s = string.Concat(prefix, next);
                            nextSet.TryGetValue(s, out var n);
                            nextSet = nextSet.SetItem(s, count + n);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            prefixes = nextSet;
        }
        
        if (localSum > 0)
        {
            sum += 1;
            ways += localSum;
            Console.WriteLine(localSum);
        }
    }

    return (sum, ways);
}

Onsen ParseData(string data)
{
    var parts = data.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    ImmutableHashSet<string> towels = parts[0].Split(",", StringSplitOptions.TrimEntries).ToImmutableHashSet();
    List<string> designs = parts[1].Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();
    int maxStripes = towels.Select(t => t.Length).Max();
    return new Onsen(towels, designs, maxStripes);
}

record Onsen(ImmutableHashSet<string> Towels, List<string> Designs, int MaxStripes);