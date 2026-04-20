
using System;
using System.IO.Compression;
using System.Diagnostics;
using System.Text.Json;
using test.Model;
using System.Net.Http;

namespace test;
public class UpdateVpnCLient{
    public string url = "https://github.com/S1et8laz/Vpn-Client/releases/download/v18";
    public string name_file = String.Empty;
    public static HttpClient client = new HttpClient();
    public byte[] data = Array.Empty<byte>();
    public List<Releases> releases = new List<Releases>();
    public Releases Latest = new Releases();
    public List<assets> assets = new List<assets>();
    public static async Task Main(string[] args){
        if(args.Length>2){
            Console.WriteLine("слишком много аргументов");
            return;
        }
        else if(args.Length < 2){
            Console.WriteLine("Введите аргумент для выбора системы, и текущей версии");
            return;
        }
        Console.WriteLine($"Система = {args[0]}");
        var Updater = new UpdateVpnCLient();
        await Updater.DownloadJson();
        Updater.Deserialisation();
        if(Updater.setName(args[0].ToLower())){
            try{
                Console.WriteLine("Запуск процесса...");
                if(Version.Parse(Updater.Latest.tag_name) < Version.Parse(args[1])){
                    await Updater.Downloadzip(args[0],args[1]);
                    Updater.StartProc();
                }
                else{
                    
                }
                
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
             
        }
        Console.WriteLine("Updater закрывается");

    }
    public void Deserialisation(){
        var OsRelease = releases.Where(c=>c.name.Contains("Desktop"))
            .MaxBy(c=>c.published_at);
        if(OsRelease != null) Latest = OsRelease;
        else throw new Exception("нет релиза");
        assets = Latest.assets;
        

        foreach(var asset in Latest.assets){
            Console.WriteLine($"name = {asset.name}");
        }
    }
    public async Task DownloadJson(){
        Console.WriteLine(InfoModel.GetUrlApi());
        client.DefaultRequestHeaders.Add("User-Agent", "Vpn_Client-Update");

        var response = await client.GetStringAsync(InfoModel.GetUrlApi());
        
        if(response!=null) releases = JsonSerializer.Deserialize<List<Releases>>(response);
        else throw new Exception("не смог десериализовать");
        

    }
    public bool setName(string Osname){
        switch(Osname){
            case "linux": name_file = "vpn-client-linux.zip";
                            return true;
            case "windows": name_file = "vpn-client-windows.zip";
                            return true;
            default: return false;
        }
    }
    public async Task<bool> CheckVersion(){
        var response = await client.GetStringAsync(url);

        return true;
    }
    public async Task Downloadzip(string Os, string version){
        try{
            Console.WriteLine("Запуск процесса...");
            
            
            string urldownload = assets.Where(c=>c.name.Contains(Os)).FirstOrDefault().browser_download_url;
            data = await client.GetByteArrayAsync($"{urldownload}");
            FileInfo fi = new FileInfo($"./{name_file}");
            using(FileStream fs = fi.Create()){
                fs.Write(data, 0, data.Length);
                fs.Close();
                Console.WriteLine("Запуск процесса...");
            }
            await Clear();
            await UnZip();
            fi.Delete();
                
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }
        
    }
    public async Task Clear()
    {
        Process[] VpnClient = Process.GetProcessesByName("Vpn-Client.Desktop");
            if(VpnClient.Length > 0){
                Console.WriteLine($"Запущенно {VpnClient.Length} процессов");
                int i = 1;
                foreach(var proc in VpnClient){
                    Console.WriteLine($"Процесс {i}, имя {proc.ProcessName} убит");
                    if(proc.CloseMainWindow()){
                        if(!proc.WaitForExit(2000)){
                            Console.WriteLine("Окно закрылось");
                            proc.Kill();
                        }
                    }
                    else proc.Kill();
                    proc.Dispose();
                
                }
            }
            else{
                Console.WriteLine("Процесс не запущен");
            }

    }
    public async Task UnZip()=> await ZipFile.ExtractToDirectoryAsync($"./{name_file}","../", overwriteFiles: true);

    public void StartProc(){
        try{
            using Process proc = Process.Start("../Vpn-Client.Desktop");
                               
        }
        catch(Exception e){
            Console.WriteLine(e);
        }
    }
}
