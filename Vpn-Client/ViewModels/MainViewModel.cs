using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System;
namespace Vpn_Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty]
    private string _greeting;

    public MainViewModel(){
        Greeting = "hello";
    }
    public void getVersion(){

        var version = Assembly.GetEntryAssembly()
            .GetName().Version?.ToString(3); 
            Greeting = version == null? "null":version ;
            
    }
    public void Update(string Os){
        using Process procc = Process.Start($"Core/test", $"{Os} {Greeting}");
        if(procc == null) Console.WriteLine("error procc");
    }
}
