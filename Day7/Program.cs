using System.Text.RegularExpressions;

var lines = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input7.txt");

var testData = """
               190: 10 19
               3267: 81 40 27
               83: 17 5
               156: 15 6
               7290: 6 8 6 15
               161011: 16 10 13
               192: 17 8 14
               21037: 9 7 18 13
               292: 11 6 16 20
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

Equation ParseLine(string line)
{
    var regex = new Regex(@"(\d+):( \d+)+");
    var match = regex.Match(line);
    var total = long.Parse(match.Groups[1].Value);
    var parts = match.Groups[2].Captures.Select(c => long.Parse(c.Value)).ToList();
    return new Equation(parts, total);
}

Console.WriteLine(Solve1(testData, false));
Console.WriteLine(Solve1(lines, false));
Console.WriteLine(Solve1(testData, true));
Console.WriteLine(Solve1(lines, true));

long Solve1(string[] data, bool concat)
{
    return data.Select(ParseLine).Where(eq => eq.Eval(concat)).Select(eq => eq.Total).Sum();
}

record Equation(List<long> Parts, long Total)
{
    public bool Eval(bool concat)
    {
        List<long> results = [Parts[0]];
        int i = 0;
        while (i < Parts.Count - 1)
        {
            var rhs = Parts[i+1];
            results = results.SelectMany(r => GetOps(r, rhs, concat))
                .Where(r => r <= Total).ToList();
            i++;
        }
        return results.Any(n => n == Total);
    }
    
    static List<long> GetOps(long lhs, long rhs, bool concat)
    {
        var result = new List<long> { lhs + rhs, lhs * rhs }; 
        if (concat)
        {
            result.Add(lhs * Magnitude(rhs) + rhs);
        }
        return result;
    }
    
    static long Magnitude(long num)
    {
        var result = 10;
        while (result < num) result *= 10;
        return result;
    }
}