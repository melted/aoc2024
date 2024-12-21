
var data = """
           459A
           671A
           846A
           285A
           083A
           """;

int[] nums = [459, 671, 846, 285, 83];
string[] translated = ["^^<<A>A^>AvvvA", "^^A<<^AvvA>>vA", "<^^^A<vA>>AvvA", "<^A^^AvAvv>A", "<A^^^Avv>AvA"];

string Transition(char from, char to)
{
    return (from, to) switch
    {
        ('<', 'A') => ">>^A",
        ('v', 'A') => "^>A",
        ('>', 'A') => "^A",
        ('^', 'A') => ">A",
        ('<', '^') => ">^A",
        ('v', '^') => "^A",
        ('>', '^') => "<^A",
        ('A', '^') => "<A",
        ('^', '<') => "v<A",
        ('v', '<') => "<A",
        ('>', '<') => "<<A",
        ('A', '<') => "v<<A",
        ('<', '>') => ">>A",
        ('v', '>') => ">A",
        ('^', '>') => "v>A",
        ('A', '>') => "vA",
        ('<', 'v') => ">A",
        ('^', 'v') => "vA",
        ('>', 'v') => "<A",
        ('A', 'v') => "<vA",
        _ => "A"
    };
}

List<string> ExpandOne(string s)
{
    char previous = 'A';
    List<string> result = [];
    foreach (var ch in s)
    {
        result.Add(Transition(previous, ch));
        previous = ch;
    }

    return result;
}

long Expand(string start, int times)
{
    Dictionary<string, long> cache = new();
    cache.Add(start, 1);
    for (int i = 0; i < times; i++)
    {
        Dictionary<string, long> next = new();
        foreach (var (key, val) in cache)
        {
            var result = ExpandOne(key);
            foreach (var frag in result)
            {
                next.TryGetValue(frag, out long n);
                next[frag] = n + val;
            }
        }

        cache = next;
    }

    return cache.Select(kv => kv.Key.Length * kv.Value).Sum();
}

long Solve(int expansions)
{
    long sum = 0;
    for (int i = 0; i < translated.Length; i++)
    {
        string expanded = translated[i];
        long length = Expand(expanded, expansions);
        sum += nums[i] * length;
    }

    return sum;
}

Console.WriteLine(Solve(2));
Console.WriteLine(Solve(25));
