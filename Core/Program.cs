
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
    public string PathToDirectory {get;set;}
    public static async Task Main(string[] args){
        if(args.Length>3){
            Console.WriteLine("слишком много аргументов");
            return;
        }
        else if(args.Length < 3){
            Console.WriteLine("Введите аргумент для выбора системы, текущей версии и путь к директории");
            return;
        }
        Console.WriteLine($"Система = {args[0]},  тэг {args[1]}, директория {args[2]}");
        var Updater = new UpdateVpnCLient();
        client.DefaultRequestHeaders.Add("User-Agent", "Vpn_Client-Update");

        Updater.PathToDirectory = args[2];
        await Updater.DownloadJson();
        Updater.Deserialisation();
        if(Updater.setName(args[0].ToLower())){
            try{
                
                if(Version.Parse(Updater.Latest.tag_name) > Version.Parse(args[1])){
                    Console.WriteLine("Запуск процесса...");
                    await Updater.Downloadzip(args[0].ToLower());
                    Updater.StartProc();
                }
                else{
                    Console.WriteLine("Нету новой версии");                    
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
        Console.WriteLine($"новая версия релиза = {Latest.tag_name}");
        assets = Latest.assets;
        

        foreach(var asset in Latest.assets){
            Console.WriteLine($"name = {asset.name}");
        }
    }
    public async Task DownloadJson(){
        try{
            Console.WriteLine(InfoModel.GetUrlApi());
            var response = await client.GetStringAsync(InfoModel.GetUrlApi());
            releases = JsonSerializer.Deserialize<List<Releases>>(response);
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }
                
        

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
    
    public async Task Downloadzip(string Os){
        try{
            Console.WriteLine("Запуск процесса...");
            string urldownload = assets.Where(c=>c.name.Contains(Os)).FirstOrDefault().browser_download_url;
            data = await client.GetByteArrayAsync($"{urldownload}");
            var path = Path.Combine(PathToDirectory, "Core", name_file);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            FileInfo fi = new FileInfo(path);
            using(FileStream fs = fi.Create()){
                fs.Write(data, 0, data.Length);
                fs.Close();
                Console.WriteLine("Запуск процесса...");
            }
            
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
                    Console.WriteLine($"Процесс {i}, имя {proc.ProcessName} убиваю");
                    try{
                        proc.Kill(true);
                        await proc.WaitForExitAsync();
                        proc.Dispose();
                    }
                    catch(Exception e){
                        Console.WriteLine(e.Message);
                    }    
                }
            }
            else{
                Console.WriteLine("Процесс не запущен");
            }

    }
    public async Task UnZip()
    {
        var temdir = Path.Combine(PathToDirectory,"temp_update");
        var backupdir = Path.Combine(PathToDirectory, "backup_dir");
        if(Directory.Exists(backupdir)) Directory.Delete(backupdir,true);
        if(Directory.Exists(temdir)) Directory.Delete(temdir,true);
        Directory.CreateDirectory(backupdir);
        Directory.CreateDirectory(temdir);
        await ZipFile.ExtractToDirectoryAsync($"{PathToDirectory}/Core/{name_file}",$"{temdir}", overwriteFiles: true);
        await Clear();
        Directory.Move(PathToDirectory,backupdir);
        Directory.Move(temdir,PathToDirectory);
        Console.WriteLine("Проверка обновления...");
        var exePath = Path.Combine(PathToDirectory, "Vpn-Client.Desktop");
        Console.WriteLine(File.GetLastWriteTime(exePath));
    }
    public void StartProc(){
        try{
            using Process proc = Process.Start($"{PathToDirectory}/Vpn-Client.Desktop");
                               
        }
        catch(Exception e){
            Console.WriteLine(e);
        }
    }
}
