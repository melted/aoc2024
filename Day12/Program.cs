string[] data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input12.txt");

string[] testData = """
                    RRRRIICCFF
                    RRRRIICCCF
                    VVRRRCCFFF
                    VVRCCCJFFF
                    VVVVCJJCFE
                    VVIVCCJJEE
                    VVIIICJJEE
                    MIIIIIJJEE
                    MIIISIJEEE
                    MMMISSJEEE
                    """.Split("\n", StringSplitOptions.RemoveEmptyEntries);


Map ParseData(string[] data)
{
    Dictionary<(int, int), char> d = new();
    for (int y = 0; y < data.Length; y++)
    {
        for (int x = 0; x < data[0].Length; x++)
        {
            d.Add((x,y), data[x][y]);
        }
    }

    return new Map(d);
}

Console.WriteLine(Solve(data));
Console.WriteLine(Solve(testData));

(int, int) Solve(string[] data)
{
    Map map = ParseData(data);
    int sum = 0;
    int sum2 = 0;
    while (map.points.Count > 0)
    {
        var (area, perimeter, sides) = GetOneRegion(map);
        sum += area * perimeter;
        sum2 += area * sides;
    }

    return (sum, sum2);
}

(int, int, int) GetOneRegion(Map map)
{
    HashSet<(int, int)> region = new();
    HashSet<((int, int), int)> edges = [];
    var start = map.points.First();
    HashSet<(int, int)> check = [start.Key];
    int perimeter = 0;
    char plant = start.Value;
    while (check.Count > 0)
    {
        HashSet<(int, int)> next = [];
        foreach (var p in check)
        {
            var tc = map.GetNeighbors(p, plant, out var peri);
            perimeter += 4 - tc.Count;
            foreach (var d in peri)
            {
                edges.Add((p, d));
            }
            next.UnionWith(tc);
        }
        region.UnionWith(check);
        next.ExceptWith(region);
        check = next;
    }

    foreach (var p in region)
    {
        map.points.Remove(p);
    }

    return (region.Count, perimeter, CountSides(edges));
}

int CountSides(HashSet<((int, int), int)> edges)
{
    int sides = 0;
    while (edges.Count > 0)
    {
        var edge = edges.First(); 
        HashSet<((int, int), int)> side = [];
        HashSet<((int, int), int)> check = [edge];
        int direction = edge.Item2;
        List<(int, int)> dirs = direction switch
        {
            0 or 2 => [(0, 1), (0, -1)],
            1 or 3 => [(1, 0), (-1, 0)]
        };
        while (check.Count > 0)
        {
            HashSet<((int, int), int)> next = [];
            foreach (var e in check)
            {
                foreach (var d in dirs)
                {
                    var p = ((e.Item1.Item1 + d.Item1, e.Item1.Item2 + d.Item2), direction);
                    if (edges.Contains(p))
                    {
                        next.Add(p);
                    }
                }

                side.Add(e);
            }
            next.ExceptWith(side);
            check = next;
        }
        edges.ExceptWith(side);
        sides += 1;
    }

    return sides;
}

record Map(Dictionary<(int x, int y), char> points)
{

    List<(int x, int y)> directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];
    public List<(int x, int y)> GetNeighbors((int x, int y) point, char plant, out List<int> perimeter)
    {
        var result = new List<(int x, int y)>();
        perimeter = [];
        foreach (var (i, d) in directions.Index())
        {
            var p = (point.x + d.x, point.y + d.y);
            if (points.TryGetValue(p, out var ch) && ch == plant)
            {
                result.Add(p);
            }
            else
            {
                perimeter.Add(i);
            }
        }

        return result;
    }
}