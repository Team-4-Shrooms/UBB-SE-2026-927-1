using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
        public string Genre { get => PrimaryGenre; set => PrimaryGenre = value; }
        public string? PosterUrl { get; set; }
        public int ReleaseYear { get; set; }
        public bool IsOnSale { get; set; }
        public decimal? ActiveSaleDiscountPercent { get; set; }
        public string Synopsis { get; set; } = string.Empty;

        public ActiveSale? ActiveSale { get; set; }

        [NotMapped]
        public string OriginalPriceText => Price.ToString("0.00");
        [NotMapped]
        public string DiscountedPriceText => GetEffectivePrice().ToString("0.00");

        public bool HasActiveSale => ActiveSaleDiscountPercent is decimal d && d > 0;
        public decimal GetEffectivePrice() => HasActiveSale ? decimal.Round(Price * (1 - (ActiveSaleDiscountPercent!.Value / 100m)), 2, MidpointRounding.AwayFromZero) : Price;
        public decimal GetDiscountedPrice(double discountPercentage) => Price * (1 - (decimal)(discountPercentage / 100.0));
        public override string ToString() => $"{Title} ({ReleaseYear}) — {Genre}";
    }
}
