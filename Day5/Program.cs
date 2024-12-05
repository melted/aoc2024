var text = File.ReadAllText("D:/Niklas/repos/aoc2024/data/input5.txt");

Console.WriteLine(Solve1(ParseData(text)));
Console.WriteLine(Solve2(ParseData(text)));

Problem ParseData(string data)
{
    Dictionary<int, HashSet<int>> rules = new();
    var parts = data.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    var ruleList = parts[0].Split("\n", StringSplitOptions.RemoveEmptyEntries)
        .Select(s =>
        {
            var sp = s.Split("|", StringSplitOptions.RemoveEmptyEntries);
            return (int.Parse(sp[0]), int.Parse(sp[1]));
        });
    foreach (var (a,b) in ruleList)
    {
        if (!rules.ContainsKey(a))
        {
            rules[a] = new HashSet<int>();
        }
        rules[a].Add(b);
    }
    var batches = parts[1].Split("\n", StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()).ToList();
    return new Problem(rules, batches);
}

bool IsValid(Dictionary<int,HashSet<int>> rules, List<int> pages)
{
    HashSet<int> seen = [];
    foreach (var p in pages)
    {
        if (rules.TryGetValue(p, out var val))
        {
            if (val.Overlaps(seen))
            {
                return false;
            }
        }
        seen.Add(p);
    }
    return true;
}

List<int> Fixup(Problem problem, List<int> pages)
{
    List<int> corrected = [];
    for (int i = 0; i < pages.Count; i++)
    {
        bool inserted = false;
        for (int j = 0; j < corrected.Count; j++)
        {
            if(!IsValid(problem.rules, [corrected[j], pages[i]]))
            {
                corrected.Insert(j, pages[i]);
                inserted = true;
                break;
            }
        }
        if (!inserted)
        {
            corrected.Add(pages[i]);
        }
    }
    return corrected;
}

int Solve1(Problem problem)
{
    return problem.pages.Where(p => IsValid(problem.rules, p)).Select(l => l[l.Count / 2]).Sum();
}

int Solve2(Problem problem)
{
    return problem.pages.Where(p => !IsValid(problem.rules, p))
        .Select(l => Fixup(problem, l))
        .Select(l => l[l.Count / 2]).Sum();
}

var test = """
47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47
""";

var prob = ParseData(test);
Console.WriteLine(Solve1(prob));
Console.WriteLine(Solve2(prob));
record Problem(Dictionary<int,HashSet<int>> rules, List<List<int>> pages);