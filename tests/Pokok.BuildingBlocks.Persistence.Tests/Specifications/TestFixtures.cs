namespace Pokok.BuildingBlocks.Persistence.Specifications;

internal class Product
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal Price { get; set; }
}
