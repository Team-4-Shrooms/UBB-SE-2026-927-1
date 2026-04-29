namespace MovieApp.WebDTOs.DTOs;

public sealed class UserReferenceDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
}

public sealed class MovieReferenceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public int ReleaseYear { get; set; }
    public string PrimaryGenre { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public sealed class ReelReferenceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
}

public sealed class MovieEventReferenceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
}

public sealed class ScrapeJobReferenceDto
{
    public int Id { get; set; }
    public string SearchQuery { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public sealed class EquipmentReferenceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
