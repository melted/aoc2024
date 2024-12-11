using System.Collections.Immutable;

List<long> data = [0, 7, 198844, 5687836, 58, 2478, 25475, 894];

List<long> testData = [125, 17];

Dictionary<long, ImmutableDictionary<long, long>> evolutions = new();

Console.WriteLine(Solve(testData, 25));
Console.WriteLine(Solve(data, 25));
Console.WriteLine(Solve(data, 75));

long Solve(List<long> data, int n)
{
    var state = ImmutableDictionary<long, long>.Empty;
    foreach (var stone in data)
    {
        state = state.Add(stone, 1);
    }

    for (int i = 0; i < n; i++)
    {
        state = Evolve(state);
    }
    return state.Values.Sum();
}


ImmutableDictionary<long, long> Evolve(ImmutableDictionary<long, long> start)
{
    var result = ImmutableDictionary<long, long>.Empty;

    foreach (var p in start)
    {
        var ev = DoStone(p.Key, p.Value);
        result = Merge(result, ev);
    }

    return result;
}

ImmutableDictionary<long, long> Merge(ImmutableDictionary<long, long> a, ImmutableDictionary<long, long> b)
{
    foreach (var kv in b)
    {
        a.TryGetValue(kv.Key, out var val);
        a = a.SetItem(kv.Key, val + kv.Value);
    }
    return a;
}

ImmutableDictionary<long, long> DoStone(long stone, long mult)
{
    var result = ImmutableDictionary<long, long>.Empty;;
    if (stone == 0)
    {
        return result.Add(1, mult);
    } 
    var d = Digits(stone);
    if (d % 2 == 0)
    {
        long splitter = (long)Math.Pow(10, d / 2);
        var lhs = stone / splitter;
        var rhs = stone % splitter;
        if (lhs == rhs) mult *= 2;
        return result.Add(lhs, mult).Add(rhs, mult);
    }
    else
    {
        return result.Add(2024*stone, mult);
    }
}

int Digits(long num)
{
    int digs = 0;
    while (num > 0)
    {
        digs++;
        num /= 10;
    }
    return digs;
}