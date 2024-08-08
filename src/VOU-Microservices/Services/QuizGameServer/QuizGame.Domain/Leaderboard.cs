namespace QuizGame.Common;

[GenerateSerializer]
public class Leaderboard
{
    [Id(0)]
    public SortedDictionary<int, Guid> Entries { get; set; }
}
 
public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
{
    public int Compare(T? x, T? y)
    {
        return y.CompareTo(x);
    }
}

