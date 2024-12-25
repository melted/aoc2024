using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

var data = File.ReadAllText("/home/niklas/repos/aoc2024/data/input24.txt");

var testData =  """
                x00: 1
                x01: 0
                x02: 1
                x03: 1
                x04: 0
                y00: 1
                y01: 1
                y02: 1
                y03: 1
                y04: 1

                ntg XOR fgs -> mjb
                y02 OR x01 -> tnw
                kwq OR kpj -> z05
                x00 OR x03 -> fst
                tgd XOR rvg -> z01
                vdt OR tnw -> bfw
                bfw AND frj -> z10
                ffh OR nrd -> bqk
                y00 AND y03 -> djm
                y03 OR y00 -> psh
                bqk OR frj -> z08
                tnw OR fst -> frj
                gnj AND tgd -> z11
                bfw XOR mjb -> z00
                x03 OR x00 -> vdt
                gnj AND wpb -> z02
                x04 AND y00 -> kjc
                djm OR pbm -> qhw
                nrd AND vdt -> hwm
                kjc AND fst -> rvg
                y04 OR y02 -> fgs
                y01 AND x02 -> pbm
                ntg OR kjc -> kwq
                psh XOR fgs -> tgd
                qhw XOR tgd -> z09
                pbm OR djm -> kpj
                x03 XOR y03 -> ffh
                x00 XOR y04 -> ntg
                bfw OR bqk -> z06
                nrd XOR fgs -> wpb
                frj XOR qhw -> z04
                bqk OR frj -> z07
                y03 OR x01 -> nrd
                hwm AND bqk -> z03
                tgd XOR rvg -> z12
                tnw OR pbm -> gnj
                """;

Circuit ParseData(string data)
{
    var parts = data.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    Dictionary<string, bool> initial = new();
    var inits = parts[0].Split("\n",StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(":", StringSplitOptions.TrimEntries));
    foreach (var iv in inits)
    {
        initial.Add(iv[0], iv[1] == "1");
    }
    HashSet<Connection> connections = [];
    Regex regex = new Regex(@"(\w\w\w) (AND|OR|XOR) (\w\w\w) -> (\w\w\w)");
    foreach (var c in parts[1].Split("\n",StringSplitOptions.RemoveEmptyEntries))
    {
        var match = regex.Match(c);
        if (match.Success)
        {
            connections.Add(new Connection((match.Groups[1].Value, match.Groups[3].Value),
                               match.Groups[2].Value, match.Groups[4].Value));
        }
    }
    return new Circuit(initial, connections);
}

long Solve(string data)
{
    var circuit = ParseData(data);
    while (circuit.Connections.Count > 0)
    {
        foreach (var candidate in circuit.Connections)
        {
            if (circuit.Values.TryGetValue(candidate.Input.Item1, out var a) &&
                circuit.Values.TryGetValue(candidate.Input.Item2, out var b))
            {
                var value = candidate.Op switch {
                    "AND" => a & b,
                    "OR" => a|b,
                    "XOR" => a^b
                };
                circuit.Values.Add(candidate.Output, value); 
                circuit.Connections.Remove(candidate);   
            }
        }
        Console.WriteLine(circuit.Connections.Count);
    }

    var output = circuit.Values.Where(kv => kv.Key[0] == 'z').Select(kv => (kv.Key, kv.Value)).ToList();
    output.Sort();
    long result = 0;
    long magn = 1;
    for (int i = 0; i < output.Count; i++)
    {
        if (output[i].Value)
        {
            result += magn;
        }
        magn *= 2;
    }
    return result;
}

Console.WriteLine(Solve(testData));
Console.WriteLine(Solve(data));
record Connection((string, string) Input, string Op, string Output);
record Circuit(Dictionary<string, bool> Values, HashSet<Connection> Connections);