using System.Collections.Immutable;

var data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input16.txt");

var testData = """
               #################
               #...#...#...#..E#
               #.#.#.#.#.#.#.#.#
               #.#.#.#...#...#.#
               #.#.#.#.###.#.#.#
               #...#.#.#.....#.#
               #.#.#.#.#.#####.#
               #.#...#.#.#.....#
               #.#.#####.#.###.#
               #.#.#.......#...#
               #.#.###.#####.###
               #.#.#...#.....#.#
               #.#.#.#####.###.#
               #.#.#.........#.#
               #.#.#.#########.#
               #S#.............#
               #################
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

var testData2 = """
                ###############
                #.......#....E#
                #.#.###.#.###.#
                #.....#.#...#.#
                #.###.#####.#.#
                #.#.#.......#.#
                #.#.#####.###.#
                #...........#.#
                ###.#.#####.#.#
                #...#.....#.#.#
                #.#.#.###.#.#.#
                #.....#...#.#.#
                #.###.#.#.#.#.#
                #S..#.....#...#
                ###############
                """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine(Solve(testData));
Console.WriteLine(Solve(data));

(int, int) Solve(string[] data)
{
    var map = ParseMap(data);
    var state = new State([map.Start], 0, 0);
    var queue = new PriorityQueue<State, int>();
    HashSet<(int, int, int)> seen = [];
    queue.Enqueue(state, 0);
    while (true)
    {
        var next = queue.Dequeue();
        var pos = next.Position.Last();
        seen.Add((pos.x, pos.y, next.Direction));
        if (pos == map.End)
        {
            HashSet<(int, int)> optimal = [];
            optimal.UnionWith(next.Position);
            while (queue.Peek().Cost == next.Cost)
            {
                var item = queue.Dequeue();
                if (item.Position.Last() == map.End)
                {
                    optimal.UnionWith(item.Position);
                }
            }
            return (next.Cost, optimal.Count);
        }
        var moves = next.Move(map);
        foreach (var m in moves)
        {
            var p = m.Position.Last();
            if (!seen.Contains((p.x, p.y, m.Direction)))
            {
                queue.Enqueue(m, m.Cost);
            }    
        }
    }
}

Map ParseMap(string[] data)
{
    (int x, int y) end = (0,0);
    (int x, int y) start = (0,0);
    foreach (var (y, s) in data.Index())
    {
        var x = s.IndexOf('E');
        if (x != -1)
        {
            end = (x, y);
        }
        x = s.IndexOf('S');
        if (x != -1)
        {
            start = (x, y);
        }
    }

    return new Map(data, start, end);
}

record Map(string[] Tiles, (int x, int y) Start, (int x, int y) End)
{
    public bool CanMove((int x, int y) pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < Tiles[pos.y].Length && pos.y < Tiles.Length && Tiles[pos.y][pos.x] != '#';
    }    
}
               
record State(ImmutableList<(int x, int y)> Position, int Direction, int Cost)
{
    public (int x, int y) Delta(int direction) => direction switch
    {
        0 => (1, 0),
        1 => (0, 1),
        2 => (-1, 0),
        3 => (0, -1)
    };
    
    public List<State> Move(Map map)
    {
        List<State> moves =
        [
            new State(Position, Direction == 0 ? 3 : Direction - 1, Cost + 1000),
            new State(Position, Direction == 3 ? 0 : Direction + 1, Cost + 1000)
        ];
        var delta = Delta(Direction);
        var now = Position.Last();
        var front = (now.x + delta.x, now.y + delta.y);
        if (map.CanMove(front))
        {
            moves.Add(new State(Position.Add(front), Direction, Cost + 1));
        }

        return moves;
    }
}