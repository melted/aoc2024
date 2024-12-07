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

var eq = new Equation([6, 8, 6, 15], 7290);
var s = eq.Eval([Operator.Multiply, Operator.Concat, Operator.Multiply]);
Console.WriteLine(Solve1(testData, false));
Console.WriteLine(Solve1(lines, false));
Console.WriteLine(Solve1(testData, true));
Console.WriteLine(Solve1(lines, true));

long Solve1(string[] data, bool concat)
{
    return data.Select(ParseLine).Where(eq => IsCorrect(eq, concat)).Select(eq => eq.Total).Sum();
}

bool IsCorrect(Equation eq, bool concat)
{
    foreach (var ops in AllCombinations(eq.Parts.Count - 1, concat))
    {
        if (eq.Eval(ops) == eq.Total)
        {
            return true;
        }
    }
    return false;
}

List<List<Operator>> AllCombinations(int len, bool concat)
{
    if (len == 0)
    {
        return [[]];
    } 
    else
    {
        var heads = AllCombinations(len - 1, concat);
        List<List<Operator>> result = [];
        foreach (var head in heads)
        {
            List<Operator> add = new(head);
            add.Add(Operator.Add);            
            result.Add(add);
            List<Operator> mul = new(head);
            mul.Add(Operator.Multiply);            
            result.Add(mul);
            if (concat)
            {
                List<Operator> cc = new(head);
                cc.Add(Operator.Concat);
                result.Add(cc);
            }
        }
        return result;
    }    
}



enum Operator
{
    Add,
    Multiply,
    Concat
}

record Equation(List<long> Parts, long Total)
{
    public long Eval(List<Operator> ops)
    {
        long result = Parts[0];
        int i = 0;
        while (i < Parts.Count - 1)
        {
            var op = ops[i];
            var rhs = Parts[i+1];
            result = op switch
            {
                Operator.Add => result + rhs,
                Operator.Multiply => result * rhs,
                Operator.Concat => result * Magnitude(rhs) + rhs,
                _ => throw new ArgumentException()
            };
            i++;
        }
        return result;
    }
    
    public static long Magnitude(long num)
    {
        var result = 10;
        while (result < num) result *= 10;
        return result;
    }
}