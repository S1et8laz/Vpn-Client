using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
namespace Shared;

public class UpdateService
{
    private readonly HttpClient _client;
    public UpdateService(){
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "VpnClient");
    }
    public async Task<List<Release>> GetReleases(string url)
    {
        var response = await _client.GetStringAsync(url);
        return JsonSerializer.Deserialize<List<Release>>(response)
            ?? new List<Release>();
    }
    public Release GetLatestRelease(List<Release> releases)
    {
        var LatestRelease = releases
            .Where(c=>c.name.Contains("Desktop"))
            .MaxBy(c=>c.published_at);
        if(LatestRelease == null){
            throw new Exception("Нет подходящего релиза");
        }
        return LatestRelease;
    }
    public bool IsNewVersion(string CurentVersion, string LastVersion)
    {
        return Version.Parse(LastVersion) > Version.Parse(CurentVersion);
    }
    public string GetDownloadUrl(Release release, string os)
    {
        var asset = release.assets
            .FirstOrDefault(c=>c.name.ToLower().Contains(os.ToLower()));
        if (asset == null) throw new Exception("Ассет не найден");
        return asset.browser_download_url;
    }

}
