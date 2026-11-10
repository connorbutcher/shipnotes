namespace ShipNotes.Domain.Applications;

/// <summary>The stored form of an API key: its hash and a short, non-secret prefix for display.</summary>
public sealed record ApiKeyCredential(string Hash, string Prefix);
