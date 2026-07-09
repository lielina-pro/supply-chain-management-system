namespace SCM.Domain.Common;

/// <summary>
/// Every table in the ER diagram uses an int identity primary key
/// (not a Guid), so all entities inherit from this.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}

/// <summary>
/// Matches the recurring IsArchived / ArchivedAt / ArchivedByUserId columns
/// used throughout the ER for soft-deletable records (BR-16).
/// Entities that are permanent, immutable ledgers (e.g. StockTransactions)
/// deliberately do NOT implement this.
/// </summary>
public interface IArchivable
{
    bool      IsArchived       { get; set; }
    DateTime? ArchivedAt       { get; set; }
    int?      ArchivedByUserId { get; set; }
}

/// <summary>
/// CreatedAt is on almost every table. UpdatedAt only exists on a few
/// (e.g. Users) - apply per-entity, not globally.
/// </summary>
public interface ICreatedAtEntity
{
    DateTime CreatedAt { get; set; }
}
