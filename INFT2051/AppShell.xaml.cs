namespace INFT2051
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(DiaryEditPage), typeof(DiaryEditPage));
            Routing.RegisterRoute(nameof(SettingPage), typeof(SettingPage));
        }
    }
}
