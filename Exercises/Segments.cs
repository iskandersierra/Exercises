using Spectre.Console.Rendering;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using static Exercises.Segments;

namespace Exercises;

public interface ISegmentDescriptor<TSegment>
    where TSegment : notnull
{
    TSegment Empty { get; }
    bool IsEmpty(TSegment segment);
    bool AreEqual(TSegment left, TSegment right);
    bool Contains(TSegment segment, TSegment otherSegment);
    TSegment Intersection(TSegment left, TSegment right);
    bool Intersects(TSegment left, TSegment right);
    bool Touches(TSegment left, TSegment right);
    TSegment Bounding(TSegment left, TSegment right);
    IEnumerable<TSegment> Union(TSegment left, TSegment right);
    IEnumerable<TSegment> Subtract(TSegment left, TSegment right);
}

public interface ISegmentDescriptor<TSegment, TElement> :
    ISegmentDescriptor<TSegment>
    where TSegment : notnull
    where TElement : notnull
{
    bool Contains(TSegment segment, TElement element);
    TSegment Create(TElement start, TElement end);
}

public interface ISegmentSet<TSegment> :
    IEnumerable<TSegment>
    where TSegment : notnull
{
    bool IsEmpty { get; }
    void Clear();
    int Count { get; }

    bool Add(TSegment newSegment);
    bool AddRange(IEnumerable<TSegment> newSegments);

    ISegmentSet<TSegment> Intersection(TSegment newSegment);
    bool Intersect(TSegment newSegment);
    bool Intersects(TSegment newSegment);

    ISegmentSet<TSegment> Subtraction(TSegment newSegment);
    bool Subtract(TSegment newSegment);

    // TODO: RemoveRange, Contains, Bounding
}

public readonly struct Segment<TElement>
{
    public readonly TElement Start;
    public readonly TElement End;

    internal Segment(TElement start, TElement end)
    {
        Start = start;
        End = end;
    }

    public void Deconstruct(out TElement start, out TElement end)
    {
        start = Start;
        end = End;
    }

    public override string ToString() =>
        $"[{Start}, {End})";

    public override bool Equals(object? obj) =>
        obj is Segment<TElement> other &&
        object.Equals(Start, other.Start) &&
        object.Equals(End, other.End);

    public override int GetHashCode() =>
        HashCode.Combine(Start, End);
}

public static class Segments
{
    public static readonly NumericSegmentDescriptor<int> Int32 = new();
    public static readonly NumericSegmentDescriptor<long> Int64 = new();

    public class NumericSegmentDescriptor<TNumber> :
        ISegmentDescriptor<Segment<TNumber>, TNumber>
        where TNumber : struct, INumber<TNumber>
    {
        private static readonly Segment<TNumber> EmptySegment = new(TNumber.Zero, TNumber.Zero);

        public Segment<TNumber> Empty => EmptySegment;

        public TNumber Size(Segment<TNumber> segment) =>
            segment.End - segment.Start;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment<TNumber> Create(TNumber start, TNumber end) =>
            start >= end ? EmptySegment : new Segment<TNumber>(start, end);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty(Segment<TNumber> segment) =>
            segment.Start >= segment.End;

        public bool AreEqual(Segment<TNumber> left, Segment<TNumber> right) =>
            left.Start == right.Start &&
            left.End == right.End;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Segment<TNumber> segment, Segment<TNumber> otherSegment) =>
            IsEmpty(otherSegment) ||
            (!IsEmpty(segment) &&
             segment.Start <= otherSegment.Start &&
             otherSegment.End <= segment.End);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment<TNumber> Intersection(Segment<TNumber> left, Segment<TNumber> right) =>
            Create(
                TNumber.Max(left.Start, right.Start),
                TNumber.Min(left.End, right.End));

        public bool Intersects(Segment<TNumber> left, Segment<TNumber> right) =>
            !IsEmpty(Intersection(left, right));

        public bool Touches(Segment<TNumber> left, Segment<TNumber> right)
        {
            if (IsEmpty(left) || IsEmpty(right)) return false;
            var bounding = Bounding(left, right);
            return Size(bounding) <= Size(left) + Size(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment<TNumber> Bounding(Segment<TNumber> left, Segment<TNumber> right) =>
            IsEmpty(left)
                ? right
                : IsEmpty(right)
                    ? left
                    : Create(
                        TNumber.Min(left.Start, right.Start),
                        TNumber.Max(left.End, right.End));

        public IEnumerable<Segment<TNumber>> Union(Segment<TNumber> left, Segment<TNumber> right)
        {
            var isEmpty1 = IsEmpty(left);
            var isEmpty2 = IsEmpty(right);

            // left  : ∅
            // right : ∅
            // result: ∅
            if (isEmpty1 && isEmpty2)
            {
                yield break;
            }

            // left  : ∅
            // right : [r--)
            // result: [r--)
            if (isEmpty1)
            {
                yield return right;
                yield break;
            }

            // left  : [l--)
            // right : ∅
            // result: [l--)
            if (isEmpty2)
            {
                yield return left;
                yield break;
            }

            // left  : [l--)
            // right :       [r--)
            // result: [l--) [r--)
            if (left.End.CompareTo(right.Start) < 0)
            {
                yield return left;
                yield return right;
                yield break;
            }

            // left  :       [l--)
            // right : [r--)
            // result: [r--) [l--)
            if (right.End.CompareTo(left.Start) < 0)
            {
                yield return right;
                yield return left;
                yield break;
            }

            // left  : [l--)     |      [l--)  |  [l--)        |  [l------)  |    [l--)
            // right :    [r--)  |  [r--)      |        [r--)  |    [r--)    |  [r------)
            // result: [l--r--)  |  [r---l--)  |  [l-----r--)  |  [l-r----)  |  [r-l----)
            yield return Create(
                TNumber.Min(left.Start, right.Start),
                TNumber.Max(left.End, right.End));
        }

        public IEnumerable<Segment<TNumber>> Subtract(Segment<TNumber> left, Segment<TNumber> right)
        {
            // left  : ∅
            // right : ∀
            // result: ∅
            if (IsEmpty(left)) yield break;

            // left  : ∀
            // right : ∅
            // result: [l--)
            if (IsEmpty(right))
            {
                yield return left;
                yield break;
            }

            // left  : [l--)        |        [l--)
            // right :       [r--)  |  [r--)
            // result: [l--)        |        [l--)
            if (left.End <= right.Start || right.End <= left.Start)
            {
                yield return left;
                yield break;
            }

            if (left.Start < right.Start)
            {
                // left  : [l--)
                // right :    [r--)
                // result: [l-)
                if (left.End <= right.End)
                {
                    yield return Create(left.Start, right.Start);
                    yield break;
                }

                // left  : [l--------)
                // right :    [r--)
                // result: [l-)   [l-)
                yield return Create(left.Start, right.Start);
                yield return Create(right.End, left.End);
                yield break;
            }

            if (left.End > right.End)
            {
                // left  :    [l--)
                // right : [r--)
                // result:     [l-)
                yield return Create(right.End, left.End);
                yield break;
            }

            // left  :    [l--)
            // right : [r-------)
            // result: ∅
            yield break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Segment<TNumber> segment, TNumber element) =>
            segment.Start <= element && element < segment.End;

        public ISegmentSet<Segment<TNumber>> CreateSet() =>
            new NumericSegmentSet<TNumber>(this);

        public ISegmentSet<Segment<TNumber>> CreateSet(IEnumerable<Segment<TNumber>> segments)
        {
            var set = CreateSet();
            set.AddRange(segments);
            return set;
        }

        public ISegmentSet<Segment<TNumber>> CreateSet(params Segment<TNumber>[] segments) =>
            CreateSet((IEnumerable<Segment<TNumber>>)segments);

        public IComparer<Segment<TNumber>> StartComparer =>
            NumericSegmentStartComparer<TNumber>.Default;

        public IComparer<Segment<TNumber>> EndComparer =>
            NumericSegmentEndComparer<TNumber>.Default;
    }

    internal class NumericSegmentStartComparer<TNumber> :
        IComparer<Segment<TNumber>>
        where TNumber : struct, INumber<TNumber>
    {
        public static readonly NumericSegmentStartComparer<TNumber> Default = new();

        public int Compare(Segment<TNumber> x, Segment<TNumber> y) =>
            x.Start.CompareTo(y.Start);
    }

    internal class NumericSegmentEndComparer<TNumber> :
        IComparer<Segment<TNumber>>
        where TNumber : struct, INumber<TNumber>
    {
        public static readonly NumericSegmentEndComparer<TNumber> Default = new();

        public int Compare(Segment<TNumber> x, Segment<TNumber> y) =>
            x.End.CompareTo(y.End);
    }

    public class NumericSegmentSet<TNumber> :
        ISegmentSet<Segment<TNumber>>
        where TNumber : struct, INumber<TNumber>
    {
        private readonly NumericSegmentDescriptor<TNumber> descriptor;
        private readonly List<Segment<TNumber>> segments;

        internal NumericSegmentSet(
            NumericSegmentDescriptor<TNumber> descriptor)
        {
            this.descriptor = descriptor;
            segments = new List<Segment<TNumber>>();
        }

        public bool Add(Segment<TNumber> newSegment)
        {
            if (descriptor.IsEmpty(newSegment)) return false;
            if (segments.Count == 0)
            {
                segments.Add(newSegment);
                return true;
            }
            // find where to insert the segment.
            // The start of the segment should be greater than the end of the previous segment
            // and the end of the segment should be less than the start of the next segment.
            // The segment should be merged with any overlapping segments.

            // segments:       [10--)  [20--)  [30--)  [40--)
            // new     : [5--)
            // result  : [5--) [10--)  [20--)  [30--)  [40--)

            // segments:       [10--)  [20--)  [30--)  [40--)
            // new     : [5-----------------------)
            // result  : [5-----10------20------30--)  [40--)

            // segments: [10--)  [20--)  [30--)  [40--)
            // new     :                [25--------------------)
            // result  : [10--)  [20--) [25-------40-----------)

            // segments: [10--)  [20--)  [30--)  [40--)
            // new     :                                 [50--)
            // result  : [10--)  [20--)  [30--)  [40--)  [50--)

            // TODO: Optimize

            var newSegments = new List<Segment<TNumber>>();
            var mergedSegment = newSegment;
            var mergedAdded = false;
            foreach (var segment in segments)
            {
                if (segment.End < mergedSegment.Start)
                {
                    newSegments.Add(segment);
                }
                else if (segment.Start > mergedSegment.End)
                {
                    if (!mergedAdded)
                    {
                        newSegments.Add(mergedSegment);
                        mergedAdded = true;
                    }
                    newSegments.Add(segment);
                }
                else
                {
                    mergedSegment = descriptor.Bounding(segment, mergedSegment);
                }
            }

            if (!mergedAdded)
            {
                newSegments.Add(mergedSegment);
            }

            var changed =
                newSegments.Count != segments.Count ||
                newSegments.Zip(segments).Any(x => !descriptor.AreEqual(x.First, x.Second));

            segments.Clear();
            segments.AddRange(newSegments);

            return changed;
        }

        public bool AddRange(IEnumerable<Segment<TNumber>> newSegments)
        {
            var result = false;
            foreach (var segment in newSegments)
            {
                result |= Add(segment);
            }
            return result;
        }

        public ISegmentSet<Segment<TNumber>> Intersection(Segment<TNumber> newSegment)
        {
            return descriptor.CreateSet(
                segments.Select(s =>
                    descriptor.Intersection(s, newSegment)));
        }

        public bool Intersect(Segment<TNumber> newSegment)
        {
            if (descriptor.IsEmpty(newSegment))
            {
                if (IsEmpty) return false;
                segments.Clear();
                return true;
            }

            var temporary = Intersection(newSegment);
            var changed = !Equals(temporary);

            if (!changed) return false;

            segments.Clear();
            segments.AddRange(temporary);

            return changed;
        }

        public bool Intersects(Segment<TNumber> newSegment) =>
            !Intersection(newSegment).IsEmpty;

        public ISegmentSet<Segment<TNumber>> Subtraction(Segment<TNumber> newSegment)
        {
            return descriptor.CreateSet(
                segments.SelectMany(segment =>
                    descriptor.Subtract(segment, newSegment)));
        }

        public bool Subtract(Segment<TNumber> newSegment)
        {
            if (descriptor.IsEmpty(newSegment))
            {
                return false;
            }

            var temporary = Subtraction(newSegment);
            var changed = !Equals(temporary);

            if (!changed) return false;

            segments.Clear();
            segments.AddRange(temporary);

            return changed;
        }

        public bool IsEmpty => segments.Count == 0;

        public void Clear() => segments.Clear();

        public int Count => segments.Count;

        public IEnumerator<Segment<TNumber>> GetEnumerator()
        {
            return segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IEnumerable<Segment<TNumber>> other) return false;
            if (ReferenceEquals(this, other)) return true;

            using var enumerator = GetEnumerator();
            using var otherEnumerator = other.GetEnumerator();

            while (true)
            {
                var hasNext = enumerator.MoveNext();
                if (hasNext != otherEnumerator.MoveNext()) return false;
                if (!hasNext) return true;

                if (!descriptor.AreEqual(enumerator.Current, otherEnumerator.Current)) return false;
            }
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var segment in segments)
            {
                hash.Add(segment);
            }
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            if (segments.Count == 0) return "∅";

            var sb = new StringBuilder();

            sb.Append("{ ");
            sb.Append(segments[0]);
            foreach (var segment in segments.Skip(1))
            {
                sb.Append(", ");
                sb.Append(segment);
            }
            sb.Append(" }");

            return sb.ToString();
        }
    }
}
