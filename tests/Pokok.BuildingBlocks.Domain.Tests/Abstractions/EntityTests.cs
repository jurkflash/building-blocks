using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Events;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.Abstractions;

internal sealed class SampleEntity : Entity<int>
{
    public SampleEntity(int id) : base(id) { }
}

internal sealed class DefaultEntity : Entity<int>
{
    public DefaultEntity() : base() { }
}

public class EntityTests
{
    [Fact]
    public void DefaultConstructor_WhenCalled_CreatesEntityWithDefaultId()
    {
        var entity = new DefaultEntity();

        Assert.Equal(default, entity.Id);
    }

    [Fact]
    public void Equals_EntityComparedToNonEntity_ReturnsFalse()
    {
        var entity = new SampleEntity(1);

        var result = entity.Equals("not an entity");

        Assert.False(result);
    }

    [Fact]
    public void Equals_TwoEntitiesWithSameId_ReturnsTrue()
    {
        var entity1 = new SampleEntity(1);
        var entity2 = new SampleEntity(1);

        var result = entity1.Equals(entity2);

        Assert.True(result);
    }

    [Fact]
    public void Equals_TwoEntitiesWithDifferentIds_ReturnsFalse()
    {
        var entity1 = new SampleEntity(1);
        var entity2 = new SampleEntity(2);

        var result = entity1.Equals(entity2);

        Assert.False(result);
    }

    [Fact]
    public void Equals_EntityComparedToNull_ReturnsFalse()
    {
        var entity = new SampleEntity(1);

        var result = entity.Equals(null);

        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_EntitiesWithSameId_ReturnsSameHashCode()
    {
        var entity1 = new SampleEntity(42);
        var entity2 = new SampleEntity(42);

        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void Id_WhenConstructedWithId_ReturnsExpectedId()
    {
        var entity = new SampleEntity(99);

        Assert.Equal(99, entity.Id);
    }
}
