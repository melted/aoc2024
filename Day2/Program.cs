var lines = File.ReadLines("D:/Niklas/repos/aoc2024/data/input2.txt").Select(ParseLine).ToList();

var testData = """
               7 6 4 2 1
               1 2 7 8 9
               9 7 6 2 1
               1 3 2 4 5
               8 6 4 4 1
               1 3 6 7 9
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(ParseLine).ToList();

Console.WriteLine(testData.Where(IsSafe).Count());

Console.WriteLine(lines.Where(IsSafe).Count());

Console.WriteLine(testData.Where(HandleOneProblem).Count());

Console.WriteLine(lines.Where(HandleOneProblem).Count());

List<int> ParseLine(string line) => line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

bool IsSafe(List<int> levels)
{
    var deltas = levels.Zip(levels.Skip(1)).Select(p => p.Second - p.First).ToList();
    var sign = Math.Sign(deltas[0]);
    return deltas.All(n => Math.Sign(n) == sign) && deltas.All(n => Math.Abs(n) > 0 && Math.Abs(n) < 4);
}

bool HandleOneProblem(List<int> levels)
{
    if (!IsSafe(levels))
    {
        for (int i = 0; i < levels.Count; i++)
        {
            var clip = new List<int>(levels);
            clip.RemoveAt(i);
            if (IsSafe(clip)) return true;
        }
        return false;
    }
    return true;
}