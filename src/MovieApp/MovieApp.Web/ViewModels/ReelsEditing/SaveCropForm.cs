namespace MovieApp.Web.ViewModels.ReelsEditing
{
    public sealed class SaveCropForm
    {
        public int ReelId { get; set; }
        public double CropMarginLeft { get; set; }
        public double CropMarginTop { get; set; }
        public double CropMarginRight { get; set; }
        public double CropMarginBottom { get; set; }
    }
}
