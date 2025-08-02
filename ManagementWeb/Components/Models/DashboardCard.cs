namespace ManagementWeb.Components.Models
{
    public class DashboardCard
    {
        public string Title { get; }
        public string Value { get; }
        public string Icon { get; }
        public string BackgroundColor { get; }
        public string MainColor { get; }
        public string ValueColor => "#000";

        public DashboardCard(string title, string value, string icon, string bgColor, string mainColor)
        {
            Title = title;
            Value = value;
            Icon = icon;
            BackgroundColor = bgColor;
            MainColor = mainColor;
        }
    }
}
