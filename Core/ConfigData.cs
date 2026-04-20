using System;
using System.Text.Json.Serialization;

namespace test.Model{
    
    public static class InfoModel{
        private static readonly string owner = "S1et8laz";
        private static readonly string repo = "Vpn-Client";
        private static readonly string UrlApi = $"https://api.github.com/repos/{owner}/{repo}/releases";
        public static string GetUrlApi(){return UrlApi;}
    }
    public class Releases{
        public DateTime published_at {get;set;}
        public string name {get;set;}
        public string tag_name{get;set;}
        public List<assets> assets  {get;set;}
        
    }

    public class assets{
        public string browser_download_url {get;set;}
        public int size  {get;set;}
        public string name {get;set;}
    }




}
