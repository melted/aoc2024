var data = File.ReadAllText("D:/Niklas/repos/aoc2024/data/input9.txt").TrimEnd();

var testData = "2333133121414131402";

Disk Parse(string data)
{
    var free = new Free();
    var fileList = new List<(int, int, long)>();
    var freeList = new List<(int, int)>();
    var pos = 0;
    
    var blocks = data.Select(c => Convert.ToInt32(c)-48).Chunk(2).Index().SelectMany(c =>
    {
        var node = new Node(c.Index);
        IEnumerable<Block> blocks = Enumerable.Repeat(node, c.Item[0]);
        fileList.Add((pos, c.Item[0], c.Index));
        pos += c.Item[0];
        if (c.Item.Length == 2)
        {
            IEnumerable<Block> frees = Enumerable.Repeat(free, c.Item[1]);
            freeList.Add((pos, c.Item[1]));
            blocks = blocks.Concat(frees);
            pos += c.Item[1];
        }
        return blocks;
    }).ToList();
    return new Disk(blocks, freeList, fileList);
}

Console.WriteLine(Solve1(testData));
Console.WriteLine(Solve1(data));
Console.WriteLine(Solve2(testData));
Console.WriteLine(Solve2(data));

long Solve1(string data)
{
    Disk disk = Parse(data);
    disk.Defragment();
    return disk.Checksum();
}

long Solve2(string data)
{
    Disk disk = Parse(data);
    disk.DefragmentFiles();
    return disk.Checksum();
}

record Block();

record Free() : Block;
record Node(long id) : Block;

class Disk(List<Block> blocks, List<(int, int)> freeList, List<(int, int, long)> fileList)
{
    private int freeBlock_ = 0;
    private int endBlock_ = blocks.Count - 1;
    private Free free_ = new Free();
    
    public void Defragment()
    {
        FindFreeBlock();
        FindEndBlock();
        while (freeBlock_ < endBlock_)
        {
            Swap(freeBlock_, endBlock_);
            FindFreeBlock();
            FindEndBlock();
        }
    }
    
    public void DefragmentFiles()
    {
        var files = new List<(int, int, long)>(fileList);
        files.Reverse();
        foreach (var file in files)
        {
            for (int i = 0; i < freeList.Count; i++)
            {
                if (file.Item1 < freeList[i].Item1) 
                    break;
                if (file.Item2 <= freeList[i].Item2)
                {
                    MoveFile(file.Item1, freeList[i].Item1, file.Item2);
                    var newSize = freeList[i].Item2 - file.Item2;
                    if (newSize == 0)
                    {
                        freeList.RemoveAt(i);
                    }
                    else
                    {
                        freeList[i] = (freeList[i].Item1 + file.Item2, newSize);
                    }
                    break;
                }
            }
        }
    }
    
    void FindFreeBlock()
    {
        while (!(blocks[freeBlock_] is Free) && freeBlock_ <= endBlock_)
            freeBlock_++;
    }
    
    void FindEndBlock()
    {
        while ((blocks[endBlock_] is Free) && freeBlock_ <= endBlock_)
            endBlock_--;
    }
    
    void Swap(int a, int b)
    {
        Block temp = blocks[a];
        blocks[a] = blocks[b];
        blocks[b] = temp;
    }
    
    void MoveFile(int from, int to, int size)
    {
        for (int i = 0; i < size; i++)
        {
            blocks[to+i] = blocks[from+i];
            blocks[from + i] = free_;
        }
    }
    
    public long Checksum()
    {
        return blocks.Index().Select(c => c.Item switch { Node(var id) => c.Index * id,
            _ => 0 }).Sum();
    }
}