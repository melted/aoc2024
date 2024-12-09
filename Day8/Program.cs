using System.Collections.Immutable;

var data = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input8.txt");

var testData = """
               ............
               ........0...
               .....0......
               .......0....
               ....0.......
               ......A.....
               ............
               ............
               ........A...
               .........A..
               ............
               ............
               """.Split("\n", StringSplitOptions.RemoveEmptyEntries);
               
Map ParseMap(string[] data)
{
    var antennas = data.Index()
        .SelectMany(y => y.Item.Index().Where(x => x.Item != '.')
            .Select(x => (x.Item, (x.Index, y.Index)))).ToList();
    Dictionary<char, ImmutableHashSet<(int, int)>> d = new();
    foreach (var a in antennas)
    {
        if (!d.ContainsKey(a.Item))
        {
            d[a.Item] = ImmutableHashSet<(int, int)>.Empty;
        }
        d[a.Item] = d[a.Item].Add(a.Item2);
    }

    return new Map(d, (data[0].Length, data.Length));
}

Console.WriteLine(CountAntinodes(ParseMap(testData), false));
Console.WriteLine(CountAntinodes(ParseMap(data), false));
Console.WriteLine(CountAntinodes(ParseMap(testData), true));
Console.WriteLine(CountAntinodes(ParseMap(data), true));

int CountAntinodes(Map map, bool harmonics)
{
    ImmutableHashSet<(int, int)> antinodes = ImmutableHashSet<(int, int)>.Empty;
    foreach (var kv in map.antennas)
    {
        antinodes = antinodes.Union(Antinodes(map, kv.Value, harmonics));
    }

    return antinodes.Where(p => map.Inbounds(p)).Count();
}

ImmutableHashSet<(int, int)> Antinodes(Map map, ImmutableHashSet<(int, int)> nodes, bool harmonics)
{
    if (nodes.Count <= 1)
    {
        return ImmutableHashSet<(int, int)>.Empty;
    }
    else
    {
        var pick = nodes.First();
        var rest = nodes.Remove(pick);
        var antinodes = ImmutableHashSet<(int, int)>.Empty;
        foreach (var node in rest)
        {
            var dx = node.Item1 - pick.Item1;
            var dy = node.Item2 - pick.Item2;
            if (harmonics)
            {
                var p = pick;
                while (map.Inbounds(p))
                {
                    antinodes = antinodes.Add(p);
                    p = (p.Item1 + dx, p.Item2 + dy);
                }
                p = (pick.Item1 - dx, pick.Item2 - dy);;
                while (map.Inbounds(p))
                {
                    antinodes = antinodes.Add(p);
                    p = (p.Item1 - dx, p.Item2 - dy);
                }
            } 
            else 
            {
                antinodes = antinodes.Add((node.Item1 + dx, node.Item2 + dy));
                antinodes = antinodes.Add((pick.Item1 - dx, pick.Item2 - dy));
            }
        }

        return antinodes.Union(Antinodes(map, rest, harmonics));
    }
}

record Map(Dictionary<char, ImmutableHashSet<(int, int)>> antennas, (int w, int h) dimensions)
{
    public bool Inbounds((int x, int y) p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < dimensions.w && p.y < dimensions.h;
    }
}