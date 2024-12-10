using System.Collections.Immutable;

var data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input10.txt");

var testData = """
               89010123
               78121874
               87430965
               96549874
               45678903
               32019012
               01329801
               10456732
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

List<(int x, int y)> directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];

int[,] ParseData(string[] data)
{
    var result = new int[data[0].Length, data.Length];
    for (int y = 0; y < data.Length; y++)
        for (int x = 0; x < data[0].Length; x++)
            result[x, y] = Convert.ToInt32(data[x][y]) - 48;
    return result;
}

Console.WriteLine(Solve(testData));
Console.WriteLine(Solve(data));

(int, int) Solve(string[] data)
{
    var map = ParseData(data);
    var score = 0;
    var rating = 0;
    for (int x = 0; x < map.GetLength(0); x++)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            if (map[x, y] == 0)
            {
                var (rate, rank) = FindTrails(map, (x, y));
                score += rank;
                rating += rate;
            }
        }
    }
    return (score, rating);
}

(int, int) FindTrails(int[,] map, (int x, int y) pos)
{
    ImmutableHashSet<ImmutableList<(int x, int y)>> routes = [[pos]];
    for (int level = 0; level < 9; level++)
    {
        ImmutableHashSet<ImmutableList<(int x, int y)>> next = [];
        foreach (var r in routes)
        {
            var head = r.Last();
            foreach (var step in GetNeighbors(map, head))
            {
                if (map[step.x, step.y] == level + 1)
                {
                    next = next.Add(r.Add(step));
                }
            }
        }
        routes = next;
    }

    var ends = routes.Select(r => r.Last()).ToImmutableHashSet();
    return (routes.Count, ends.Count);
}

ImmutableHashSet<(int x, int y)> GetNeighbors(int[,] map, (int x, int y) pos)
{
    ImmutableHashSet<(int x, int y)> result = [];
    foreach (var d in directions)
    {
        var n = (pos.x + d.x, pos.y + d.y);
        if (Inbounds(map, n))
        {
            result = result.Add(n);
        }
    }
    return result;
}

bool Inbounds(int[,] map, (int x, int y) pos)
{
    return pos.x >= 0 && pos.y >= 0 && pos.x < map.GetLength(0) && pos.y < map.GetLength(1);
}

