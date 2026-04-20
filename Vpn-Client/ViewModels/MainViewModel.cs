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
    public MainViewModel(){
        Greeting = "hello";

    }
    public void getVersion(){

        version = Assembly.GetEntryAssembly()
            .GetName().Version?.ToString(3); 
            
    }
    public void Update(string Os){
        using Process procc = Process.Start($"Core/test", $"{Os} {version}");
        if(procc == null) Console.WriteLine("error procc");
    }
}
