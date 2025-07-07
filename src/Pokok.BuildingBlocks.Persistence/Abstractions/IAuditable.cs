﻿namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    public interface IAuditable
    {
        DateTime CreatedAtUtc { get; set; }
        string? CreatedBy { get; set; }
        DateTime? ModifiedAtUtc { get; set; }
        string? ModifiedBy { get; set; }
    }
}
