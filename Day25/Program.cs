string[] data = File.ReadAllText("/home/niklas/repos/aoc2024/data/input25.txt")
                    .Split("\n\n",StringSplitOptions.RemoveEmptyEntries);

string[] testData = """
#####
.####
.####
.####
.#.#.
.#...
.....

#####
##.##
.#.##
...##
...#.
...#.
.....

.....
#....
#....
#...#
#.#.#
#.###
#####

.....
.....
#.#..
###..
###.#
###.#
#####

.....
.....
.....
#....
#.#..
#.#.#
#####
""".Split("\n\n",StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine(Solve(testData));
Console.WriteLine(Solve(data));

Problem ParseData(string[] data)
{
    List<List<int>> locks = [];
    List<List<int>> keys = [];
    var groups = data.Select(s => s.Split("\n", StringSplitOptions.RemoveEmptyEntries))
                     .GroupBy(x => x[0].All(c => c == '.'), (isKey, entries) => new 
                     { IsKey = isKey, Entries = entries });
    foreach (var group in groups)
    {
        if (group.IsKey)
        {
            foreach (var key in group.Entries)
            {
                var cols = Enumerable.Range(0, key[0].Length).Select(i => key.Select(s => s[i]))
                            .Select(cs => cs.SkipWhile(c => c == '.').Count()).ToList();
                keys.Add(cols);
            }
            
        }
        else
        {
            foreach (var entry in group.Entries)
            {
                var cols = Enumerable.Range(0, entry[0].Length).Select(i => entry.Select(s => s[i]))
                            .Select(cs => cs.TakeWhile(c => c == '#').Count()).ToList();
                locks.Add(cols);
            }
        }
    }
    return new Problem(locks, keys);
}

int Solve(string[] data)
{
    var problem = ParseData(data);
    int sum = 0;
    foreach (var k in problem.keys) 
    {
        foreach (var l in problem.locks)
        {
            if (k.Zip(l, (a, b) => a+b).All(x => x <= 7))
            {
                sum += 1;
            }
        }
    }
    return sum;
}
record Problem(List<List<int>> locks, List<List<int>> keys);