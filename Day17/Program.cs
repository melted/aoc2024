
using System.Collections.Immutable;

var startState = new State([2, 4, 1, 4, 7, 5, 4, 1, 1, 4, 5, 5, 0, 3, 3, 0], 0, 25986278, 0, 0, []);

var testState = new State([0, 1, 5, 4, 3, 0], 0, 729, 0, 0, []);

var testState2 = new State([2,6], 0, 0, 0, 9, []);

var testState3 = new State([5,0,5,1,5,4], 0, 10, 0, 0, []);

var testState4 = new State([0,1,5,4,3,0], 0, 2024, 0, 0, []);

var testState5 = new State([0,3,5,4,3,0], 0, 2024, 0, 0, []);

Console.WriteLine(Solve1(startState));
Console.WriteLine(FindCopy(testState5));
Console.WriteLine(FindCopy(startState));

State Run(State start)
{
    State state = start;
    while (state.Ip < state.Program.Count)
    {
        state = state.Step();
    }

    return state;
}

string Solve1(State start) => String.Join(',', Run(start).output);


long FindCopy(State start)
{
    ImmutableList<int> target = start.Program;
    int proglen = start.Program.Count;
    Stack<(long, int)> states = new([(0,0)]);
    List<long> result = [];
    while(states.Count > 0)
    {
        var next = states.Pop();
        if (next.Item2 == proglen)
        {
            State end = Run(start with { A = next.Item1 });
            if (end.output.SequenceEqual(target))
            {
                result.Add(next.Item1);
            }
            continue;
        }
        for (int b = 0; b < 8; b++)
        {
            var val = (next.Item1 << 3) | b; 
            State state = start with { A = val };
            while (state.output.Count == 0)
            {
                state = state.Step();
            }
            if (state.output[0] == target[proglen - next.Item2 - 1])
            {
                states.Push((val, next.Item2+1));
            }
        }
    }

    return result.Min();
}

long Pack(ImmutableList<int> digits)
{
    return digits.Reverse().Aggregate((acc, x) => acc * 8 + x);
}

record State(ImmutableList<int> Program, int Ip, long A, long B, long C, ImmutableList<int> output)
{
    public State Step()
    {
        if (Ip >= Program.Count)
        {
            return this;
        }

        int nextIp = Ip + 2;
        int op = Program[Ip + 1];
        
        return Program[Ip] switch
        {
            0 => this with { Ip = nextIp, A = A / Pow2(Combo(op)) },
            1 => this with { Ip = nextIp, B = op ^ B },
            2 => this with { Ip = nextIp, B = Combo(op) % 8 },
            3 => this with { Ip = A == 0 ? nextIp : op },
            4 => this with { Ip = nextIp, B = B ^ C },
            5 => this with { Ip = nextIp, output = output.Add((int)(Combo(op) % 8)) },
            6 => this with { Ip = nextIp, B = A / Pow2(Combo(op)) },
            7 => this with { Ip = nextIp, C = A / Pow2(Combo(op)) },
        };
    }

    long Pow2(long n)
    {
        long res = 1;
        for (int i = 0; i < n; i++)
        {
            res *= 2;
        }

        return res;
    }
    long Combo(int op) => op switch
    {
        0 or 1 or 2 or 3 => op,
        4 => A,
        5 => B,
        6 => C,
        7 => throw new ArgumentException()
    };
}