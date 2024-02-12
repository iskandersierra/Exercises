namespace Exercises;



public class DisjointSet<T>
{
    private abstract record Node(T Start, T End)
    {
        public abstract bool IsLeaf { get; }
        public abstract int Height { get; }
    }

    private record LeafNode(T Start, T End) : Node(Start, End)
    {
        public override bool IsLeaf => true;
        public override int Height => 0;
    }

    private record MidNode(T Start, T End, Node Left, Node Right) : Node(Start, End)
    {
        public override bool IsLeaf => false;
        public override int Height => 1 + Math.Max(Left.Height, Right.Height);
    }
}
