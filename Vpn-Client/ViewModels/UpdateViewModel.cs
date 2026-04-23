using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System;
using Shared;
using Shared.Models;
using System.Threading.Tasks;
using System.Threading;
namespace Vpn_Client.ViewModels;

public partial class UpdateViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _text;
    private string PathToDirectory {get;set;}
    public UpdateViewModel(){
        PathToDirectory = AppContext.BaseDirectory;
        Text = "обновление";
    }
    
    public async Task Update(string url, String Os){
        using Process procc = Process.Start($"{PathToDirectory}/Core/test", $"{url} {PathToDirectory} {Os}");
        if(procc == null) Console.WriteLine("error procc");
        Thread.Sleep(5000);
        Environment.Exit(0);
    }
}
