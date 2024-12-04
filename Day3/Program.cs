using System.Text.RegularExpressions;

var data = File.ReadAllText("D:/niklas/repos/aoc2024/data/input3.txt");

var testData = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
var testData2 = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";

Console.WriteLine(Solve1(testData));
Console.WriteLine(Solve1(data));
Console.WriteLine(Solve2(testData2));
Console.WriteLine(Solve2(data));

long Solve1(string data)
{
    var insts = ParseData(data);
    return insts.Select(p => p switch { Mul(var x,var y) => x*y, _ => 0L}).Sum();
}

long Solve2(string data)
{
    var insts = ParseData(data);
    long sum = 0;
    bool on = true;
    foreach (Inst inst in insts)
    {
        switch (inst)
        {
            case Mul(var x, var y):
                if (on) sum += x * y;
                break;
            case On:
                on = true;
                break;
            case Off:
                on = false;
                break;
        }
    }
    return sum;
}

List<Inst> ParseData(string text)
{
    var regex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|don\'t\(\)|do\(\)");
    var result = new List<Inst>();
    foreach (Match match in regex.Matches(text))
    {
        Inst inst = match.Value.Split("(")[0] switch
        {
            "mul" => new Mul(Int64.Parse(match.Groups[1].Value), Int64.Parse(match.Groups[2].Value)),
            "do" => new On(),
            "don't" => new Off(),
            _ => throw new ArgumentException()
        };
        result.Add(inst);
    }
    return result;
}

record Inst();

record Mul(long x, long y) : Inst;
record On() : Inst;
record Off() : Inst;