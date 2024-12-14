using System.Text;
using System.Text.RegularExpressions;

string[] data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input14.txt");

string[] testData = """
                    p=0,4 v=3,-3
                    p=6,3 v=-1,-3
                    p=10,3 v=-1,2
                    p=2,0 v=2,-1
                    p=0,0 v=1,3
                    p=3,0 v=-2,-2
                    p=7,6 v=-1,-3
                    p=3,0 v=-1,-2
                    p=9,3 v=2,3
                    p=7,3 v=-1,2
                    p=2,4 v=2,-3
                    p=9,5 v=-3,-3
                    """.Split("\n", StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine(Solve(testData, (11,7)));
Console.WriteLine(Solve(data, (101,103)));
Console.WriteLine(Solve2(data, (101, 103)));

int Solve(string[] data, (int x, int y) dimensions)
{
    Map m = ParseData(data, dimensions).Move(100);
    return m.Eval();
}

int Solve2(string[] data, (int x, int y) dimensions)
{
    Map m = ParseData(data, dimensions);
    int i = 0;
    while(true)
    {
        var clump = m.Census();
        if (clump > 20)
        {
            return i;
        }

        i++;
        m = m.Move(1);
    }

    return -1;
}

Map ParseData(string[] data, (int x, int y) dimensions)
{
    Regex r = new Regex(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)");
    List<Robot> robots = [];
    foreach (var line in data)
    {
        Match m = r.Match(line);
        if (m.Success)
        {
            robots.Add(new Robot((int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)),
                (int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value))));
        }
    }

    return new Map(robots, dimensions);
}

record Map(List<Robot> Robots, (int x, int y) Dimensions)
{
    public Map Move(int steps)
    {
        return this with { Robots = Robots.Select(r => r.Move(Dimensions, steps)).ToList() };
    }
    
    public int Eval()
    {
        var q1 = Robots.Count(r => r.Position.x < Dimensions.x / 2 && r.Position.y < Dimensions.y / 2);
        var q2 = Robots.Count(r => r.Position.x > Dimensions.x / 2 && r.Position.y < Dimensions.y / 2);
        var q3 = Robots.Count(r => r.Position.x < Dimensions.x / 2 && r.Position.y > Dimensions.y / 2);
        var q4 = Robots.Count(r => r.Position.x > Dimensions.x / 2 && r.Position.y > Dimensions.y / 2);
        return q1 * q2 * q3 * q4;
    }
    
    public void Display()
    {
        char[][] disp = new char[Dimensions.y][];
        for (int i = 0; i < disp.Length; i++)
        {
            disp[i] = new char[Dimensions.x];
            Array.Fill(disp[i], '.' );
        }
        foreach (var r in Robots)
        {
            disp[r.Position.y][r.Position.x] = 'X';
        }

        foreach (var line in disp)
        {
            Console.WriteLine(new String(line));
        }
    }
    
    public int Census()
    {
        List<(int x, int y)> directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];
        HashSet<(int x, int y)> positions = Robots.Select(r => r.Position).ToHashSet();
        List<int> clumps = [];
        while (positions.Count > 0)
        {
            HashSet<(int x, int y)> clump = [];
            HashSet<(int x, int y)> toCheck = [positions.First()];
            while (toCheck.Count > 0)
            {
                HashSet<(int x, int y)> next = [];
                foreach (var p in toCheck)
                {
                    var neighbors = directions.Select(d => (p.x + d.x, p.y + d.y))
                        .Where(p => positions.Contains(p));
                    next.UnionWith(neighbors);
                }
                
                clump.UnionWith(toCheck);
                next.ExceptWith(clump);
                toCheck = next;
            }
            positions.ExceptWith(clump);
            clumps.Add(clump.Count);
        }

        return clumps.Max();
    }
}

record Robot((int x, int y) Position, (int x, int y) Velocity)
{
    public Robot Move((int x, int y) dimensions, int n)
    {
        var posX = (Position.x + n * Velocity.x) % dimensions.x;
        var posY = (Position.y + n * Velocity.y) % dimensions.y;
        if (posX < 0) posX += dimensions.x;
        if (posY < 0) posY += dimensions.y;
        return this with
        {
            Position = (posX, posY)
        };
    }
}