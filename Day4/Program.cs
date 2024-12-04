var lines = File.ReadAllLines("D:/Niklas/repos/aoc2024/data/input4.txt");
var testData = """
               MMMSXXMASM
               MSAMXMSMSA
               AMXSXMAAMM
               MSAMASMSMX
               XMASAMXAMM
               XXAMMXXAMA
               SMSMSASXSS
               SAXAMASAAA
               MAMMMXMMMM
               MXMXAXMASX
               """.Split(["\r","\n"], StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine(Solve1(testData));
Console.WriteLine(Solve1(lines));
Console.WriteLine(Solve2(testData));
Console.WriteLine(Solve2(lines));

int Solve1(string[] data)
{
    return AllStrings(data).Count(s => s == "XMAS" || s == "SAMX");
}

List<string> AllStrings(string[] data)
{
    var width = data[0].Length;
    var height = data.Length;
    List<string> result = [];
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            result.AddRange(GetStrings(data, j,i));
        }
    }
    return result;
}

List<string> GetStrings(string[] data, int x, int y)
{
    List<string> result = [];
    List<(int, int)> strides = [(1,0), (1,1), (0,1),(1,-1)];
    foreach (var (dx, dy) in strides)
    {
        string s = "";
        for (int i = 0; i < 4; i++)
        {
            var u = y + dy * i;
            var z = x + dx * i;
            if (u >= 0 && z >= 0 && u < data.Length && z < data[0].Length)
            {
                s += data[u][z];
            }
        }
        result.Add(s);
    }
    return result;
}

bool IsMas(string[] data, int x, int y)
{
    if (x == 0 || x == data[0].Length-1 || y == 0 || y == data.Length-1 || data[x][y] != 'A')
    {
        return false;
    }
    HashSet<char> a = [data[x + 1][y + 1], data[x - 1][y - 1]];
    HashSet<char> b = [data[x - 1][y + 1], data[x + 1][y - 1]];
    HashSet<char> correct = ['S', 'M'];
    return a.SetEquals(correct) && b.SetEquals(correct);
}

int Solve2(string[] data)
{
    return Enumerable.Range(0, data.Length)
        .SelectMany(y => Enumerable.Range(0, data[y].Length), (y, x) => (x, y))
        .Count(p => IsMas(data, p.x, p.y));
}