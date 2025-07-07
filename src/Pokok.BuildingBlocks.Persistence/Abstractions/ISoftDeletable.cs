﻿namespace Pokok.BuildingBlocks.Persistence.Abstractions
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAtUtc { get; set; }
        string? DeletedBy { get; set; }
    }
}
