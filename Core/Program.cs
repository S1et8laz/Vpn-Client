
using System;
using System.IO.Compression;
using System.Diagnostics;
using System.Text.Json;

public class UpdateVpnCLient{
    public string url = "https://github.com/S1et8laz/Vpn-Client/releases/v18/download";
    public string name_file = String.Empty;
    public static HttpClient client = new HttpClient();
    public byte[] data = Array.Empty<byte>();

    public static async Task Main(string[] args){
        if(args.Length>2){
            Console.WriteLine("слишком много аргументов");
            return;
        }
        else if(args.Length == 0){
            Console.WriteLine("Введите аргумент для выбора системы");
            return;
        }
        Console.WriteLine($"Система = {args[0]}");
        var Updater = new UpdateVpnCLient();
        if(Updater.setName(args[0].ToLower())){
            try{
                Console.WriteLine("Запуск процесса...");
                await Updater.Downloadzip();
                Updater.StartProc();
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
             
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
    public async Task<bool> CheckVersion(){
        var response = await client.GetStringAsync(url);

        return true;
    }
    public async Task Downloadzip(){
        try{
            Console.WriteLine("Запуск процесса...");
            data = await client.GetByteArrayAsync($"{url}/{name_file}");
            FileInfo fi = new FileInfo($"./{name_file}");
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
    public async Task UnZip()=> await ZipFile.ExtractToDirectoryAsync($"./{name_file}","./unzip", overwriteFiles: true);

    public void StartProc(){
        try{
            using(Process proc = Process.Start("./unzip/Vpn-Client.Desktop")){ 
            }
            Thread.Sleep(2000);
            Process[] VpnClient = Process.GetProcessesByName("Vpn-Client.Desktop");
            if(VpnClient.Length > 0){
                Console.WriteLine($"Запущенно {VpnClient.Length} процессов");
                Thread.Sleep(2000);
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
        catch(Exception e){
            Console.WriteLine(e);
        }
    }
}
