var data = File.ReadAllLines("/home/niklas/repos/aoc2024/data/input23.txt");

var testData =  """
                kh-tc
                qp-kh
                de-cg
                ka-co
                yn-aq
                qp-ub
                cg-tb
                vc-aq
                tb-ka
                wh-tc
                yn-cg
                kh-ub
                ta-co
                de-co
                tc-td
                tb-wq
                wh-td
                ta-ka
                td-qp
                aq-cg
                wq-ub
                ub-vc
                de-ta
                wq-aq
                wq-vc
                wh-yn
                ka-de
                kh-ta
                co-tc
                wh-qp
                tb-vc
                td-yn
                """.Split("\n",StringSplitOptions.RemoveEmptyEntries);

Graph ParseData(string[] data)
{
    List<(string, string)> edges = data.Select(s => {
        var parts = s.Split("-", StringSplitOptions.TrimEntries);
        return (parts[0], parts[1]);
    }).ToList();
    Dictionary<string, HashSet<string>> connections = new();
    foreach (var (a,b) in edges)
    {
        if (!connections.ContainsKey(a))
        {
            connections[a] = new HashSet<string>();
        }
        if (!connections.ContainsKey(b))
        {
            connections[b] = new HashSet<string>();
        }
        connections[a].Add(b);
        connections[b].Add(a);
    }
    return new Graph(edges, connections);
}

int Solve(string[] data)
{
    Graph graph = ParseData(data);
    HashSet<(string, string, string)> triples = [];
    foreach (var (a,b) in graph.Edges)
    {
        var common = new HashSet<string>(graph.Connections[a]);
        common.IntersectWith(graph.Connections[b]);
        foreach (var c in common)
        {
            List<string> triple = [a,b,c];
            triple.Sort();
            triples.Add((triple[0], triple[1], triple[2]));
        }
    }

    return triples.Where(t => t.Item1[0] == 't' ||
                              t.Item2[0] == 't' ||
                              t.Item3[0] == 't').Count();
}

string Solve2(string[] data)
{
    var graph = ParseData(data);
    List<List<string>> candidates = [];
    HashSet<(string, string)> remaining = graph.Edges.ToHashSet();
    while (remaining.Count > 0)
    {
        var start = remaining.First();
        remaining.Remove(start);
        List<string> current = [start.Item1, start.Item2];
        var possible = graph.Connections[start.Item1];
        foreach (var p in possible)
        {
            if (current.All(s => graph.Connections[p].Contains(s)))
            {
                current.Add(p);
            }
        }
        foreach (var i in current) {
            foreach (var j in current)
            {
                if (i != j)
                {
                    remaining.Remove((i,j));
                }
            }
        }
        candidates.Add(current);
    }
    List<string> best = candidates.MaxBy(x => x.Count) ?? [];
    best.Sort();
    return String.Join(",", best);
}

Console.WriteLine(Solve(testData));
Console.WriteLine(Solve(data));
Console.WriteLine(Solve2(testData));
Console.WriteLine(Solve2(data));
record Graph(List<(string, string)> Edges, Dictionary<string, HashSet<string>> Connections);