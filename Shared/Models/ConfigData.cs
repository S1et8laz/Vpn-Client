namespace Shared.Models
{


    public static class InfoModel{
        private static readonly string owner = "S1et8laz";
        private static readonly string repo = "Vpn-Client";
        private static readonly string UrlApi = $"https://api.github.com/repos/{owner}/{repo}/releases";
        public static string GetUrlApi(){return UrlApi;}
    }
}
