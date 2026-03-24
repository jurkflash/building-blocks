using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class DateTimeRangeTests
{
    [Fact]
    public void Constructor_WithValidStartAndEnd_CreatesRange()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddHours(1);

        var range = new DateTimeRange(start, end);

        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void Constructor_WithEndBeforeStart_ThrowsDomainException()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddHours(-1);

        var exception = Assert.Throws<DomainException>(() => new DateTimeRange(start, end));

        Assert.Equal("End must be greater than or equal to start.", exception.Message);
    }

    [Fact]
    public void Constructor_WithStartEqualToEnd_CreatesRange()
    {
        var point = DateTimeOffset.UtcNow;

        var range = new DateTimeRange(point, point);

        Assert.Equal(point, range.Start);
        Assert.Equal(point, range.End);
    }

    [Fact]
    public void Contains_WithValueInsideRange_ReturnsTrue()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddHours(2);
        var range = new DateTimeRange(start, end);

        var result = range.Contains(start.AddHours(1));

        Assert.True(result);
    }

    [Fact]
    public void Contains_WithValueOutsideRange_ReturnsFalse()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddHours(2);
        var range = new DateTimeRange(start, end);

        var result = range.Contains(start.AddHours(3));

        Assert.False(result);
    }

    [Fact]
    public void Overlaps_WithOverlappingRange_ReturnsTrue()
    {
        var start = DateTimeOffset.UtcNow;
        var range1 = new DateTimeRange(start, start.AddHours(2));
        var range2 = new DateTimeRange(start.AddHours(1), start.AddHours(3));

        var result = range1.Overlaps(range2);

        Assert.True(result);
    }

    [Fact]
    public void Overlaps_WithNonOverlappingRange_ReturnsFalse()
    {
        var start = DateTimeOffset.UtcNow;
        var range1 = new DateTimeRange(start, start.AddHours(1));
        var range2 = new DateTimeRange(start.AddHours(2), start.AddHours(3));

        var result = range1.Overlaps(range2);

        Assert.False(result);
    }

    [Fact]
    public void Duration_WithOneHourRange_ReturnsOneHour()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddHours(1);
        var range = new DateTimeRange(start, end);

        Assert.Equal(TimeSpan.FromHours(1), range.Duration);
    }

    [Fact]
    public void Equals_TwoRangesWithSameStartAndEnd_ReturnsTrue()
    {
        var start = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddDays(1);

        var range1 = new DateTimeRange(start, end);
        var range2 = new DateTimeRange(start, end);

        Assert.Equal(range1, range2);
    }
}
