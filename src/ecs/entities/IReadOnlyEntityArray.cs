namespace HUI;

public interface IReadOnlyEntityArray
{
    int Count { get; }

    bool this[int index] { get; }
}
