﻿using System.Collections.Immutable;

string[] data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input20.txt");

string[] testData = """
    ###############
    #...#...#.....#
    #.#.#.#.#.###.#
    #S#...#.#.#...#
    #######.#.#.###
    #######.#.#...#
    #######.#.###.#
    ###..E#...#...#
    ###.#######.###
    #...###...#...#
    #.#####.#.###.#
    #.#...#.#.#...#
    #.#.#.#.#.#.###
    #...#...#...###
    ###############
    """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine(Solve(testData, 2, 15));
Console.WriteLine(Solve(data, 2, 100));
Console.WriteLine(Solve(testData, 20, 73));
Console.WriteLine(Solve(data, 20, 100));

List<(int, int)> GetTargets(int range)
{
    List<(int, int)> result = [];
    for (int y = -range; y < range + 1; y++)
    {
        int xr = range - Math.Abs(y);
        for (int x = -xr; x < xr + 1; x++)
        {
            result.Add((x, y));
        }
    }
    return result;
}

int Solve(string[] data, int range, int num)
{
    var map = ParseMap(data);
    var route = GetRoute(map);
    int count = 0;
    List<(int x, int y)> neighbors = GetTargets(range);
    foreach (var kv in route)
    {
        var x = kv.Key;
        foreach (var p in neighbors)
        {
            var distance = Math.Abs(p.x) + Math.Abs(p.y);
            if (route.TryGetValue((x.Item1 + p.x, x.Item2 + p.y), out var c))
            {
                if (c-kv.Value >= num + distance) count++;
            }
        }
    } 
    return count;
}

Dictionary<(int, int), int> GetRoute(Map map)
{
    var state = new State(map.Start, 0);
    var queue = new PriorityQueue<State, int>();
    Dictionary<(int, int), int> seen = [];
    queue.Enqueue(state, 0);
    while (true)
    {
        var next = queue.Dequeue();
        var pos = next.Position;
        seen.Add((pos.x, pos.y), next.Cost);
        if (pos == map.End)
        {
            return seen;
        }
        var moves = next.Move(map);
        foreach (var m in moves)
        {
            var p = m.Position;
            if (!seen.ContainsKey(p))
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
               
record State((int x, int y) Position, int Cost)
{
    public (int x, int y) Delta(int direction) => direction switch
    {
        0 => (1, 0),
        1 => (0, 1),
        2 => (-1, 0),
        3 => (0, -1),
        _ => throw new ArgumentException(),
    };
    
    public List<State> Move(Map map)
    {
        List<State> moves = [];
        for (int i = 0; i < 4; i++)
        {
            var delta = Delta(i);
            var step = (Position.x + delta.x, Position.y + delta.y);
            if (map.CanMove(step))
            {
                moves.Add(new State(step, Cost + 1));
            }
        }
        return moves;
    }
}