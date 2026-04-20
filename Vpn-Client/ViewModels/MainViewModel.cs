using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System;
namespace Vpn_Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty]
    private string _greeting;
    private string version {get;set;}
    private string PathToDirectory {get;set;}
    public MainViewModel(){
        Greeting = "salo";
        PathToDirectory = Environment.CurrentDirectory;

    }
    public void getVersion(){

        version = Assembly.GetEntryAssembly()
            .GetName().Version?.ToString(3); 
            
    }
    public void Update(string Os){
        using Process procc = Process.Start($"Core/test", $"{Os} {version} {PathToDirectory}");
        if(procc == null) Console.WriteLine("error procc");
    }
}
