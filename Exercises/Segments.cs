using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Exercises;

public interface ISegmentDescriptor<TSegment>
    where TSegment : notnull
{
    TSegment Empty { get; }
    bool IsEmpty(TSegment segment);
    bool AreEqual(TSegment left, TSegment right);
    bool Contains(TSegment segment, TSegment otherSegment);
    TSegment Intersection(TSegment left, TSegment right);
    TSegment Bounding(TSegment left, TSegment right);
    IEnumerable<TSegment> Union(TSegment left, TSegment right);
    IEnumerable<TSegment> Substract(TSegment left, TSegment right);
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
    bool Add(TSegment newSegment);
    bool AddRange(IEnumerable<TSegment> newSegments);
    void Clear(TSegment segment);
    int Count { get; }
    // TODO: AddRange, Remove, RemoveRange, Contains, Intersect, Union, Substract, Bounding
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

        public IEnumerable<Segment<TNumber>> Substract(Segment<TNumber> left, Segment<TNumber> right)
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
            //var startIndex = FindStart(newSegment.Start);
            //var endIndex = FindEnd(newSegment.End);

            if (newSegment.End < segments[0].Start)
            {
                segments.Insert(0, newSegment);
                return true;
            }

            if (newSegment.Start > segments[^1].End)
            {
                segments.Add(newSegment);
                return true;
            }

            //var index = 0;
            //var toMerge = newSegment;
            //var found = false;
            //var startIndex = 0;
            //var endIndex = 0;
            //while (index < segments.Count)
            //{
            //    var segment = segments[index];
            //    if (segment.End < toMerge.Start)
            //    {
            //        startIndex = index;
            //    }
            //    else if (segment.Start > toMerge.End)
            //    {
            //        endIndex = index;
            //        break;
            //    }
            //    else
            //    {
            //        toMerge = descriptor.Bounding(toMerge, segment);
            //        segments.RemoveAt(index);
            //        merged = true;
            //    }
            //}

            return false;
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

        public void Clear(Segment<TNumber> segment)
        {
            segments.Clear();
        }

        public int Count => segments.Count;

        public IEnumerator<Segment<TNumber>> GetEnumerator()
        {
            return segments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        private int FindStart(TNumber element)
        {
            var left = 0;
            var length = segments.Count;
            while (length > 0)
            {
                var half = length >> 1;
                var middle = left + half;

                var middleSegment = segments[middle];

                if (element < middleSegment.Start)
                {
                    length = half;
                }
                else if (element >= middleSegment.End)
                {
                    left = middle + 1;
                    length -= half + 1;
                }
                else
                {
                    return middle;
                }
            }
            return ~left;
        }

        private int FindEnd(TNumber element)
        {
            var left = 0;
            var length = segments.Count;
            while (length > 0)
            {
                var half = length >> 1;
                var middle = left + half;

                var middleSegment = segments[middle];

                if (element <= middleSegment.Start)
                {
                    length = half;
                }
                else if (element > middleSegment.End)
                {
                    left = middle + 1;
                    length -= half + 1;
                }
                else
                {
                    return middle;
                }
            }
            return ~left;
        }
    }
}
