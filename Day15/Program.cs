using System.Security.Cryptography;

string data = File.ReadAllText("D:/Niklas/repos/aoc2024/data/input15.txt");

string testData = """
                  ##########
                  #..O..O.O#
                  #......O.#
                  #.OO..O.O#
                  #..O@..O.#
                  #O#..O...#
                  #O..O..O.#
                  #.OO.O.OO#
                  #....O...#
                  ##########
                  
                  <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                  vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                  ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                  <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                  ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                  ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                  >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                  <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                  ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                  v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<
                  """;

var testData2 = """
                ########
                #..O.O.#
                ##@.O..#
                #...O..#
                #.#.O..#
                #...O..#
                #......#
                ########
                
                <^^>>>vv<v>>v<<
                """;

string testData3 = """
                   #######
                   #...#.#
                   #.....#
                   #..OO@#
                   #..O..#
                   #.....#
                   #######

                   <vv<<^^<<^^
                   """;

Console.WriteLine(ParseData(testData).Run());
Console.WriteLine(ParseData(testData2).Run());
Console.WriteLine(ParseData(data).Run());
Console.WriteLine(ParseData(testData, true).Run2());
Console.WriteLine(ParseData(data, true).Run2());

Map ParseData(string data, bool part2 = false)
{
    var parts = data.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    var rows = parts[0].Split("\n", StringSplitOptions.RemoveEmptyEntries);
    HashSet<(int x, int y)> boxes = [];
    HashSet<(int x, int y)> walls = [];
    (int x, int y) robot = (0, 0);
    var stride = part2 ? 2 : 1;
    for (int y = 0; y < rows.Length; y++)
    {
        for(int x = 0; x < rows[y].Length; x++)
        {
            var ch = rows[y][x];
            switch (ch)
            {
                case '#':
                    walls.Add((x*stride,y));
                    if (part2) walls.Add((x*stride + 1, y));
                    break;
                case 'O':
                    boxes.Add((x*stride,y));
                    break;
                case '@':
                    robot = (x*stride, y);
                    break;
            }
        }
    }

    List<Map.Move> moves = [];
    foreach (var ch in parts[1])
    {
        switch (ch)
        {
            case '>':
                moves.Add(Map.Move.Right);
                break;
            case 'v':
                moves.Add(Map.Move.Down);
                break;
            case '<':
                moves.Add(Map.Move.Left);
                break;
            case '^':
                moves.Add(Map.Move.Up);
                break;
        }
    }

    return new Map(boxes, walls, robot, moves);
}

class Map(HashSet<(int x, int y)> Boxes, HashSet<(int x, int y)> Walls,  (int x, int y) Robot, List<Map.Move> Moves)
{
    public enum Move
    {
        Right,
        Down,
        Left,
        Up
    }
    
    public void MoveRobot(Move m)
    {
        var dir = Direction(m);
        var next = (Robot.x + dir.x, Robot.y + dir.y);
        List<(int x, int y)> moved = [];
        while (Boxes.Contains(next))
        {
            moved.Add(next);
            next = (next.Item1 + dir.x, next.Item2 + dir.y);
        }
        if (!Walls.Contains(next))
        {
            Robot = (Robot.x + dir.x, Robot.y + dir.y);
            if (moved.Count > 0)
            {
                Boxes.Remove(moved.First());
                Boxes.Add(next);
            }
        }
    }
    
    public void MoveRobot2(Move m)
    {
        var dir = Direction(m);
        var next = (Robot.x + dir.x, Robot.y + dir.y);
        if (!Walls.Contains(next))
        {
            List<(int x, int y)> moved = GetMovableWideBoxes(Robot, m, out var blocked);
            if (!blocked)
            {
                Robot = (Robot.x + dir.x, Robot.y + dir.y);
                foreach (var box in moved)
                {
                    Boxes.Remove(box);
                }
                foreach (var box in moved)
                {
                    Boxes.Add((box.x + dir.x, box.y + dir.y));
                }
            }
        }
    }
    
    public List<(int x, int y)> GetMovableWideBoxes((int x, int y) position, Move move, out bool blocked)
    {
        var dir = Direction(move);
        blocked = false;
        var next = (Robot.x + dir.x, Robot.y + dir.y);
        if (move is Move.Left or Move.Right)
        {
            List<(int x, int y)> moved = [];
            while (HasWideBox(next))
            {
                moved.Add(next);
                next = (next.Item1 + dir.x, next.Item2 + dir.y);
            }
            if (!Walls.Contains(next))
            {
                return moved.Intersect(Boxes).ToList();
            }
            else
            {
                blocked = true;
                return [];
            }
        }
        else
        {
            HashSet<(int x, int y)> boxes = [];
            HashSet<(int x, int y)> toCheck = WideBox(next).ToHashSet();
            while (toCheck.Count > 0)
            {
                HashSet<(int next, int y)> nextRow = [];
                foreach (var p in toCheck)
                {
                    nextRow.UnionWith(WideBox((p.x + dir.x, p.y + dir.y)));
                }
                boxes.UnionWith(toCheck);
                toCheck = nextRow;
            }

            foreach (var b in boxes)
            {
                if (Walls.Contains((b.x + dir.x, b.y + dir.y)))
                {
                    blocked = true;
                    return [];
                }
            }

            return boxes.Intersect(Boxes).ToList();
        }
    }
    
    public List<(int x, int y)> WideBox((int x, int y) position)
    {
        if (Boxes.Contains(position)) return [position, (position.x + 1, position.y)];
        if (Boxes.Contains((position.x - 1, position.y))) return [(position.x - 1, position.y), position];
        return [];
    }
    
    public bool HasWideBox((int x, int y) position)
    {
        return Boxes.Contains(position) || Boxes.Contains((position.x - 1, position.y));
    }
    
    public (int x, int y) Direction(Move m) => m switch
    {
        Move.Right => (1, 0),
        Move.Down => (0, 1),
        Move.Left => (-1, 0),
        Move.Up => (0, -1)
    };
    
    public int Eval()
    {
        return Boxes.Select(b => 100 * b.y + b.x).Sum();
    }
    
    public int Run()
    {
        foreach (var move in Moves)
        {
            MoveRobot(move);
        }

        return Eval();
    }
    
    public int Run2()
    {
        foreach (var move in Moves)
        {
            MoveRobot2(move);
 //           Console.WriteLine($"{move}-----\n\n");
 //           Display(true);
        }

        return Eval();
    }
    
    public void Display(bool part2)
    {
        int width = Walls.Select(p => p.x).Max() + 1;
        int height = Walls.Select(p => p.y).Max() + 1;
        char[][] disp = new char[height][];
        for (int i = 0; i < disp.Length; i++)
        {
            disp[i] = new char[width];
            Array.Fill(disp[i], '.' );
        }
        foreach (var r in Walls)
        {
            disp[r.y][r.x] = '#';
        }
        foreach (var r in Boxes)
        {
            if (part2)
            {
                disp[r.y][r.x] = '[';
                disp[r.y][r.x+1] = ']';
            }
            else
            {
                disp[r.y][r.x] = 'O';
            }
        }

        disp[Robot.y][Robot.x] = '@';
        foreach (var line in disp)
        {
            Console.WriteLine(new String(line));
        }
    }
}