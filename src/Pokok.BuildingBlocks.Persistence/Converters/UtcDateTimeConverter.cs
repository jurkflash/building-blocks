using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pokok.BuildingBlocks.Persistence.Converters
{
    /// <summary>
    /// EF Core value converter that normalizes <see cref="DateTime"/> values to <see cref="DateTimeKind.Utc"/>
    /// on both read and write, ensuring consistent UTC storage in the database.
    /// </summary>
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UtcDateTimeConverter"/>.
        /// </summary>
        public UtcDateTimeConverter()
            : base(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}
