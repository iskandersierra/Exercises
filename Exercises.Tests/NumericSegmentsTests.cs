namespace Exercises.Tests;

public class NumericSegmentsTests
{
    [Fact]
    public void Empty()
    {
        var descriptor = Segments.Int64;
        var segment = descriptor.Empty;

        Assert.True(descriptor.IsEmpty(segment));
        Assert.Equal(0, descriptor.Size(segment));
        Assert.Equal(0, segment.Start);
        Assert.Equal(0, segment.End);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0, true)]
    [InlineData(10, 5, 0, 0, 0, true)]
    [InlineData(10, 10, 0, 0, 0, true)]
    [InlineData(0, 10, 0, 10, 10, false)]
    [InlineData(10, 20, 10, 20, 10, false)]
    public void Create(
        long start, long end,
        long expectedStart, long expectedEnd,
        long expectedSize, bool expectedIsEmpty)
    {
        var descriptor = Segments.Int64;
        var segment = descriptor.Create(start, end);

        Assert.Equal(expectedIsEmpty, descriptor.IsEmpty(segment));
        Assert.Equal(expectedSize, descriptor.Size(segment));
        Assert.Equal(expectedStart, segment.Start);
        Assert.Equal(expectedEnd, segment.End);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, true)]
    [InlineData(10, 20, 0, 0, true)]
    [InlineData(0, 0, 10, 20, false)]
    [InlineData(10, 20, 10, 20, true)]
    [InlineData(10, 20, 12, 18, true)]
    [InlineData(10, 20, 12, 20, true)]
    [InlineData(10, 20, 10, 18, true)]
    [InlineData(10, 20, 8, 18, false)]
    [InlineData(10, 20, 12, 22, false)]
    [InlineData(10, 20, 20, 22, false)]
    [InlineData(10, 20, 8, 10, false)]
    public void Contains(
        long start1, long end1,
        long start2, long end2,
        bool expectedContains)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);

        Assert.Equal(expectedContains, descriptor.Contains(segment1, segment2));
    }

    [Theory]
    [InlineData(0, 0, 0, false)]
    [InlineData(10, 20, 0, false)]
    [InlineData(10, 20, 10, true)]
    [InlineData(10, 20, 15, true)]
    [InlineData(10, 20, 19, true)]
    [InlineData(10, 20, 20, false)]
    [InlineData(10, 20, 30, false)]
    public void ContainsElement(
        long start, long end,
        long element,
        bool expectedContains)
    {
        var descriptor = Segments.Int64;
        var segment = descriptor.Create(start, end);

        Assert.Equal(expectedContains, descriptor.Contains(segment, element));
    }
    
    [Theory]
    [InlineData(0, 0, 0, 0, true)]
    [InlineData(10, 10, 0, 0, true)]
    [InlineData(20, 10, 0, 0, true)]
    [InlineData(10, 20, 10, 20, true)]
    [InlineData(10, 20, 20, 30, false)]
    public void AreEqual(
        long start1, long end1,
        long start2, long end2,
        bool expectedAreEqual)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);

        Assert.Equal(expectedAreEqual, descriptor.AreEqual(segment1, segment2));
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(0, 0, 10, 20, 0, 0)]
    [InlineData(10, 20, 0, 0, 0, 0)]
    [InlineData(10, 20, 10, 20, 10, 20)]
    [InlineData(10, 20, 15, 25, 15, 20)]
    [InlineData(10, 20, 5, 15, 10, 15)]
    [InlineData(10, 20, 5, 25, 10, 20)]
    [InlineData(5, 25, 10, 20, 10, 20)]
    [InlineData(10, 20, 20, 30, 0, 0)]
    [InlineData(20, 30, 10, 20, 0, 0)]
    [InlineData(10, 20, 30, 40, 0, 0)]
    [InlineData(30, 40, 10, 20, 0, 0)]
    public void Intersection(
        long start1, long end1,
        long start2, long end2,
        long expectedStart, long expectedEnd)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);
        var expectedIntersection = descriptor.Create(expectedStart, expectedEnd);

        Assert.Equal(expectedIntersection, descriptor.Intersection(segment1, segment2));
    }

    [Theory]
    [InlineData(0, 0, 0, 0, false)]
    [InlineData(0, 0, 10, 20, false)]
    [InlineData(10, 20, 0, 0, false)]
    [InlineData(10, 20, 10, 20, true)]
    [InlineData(10, 20, 15, 25, true)]
    [InlineData(10, 20, 5, 15, true)]
    [InlineData(10, 20, 5, 25, true)]
    [InlineData(5, 25, 10, 20, true)]
    [InlineData(10, 20, 20, 30, false)]
    [InlineData(20, 30, 10, 20, false)]
    [InlineData(10, 20, 30, 40, false)]
    [InlineData(30, 40, 10, 20, false)]
    public void Intersects(
        long start1, long end1,
        long start2, long end2,
        bool expectedIntersects)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);

        Assert.Equal(expectedIntersects, descriptor.Intersects(segment1, segment2));
    }

    [Theory]
    [InlineData(0, 0, 0, 0, false)]
    [InlineData(0, 0, 10, 20, false)]
    [InlineData(10, 20, 0, 0, false)]
    [InlineData(10, 20, 10, 20, true)]
    [InlineData(10, 20, 15, 25, true)]
    [InlineData(10, 20, 5, 15, true)]
    [InlineData(10, 20, 5, 25, true)]
    [InlineData(5, 25, 10, 20, true)]
    [InlineData(10, 20, 20, 30, true)]
    [InlineData(20, 30, 10, 20, true)]
    [InlineData(10, 20, 30, 40, false)]
    [InlineData(30, 40, 10, 20, false)]
    public void Touches(
        long start1, long end1,
        long start2, long end2,
        bool expectedTouches)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);

        Assert.Equal(expectedTouches, descriptor.Touches(segment1, segment2));
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(0, 0, 10, 20, 10, 20)]
    [InlineData(10, 20, 0, 0, 10, 20)]
    [InlineData(10, 20, 10, 20, 10, 20)]
    [InlineData(10, 20, 15, 25, 10, 25)]
    [InlineData(10, 20, 5, 15, 5, 20)]
    [InlineData(10, 20, 5, 25, 5, 25)]
    [InlineData(5, 25, 10, 20, 5, 25)]
    [InlineData(10, 20, 20, 30, 10, 30)]
    [InlineData(20, 30, 10, 20, 10, 30)]
    [InlineData(10, 20, 30, 40, 10, 40)]
    public void Bounding(
        long start1, long end1,
        long start2, long end2,
        long expectedStart, long expectedEnd)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);
        var expectedBounding = descriptor.Create(expectedStart, expectedEnd);

        Assert.Equal(expectedBounding, descriptor.Bounding(segment1, segment2));
    }

    [Theory]
    [InlineData(0, 0, 0, 0, new long[] { })]
    [InlineData(10, 20, 0, 0, new long[] { 10, 20 })]
    [InlineData(0, 0, 10, 20, new long[] { 10, 20 })]
    [InlineData(0, 10, 10, 20, new long[] { 0, 20 })]
    [InlineData(0, 10, 20, 30, new long[] { 0, 10, 20, 30 })]
    [InlineData(10, 20, 0, 10, new long[] { 0, 20 })]
    [InlineData(20, 30, 0, 10, new long[] { 0, 10, 20, 30 })]
    public void Union(
        long start1, long end1,
        long start2, long end2,
        long[] expectedRanges)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);
        var expectedUnion = expectedRanges
            .Chunk(2)
            .Select(x => descriptor.Create(x[0], x[1]))
            .ToArray();

        Assert.Equal(expectedUnion, descriptor.Union(segment1, segment2));
    }

    [Theory]
    [InlineData(0, 0, 0, 0, new long[] { })]
    [InlineData(10, 20, 0, 0, new long[] { 10, 20 })]
    [InlineData(0, 0, 10, 20, new long[] { })]
    [InlineData(10, 20, 20, 30, new long[] { 10, 20 })]
    [InlineData(10, 20, 10, 20, new long[] { })]
    [InlineData(10, 20, 15, 25, new long[] { 10, 15 })]
    [InlineData(10, 20, 5, 25, new long[] { })]
    [InlineData(10, 20, 5, 15, new long[] { 15, 20 })]
    [InlineData(5, 25, 10, 20, new long[] { 5, 10, 20, 25 })]
    public void Subtract(
        long start1, long end1,
        long start2, long end2,
        long[] expectedRanges)
    {
        var descriptor = Segments.Int64;
        var segment1 = descriptor.Create(start1, end1);
        var segment2 = descriptor.Create(start2, end2);
        var expectedSubtract = expectedRanges
            .Chunk(2)
            .Select(x => descriptor.Create(x[0], x[1]))
            .ToArray();

        Assert.Equal(expectedSubtract, descriptor.Subtract(segment1, segment2));
    }

    [Theory]
    [InlineData(new long[] { }, 0, 0, false, new long[] { })]
    [InlineData(new long[] { 10, 20 }, 0, 0, false, new long[] { 10, 20 })]
    [InlineData(new long[] { 10, 20, 30, 40 }, 0, 0, false, new long[] { 10, 20, 30, 40 })]
    [InlineData(new long[] { 10, 20 }, 10, 20, false, new long[] { 10, 20 })]
    [InlineData(new long[] { 10, 20 }, 0, 5, true, new long[] { 0, 5, 10, 20 })]
    [InlineData(new long[] { 10, 20 }, 30, 40, true, new long[] { 10, 20, 30, 40 })]
    [InlineData(new long[] { 10, 20, 30, 40 }, 20, 30, true, new long[] { 10, 40 })]
    [InlineData(new long[] { 10, 20, 30, 40 }, 22, 28, true, new long[] { 10, 20, 22, 28, 30, 40 })]
    [InlineData(new long[] { 10, 20, 30, 40 }, 25, 35, true, new long[] { 10, 20, 25, 40 })]
    [InlineData(new long[] { 10, 20, 30, 40 }, 15, 25, true, new long[] { 10, 25, 30, 40 })]
    public void SetAdd(
        long[] startRanges,
        long start, long end,
        bool expectedChanged,
        long[] expectedRanges)
    {
        var descriptor = Segments.Int64;

        var set = descriptor.CreateSet();
        set.AddRange(startRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        var segment = descriptor.Create(start, end);
        
        var expectedResult = expectedRanges
            .Chunk(2)
            .Select(x => descriptor.Create(x[0], x[1]))
            .ToArray();

        var changed = set.Add(segment);

        Assert.Equal(expectedResult, set);
        Assert.Equal(expectedChanged, changed);
    }

    [Theory]
    [InlineData(new long[] { })]
    [InlineData(new long[] { 10, 20 })]
    [InlineData(new long[] { 10, 20, 30, 40 })]
    public void SetClear(long[] startRanges)
    {
        var descriptor = Segments.Int64;

        var set = descriptor.CreateSet();
        set.AddRange(startRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        set.Clear();

        Assert.Equal([], set);
    }

    [Theory]
    [InlineData(new long[] { }, "\u2205")]
    [InlineData(new long[] { 10, 20 }, "{ [10, 20) }")]
    [InlineData(new long[] { 10, 20, 30, 40 }, "{ [10, 20), [30, 40) }")]
    public void SetToString(long[] startRanges, string expected)
    {
        var descriptor = Segments.Int64;

        var set = descriptor.CreateSet();
        set.AddRange(startRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        Assert.Equal(expected, set.ToString());
    }

    [Theory]
    [InlineData(new long[] { }, 0)]
    [InlineData(new long[] { 10, 20 }, 1)]
    [InlineData(new long[] { 10, 20, 30, 40 }, 2)]
    public void SetCount(long[] startRanges, int expectedCount)
    {
        var descriptor = Segments.Int64;

        var set = descriptor.CreateSet();
        set.AddRange(startRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        Assert.Equal(expectedCount, set.Count);
    }

    [Theory]
    [InlineData(new long[] { }, new long[] { }, true)]
    [InlineData(new long[] { }, new long[] { 10, 20 }, false)]
    [InlineData(new long[] { 10, 20 }, new long[] { }, false)]
    [InlineData(new long[] { 10, 20 }, new long[] { 10, 20 }, true)]
    [InlineData(new long[] { 10, 20 }, new long[] { 10, 20, 30, 40 }, false)]
    [InlineData(new long[] { 10, 20, 30, 40 }, new long[] { 10, 20 }, false)]
    [InlineData(new long[] { 10, 20, 30, 40 }, new long[] { 10, 20, 30, 40 }, true)]
    [InlineData(new long[] { 10, 20, 30, 40 }, new long[] { 30, 40, 10, 20 }, true)]
    [InlineData(new long[] { 10, 20, 30, 40 }, new long[] { 10, 20, 25, 35 }, false)]
    public void SetEqualsAndHashCode(
        long[] leftRanges,
        long[] rightRanges,
        bool expectedEquals)
    {
        var descriptor = Segments.Int64;

        var leftSet = descriptor.CreateSet();
        leftSet.AddRange(leftRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        var rightSet = descriptor.CreateSet();
        rightSet.AddRange(rightRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        Assert.False(leftSet.Equals("value"));
        Assert.True(leftSet.Equals(leftSet));
        Assert.True(rightSet.Equals(rightSet));

        var equals = leftSet.Equals(rightSet);

        Assert.Equal(expectedEquals, equals);

        if (expectedEquals)
        {
            Assert.Equal(leftSet.GetHashCode(), rightSet.GetHashCode());
        }
        else
        {
            Assert.NotEqual(leftSet.GetHashCode(), rightSet.GetHashCode());
        }
    }

    [Theory]
    [InlineData(new long[] { }, 0, 0, new long[] { }, false, false)]
    [InlineData(new long[] { 10, 20 }, 0, 0, new long[] { }, true, false)]
    [InlineData(new long[] { }, 10, 20, new long[] { }, false, false)]
    [InlineData(new long[] { 10, 20 }, 10, 20, new long[] { 10, 20 }, false, true)]
    [InlineData(new long[] { 10, 20, 30, 40, 50, 60 }, 15, 35, new long[] { 15, 20, 30, 35 }, true, true)]
    [InlineData(new long[] { 10, 20, 30, 40, 50, 60 }, 55, 100, new long[] { 55, 60 }, true, true)]
    [InlineData(new long[] { 10, 20, 30, 40, 50, 60 }, 65, 100, new long[] { }, true, false)]
    public void SetIntersection(
        long[] startRanges,
        long start, long end,
        long[] expectedRanges,
        bool expectedChanged,
        bool expectedIntersects)
    {
        var descriptor = Segments.Int64;

        var set = descriptor.CreateSet(startRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        var segment = descriptor.Create(start, end);
        
        var expectedResult = expectedRanges
            .Chunk(2)
            .Select(x => descriptor.Create(x[0], x[1]))
            .ToArray();

        var result = set.Intersection(segment);
        var changed = set.Intersect(segment);
        var intersects = set.Intersects(segment);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedChanged, changed);
        Assert.Equal(expectedResult, set);
        Assert.Equal(expectedIntersects, intersects);
    }

    [Theory]
    [InlineData(new long[] { }, 0, 0, new long[] { }, false)]
    [InlineData(new long[] { 10, 20 }, 0, 0, new long[] { 10, 20 }, false)]
    [InlineData(new long[] { }, 10, 20, new long[] { }, false)]
    [InlineData(new long[] { 10, 20 }, 10, 20, new long[] { }, true)]
    [InlineData(new long[] { 10, 20, 30, 40, 50, 60 }, 15, 35, new long[] { 10, 15, 35, 40, 50, 60 }, true)]
    [InlineData(new long[] { 10, 20, 30, 40, 50, 60 }, 55, 100, new long[] { 10, 20, 30, 40, 50, 55 }, true)]
    [InlineData(new long[] { 10, 20, 30, 40, 50, 60 }, 65, 100, new long[] { 10, 20, 30, 40, 50, 60 }, false)]
    public void SetSubtraction(
        long[] startRanges,
        long start, long end,
        long[] expectedRanges,
        bool expectedChanged)
    {
        var descriptor = Segments.Int64;

        var set = descriptor.CreateSet(startRanges.Chunk(2).Select(x => descriptor.Create(x[0], x[1])));

        var segment = descriptor.Create(start, end);
        
        var expectedResult = expectedRanges
            .Chunk(2)
            .Select(x => descriptor.Create(x[0], x[1]))
            .ToArray();

        var result = set.Subtraction(segment);
        var changed = set.Subtract(segment);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedChanged, changed);
        Assert.Equal(expectedResult, set);
    }
}
