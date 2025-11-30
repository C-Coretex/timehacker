namespace TimeHacker.Helpers.Domain.Abstractions.Delegates;

// ReSharper disable once GrammarMistakeInComment
/// <summary>
/// Encapsulates Includes and any filtering logic for queries.
/// </summary>
public delegate IQueryable<T> QueryPipelineStep<T>(IQueryable<T> query);
