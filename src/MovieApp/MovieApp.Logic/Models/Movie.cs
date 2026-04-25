using System;

namespace MovieApp.Logic.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; }

        public double Rating { get; set; }

        public decimal Price { get; set; }

        public string PrimaryGenre { get; set; } = string.Empty;

        public string Genre { get => PrimaryGenre; set => PrimaryGenre = value; }

        public string? ImageUrl { get; set; }

        public string PosterUrl { get; set; } = string.Empty;

        public int ReleaseYear { get; set; }

        public bool IsOnSale { get; set; }

        public decimal? ActiveSaleDiscountPercent { get; set; }

        public string Synopsis { get; set; } = string.Empty;

        public bool HasActiveSale => ActiveSaleDiscountPercent is decimal d && d > 0;

        public decimal GetEffectivePrice() => HasActiveSale ? decimal.Round(Price * (1 - (ActiveSaleDiscountPercent!.Value / 100m)), 2, MidpointRounding.AwayFromZero)

        public decimal GetDiscountedPrice(double discountPercentage) => Price * (1 - (decimal)(discountPercentage / 100.0));

        public string OriginalPriceText => Price.ToString("0.00");

        public string DiscountedPriceText => GetEffectivePrice().ToString("0.00");

        public override string ToString() => $"{Title} ({ReleaseYear}) — {Genre}";
    }
}
