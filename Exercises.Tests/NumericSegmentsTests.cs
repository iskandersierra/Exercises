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
    [InlineData(0L, 0L, 0L, 0L, 0L, true)]
    [InlineData(10L, 5L, 0L, 0L, 0L, true)]
    [InlineData(10L, 10L, 0L, 0L, 0L, true)]
    [InlineData(0L, 10L, 0L, 10L, 10L, false)]
    [InlineData(10L, 20L, 10L, 20L, 10L, false)]
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
    [InlineData(0L, 0L, 0L, 0L, true)]
    [InlineData(10L, 20L, 0L, 0L, true)]
    [InlineData(0L, 0L, 10L, 20L, false)]
    [InlineData(10L, 20L, 10L, 20L, true)]
    [InlineData(10L, 20L, 12L, 18L, true)]
    [InlineData(10L, 20L, 12L, 20L, true)]
    [InlineData(10L, 20L, 10L, 18L, true)]
    [InlineData(10L, 20L, 8L, 18L, false)]
    [InlineData(10L, 20L, 12L, 22L, false)]
    [InlineData(10L, 20L, 20L, 22L, false)]
    [InlineData(10L, 20L, 8L, 10L, false)]
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
    [InlineData(0L, 0L, 0L, 0L, 0L, 0L)]
    [InlineData(0L, 0L, 10L, 20L, 0L, 0L)]
    [InlineData(10L, 20L, 0L, 0L, 0L, 0L)]
    [InlineData(10L, 20L, 10L, 20L, 10L, 20L)]
    [InlineData(10L, 20L, 15L, 25L, 15L, 20L)]
    [InlineData(10L, 20L, 5L, 15L, 10L, 15L)]
    [InlineData(10L, 20L, 5L, 25L, 10L, 20L)]
    [InlineData(5L, 25L, 10L, 20L, 10L, 20L)]
    [InlineData(10L, 20L, 20L, 30L, 20L, 20L)]
    [InlineData(20L, 30L, 10L, 20L, 20L, 20L)]
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
    [InlineData(0L, 0L, 0L, 0L, 0L, 0L)]
    [InlineData(0L, 0L, 10L, 20L, 10L, 20L)]
    [InlineData(10L, 20L, 0L, 0L, 10L, 20L)]
    [InlineData(10L, 20L, 10L, 20L, 10L, 20L)]
    [InlineData(10L, 20L, 15L, 25L, 10L, 25L)]
    [InlineData(10L, 20L, 5L, 15L, 5L, 20L)]
    [InlineData(10L, 20L, 5L, 25L, 5L, 25L)]
    [InlineData(5L, 25L, 10L, 20L, 5L, 25L)]
    [InlineData(10L, 20L, 20L, 30L, 10L, 30L)]
    [InlineData(20L, 30L, 10L, 20L, 10L, 30L)]
    [InlineData(10L, 20L, 30L, 40L, 10L, 40L)]
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
}
