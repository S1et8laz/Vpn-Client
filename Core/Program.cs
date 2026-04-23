
using System.IO.Compression;
using System.Diagnostics;
namespace test;
public class UpdateVpnCLient{
    public string name_file = String.Empty;
    public static HttpClient client = new HttpClient();
    public byte[] data {get; set;}
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
        Console.WriteLine($"url = {args[0]}, директория {args[1]}, ос = {args[2]}");
        client.DefaultRequestHeaders.Add("User-Agent", "Vpn_Client-Update");
        var Updater = new UpdateVpnCLient();
        Updater.PathToDirectory = args[1];
        Updater.setName(args[2].ToLower());
        try{
            Console.WriteLine("Запуск процесса...");
            await Updater.Downloadzip(args[0]);
            Updater.StartProc();  
        }
        catch(Exception ex){
            Console.WriteLine(ex.Message);
        }
             
    }
    
        
    public void setName(string Osname){
        switch(Osname){
            case "linux": name_file = "vpn-client-linux.zip";break;
            case "windows": name_file = "vpn-client-windows.zip";break;
        }
    }
    public async Task Downloadzip(string url){
        try{
            Console.WriteLine("Запуск процесса...");
            data = await client.GetByteArrayAsync($"{url}");
            var path = Path.Combine(PathToDirectory, "..", name_file);
            Console.WriteLine($"Скачали в это место {path}");
            if(!Directory.Exists(path)) Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            
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
        while(Process.GetProcessesByName("Vpn-Client.Desktop").Length > 0)
        {
            Console.WriteLine("Жду закрытия приложения...");
            await Task.Delay(500);
        }
        
    }
    public async Task UnZip()
    {
        try{
            var temdir = Path.GetFullPath(Path.Combine( PathToDirectory, ".." ,"temp_update"));
            var backupdir = Path.GetFullPath(Path.Combine(PathToDirectory, ".." ,"backup_dir"));
            var zipfile = Path.GetFullPath(Path.Combine(PathToDirectory,"..",name_file ));
            Console.WriteLine($"путь к файлу зип {zipfile}");
            if(Directory.Exists(backupdir)) Directory.Delete(backupdir,true);
            if(Directory.Exists(temdir)) Directory.Delete(temdir,true);
            Directory.CreateDirectory(temdir);
            await ZipFile.ExtractToDirectoryAsync($"{zipfile}",$"{temdir}", overwriteFiles: true);
            await Clear();
            Directory.Move(PathToDirectory,backupdir);
            Directory.Move(temdir,PathToDirectory);
            Console.WriteLine("Проверка обновления...");
            var exePath = Path.GetFullPath(Path.Combine(PathToDirectory, "Vpn-Client.Desktop"));
            if(!File.Exists(exePath)){
                Console.WriteLine("Файл не найден после обновления!");
                return; 
            }
            else Console.WriteLine(File.GetLastWriteTime(exePath));
            Directory.Delete(temdir,true);
            Directory.Delete(backupdir, true);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    public void StartProc(){
        try {
            var exePath = Path.Combine(PathToDirectory, "Vpn-Client.Desktop");

            // 1. Обязательно даем права на запуск (только для Linux)
            if (!OperatingSystem.IsWindows()) {
                Process.Start("chmod", $"+x {exePath}").WaitForExit();
            }

            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = exePath,
                WorkingDirectory = PathToDirectory, // 2. Указываем рабочую папку!
                UseShellExecute = true
        };
        Console.WriteLine($"Запуск: {exePath}");
        Process.Start(startInfo);
    }
    catch (Exception e) {
        Console.WriteLine($"Ошибка при запуске: {e.Message}");
    }
    }
}
