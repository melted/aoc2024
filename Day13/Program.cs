using System.Text.RegularExpressions;

string[] data = File.ReadAllText("D:/Niklas/repos/aoc2024/data/input13.txt")
    .Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    
string[] testData = """
                    Button A: X+94, Y+34
                    Button B: X+22, Y+67
                    Prize: X=8400, Y=5400
                    
                    Button A: X+26, Y+66
                    Button B: X+67, Y+21
                    Prize: X=12748, Y=12176
                    
                    Button A: X+17, Y+86
                    Button B: X+84, Y+37
                    Prize: X=7870, Y=6450
                    
                    Button A: X+69, Y+23
                    Button B: X+27, Y+71
                    Prize: X=18641, Y=10279
                    """.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine(Solve(testData, false));
Console.WriteLine(Solve(data, false));
Console.WriteLine(Solve(testData, true));
Console.WriteLine(Solve(data, true));

long Solve(string[] data, bool part2)
{
    var entries = ParseData(data);
    if (part2)
    {
        long offset = 10000000000000;
        entries = entries.Select(e => e with { Prize = (e.Prize.x + offset, e.Prize.y + offset) }).ToList();
    }
    long sum = 0;
    foreach (var e in entries)
    {
        if (e.Solve(out var solution))
        {
            sum += solution.Item1 * 3;
            sum += solution.Item2;
        }
    }

    return sum;
}

List<Entry> ParseData(string[] data)
{
    string pattern = """
                     Button A: X\+(\d+), Y\+(\d+)
                     Button B: X\+(\d+), Y\+(\d+)
                     Prize: X=(\d+), Y=(\d+)
                     """;
    Regex r = new(pattern, RegexOptions.Multiline);
    List<Entry> result = [];
    foreach (var field in data)
    {
        var m = r.Match(field);
        if (m.Success)
        {
            result.Add(new Entry((Int64.Parse(m.Groups[1].Value), Int64.Parse(m.Groups[2].Value)),
                (Int64.Parse(m.Groups[3].Value), Int64.Parse(m.Groups[4].Value)),
                (Int64.Parse(m.Groups[5].Value), Int64.Parse(m.Groups[6].Value))));
        }
    }

    return result;
}


record Entry((long x, long y) ButtonA, (long x, long y) ButtonB, (long x, long y) Prize)
{
    public bool Solve(out (long, long) solution)
    {
        // We have two unknowns and two equations, eliminate ButtonB by multiplying each
        // equation by the number of buttonBs in the other and subtract them.
        long lhs = ButtonA.x * ButtonB.y - ButtonA.y * ButtonB.x;
        long rhs = Prize.x * ButtonB.y - Prize.y * ButtonB.x;
        if (lhs != 0 && rhs % lhs == 0)
        {
            long a = rhs / lhs;
            long bx = Prize.x - a * ButtonA.x;
            if (bx % ButtonB.x == 0)
            {
                long b = bx / ButtonB.x;
                solution = (a, b);
                return true;
            }
        }

        solution = (0, 0);
        return false;
    }
}