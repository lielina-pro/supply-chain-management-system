namespace SCM.Application.Common.Interfaces;

/// <summary>Resolves the acting user for the current request (used for audit fields like ArchivedByUserId).</summary>
public interface ICurrentUserService
{
    int?    UserId { get; }
    string? Role   { get; }
}
