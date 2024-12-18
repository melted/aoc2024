var data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input18.txt");

var testData = """
               5,4
               4,2
               4,5
               3,0
               2,1
               6,3
               2,4
               1,5
               0,6
               3,3
               2,6
               5,1
               1,2
               5,5
               2,5
               6,5
               1,4
               0,4
               6,4
               1,1
               6,1
               1,0
               0,5
               1,6
               2,0
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine(Solve1(testData, (7, 7), 12));
Console.WriteLine(Solve1(data, (71, 71), 1024));
Console.WriteLine(Solve2(testData, (7, 7), 12));
Console.WriteLine(Solve2(data, (71, 71), 1024));

int Solve1(string[] data, (int x, int y) dimensions, int nRead)
{
    Map map = ParseData(data, dimensions, nRead);
    return Solve(map);
}

(int, int) Solve2(string[] data, (int x, int y) dimensions, int nRead)
{
    Map map = ParseData(data, dimensions, nRead);
    foreach (var p in map.Extra)
    {
        map.Blocks.Add(p);
        var res = Solve(map);
        if (res < 0)
        {
            return p;
        }
    }

    return (0, 0);
}

int Solve(Map map) 
{
    HashSet<(int x, int y)> seen = [];
    (int, int) end = (map.Dimensions.x - 1, map.Dimensions.y - 1);
    PriorityQueue<((int x, int y) p, int c), int> queue = new();
    queue.Enqueue(((0,0),0), 0);
    while (queue.Count > 0)
    {
        var next = queue.Dequeue();
        if (seen.Contains(next.p))
        {
            continue;
        }
        if (next.p == end)
        {
            return next.c;
        }
        seen.Add(next.p);
        var moves = map.Moves(next.p).Where(p => !seen.Contains(p));
        foreach (var m in moves)
        {
            queue.Enqueue((m, next.c+1), next.c+1);
        }
    }

    return -1;
}


Map ParseData(string[] data, (int x, int y) dimensions, int nRead)
{
    List<(int x, int y)> blocks = [];
    foreach (var line in data)
    {
        var parts = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
        blocks.Add((int.Parse(parts[0]), int.Parse(parts[1])));
    }

    return new Map(blocks.Take(nRead).ToHashSet(), dimensions, blocks.Skip(nRead).ToList());
}


record Map(HashSet<(int x, int y)> Blocks, (int x, int y) Dimensions, List<(int x, int y)> Extra)
{
    public List<(int x, int y)> Moves((int x, int y) position)
    {
        List<(int x, int y)> directions =  [(1, 0), (0, 1), (-1, 0), (0, -1)];
        return directions.Select(delta => (x: position.x + delta.x, y: position.y + delta.y))
            .Where(p => p.x >= 0 && p.y >= 0
                                 && p.x < Dimensions.x && p.y < Dimensions.y
                                 && !Blocks.Contains(p)).ToList();
    }
}