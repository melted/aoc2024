var data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input6.txt");

var testData =  """
                ....#.....
                .........#
                ..........
                ..#.......
                .......#..
                ..........
                .#..^.....
                ........#.
                #.........
                ......#...
                """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

List<(int dx, int dy)> directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

Map ParseMap(string[] data)
{
    HashSet<(int, int)> blocks = [];
    (int, int) start = (0, 0);
    int x = 0;
    int y = 0;
    foreach (var line in data)
    {
        foreach (var ch in line)
        {
            switch (ch)
            {
                case '#':
                    blocks.Add((x, y));
                    break;
                case '.':
                    break;
                case '^':
                    start = (x, y);
                    break;
            }
            x++;
        }
        x = 0;
        y++;
    }
    return new Map(blocks, (data[0].Length, data.Length), start);
}

(HashSet<(int, int)> visited, bool loop) MakeRun(Map map)
{
    HashSet<(int, int)> visited = [];
    HashSet<(int, int, int)> states = [];
    var pos = map.start;
    var dir = 0;
    while (map.InBounds(pos))
    {
        visited.Add(pos);
        var state = (pos.x, pos.y, dir);
        if (!states.Add(state))
        {
            return (visited, true);
        }
        var direction = directions[dir];
        var next = (pos.x + direction.dx, pos.y + direction.dy);
        if (map.blocks.Contains(next))
        {
            dir = (dir + 1) % directions.Count;
        }
        else
        {
            pos = next;
        }
    }
    return (visited, false);
}

Console.WriteLine(MakeRun(ParseMap(testData)).visited.Count);
Console.WriteLine(MakeRun(ParseMap(data)).visited.Count);

int CountLoops(Map map)
{
    var loops = 0;
    var visited = MakeRun(map).visited;
    visited.Remove(map.start);
    foreach (var pos in visited)
    {
        HashSet<(int, int)> newBlocks = new(map.blocks);
        newBlocks.Add(pos);
        var augmented = map with { blocks = newBlocks };
        var (_, loop) = MakeRun(augmented);
        if (loop) loops++;
    }
    return loops;
}

Console.WriteLine(CountLoops(ParseMap(testData)));
Console.WriteLine(CountLoops(ParseMap(data)));

record Map(HashSet<(int x, int y)> blocks, (int x, int y) dimensions, (int x, int y) start)
{
    public bool InBounds((int x, int y) pos)
    {
        return pos.x >= 0 && pos.x < dimensions.x && pos.y >= 0 && pos.y < dimensions.y;
    }
}