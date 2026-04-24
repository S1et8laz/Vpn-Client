using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Vpn_Client.ViewModels;
using Vpn_Client.Views;
using System;
using Shared;
using Shared.Models;
using System.Reflection;
using System.Collections.Generic;
using Avalonia.Controls;
using System.Threading.Tasks;
namespace Vpn_Client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            UpdateViewModel updateviewmodel = new UpdateViewModel();
            UpdateWindow updatewindow = new UpdateWindow
            {
                DataContext =  updateviewmodel
            };
            MainViewModel mainviewmodel = new MainViewModel();
            MainWindow mainWindow = new MainWindow
            {
                DataContext = mainviewmodel
            };
            desktop.MainWindow = updatewindow;
            
            

            
            if(OperatingSystem.IsLinux())
            {
                
                Update("linux",desktop,updatewindow,updateviewmodel, mainWindow);
            }
            else if( OperatingSystem.IsWindows()){
                Update("windows",desktop,updatewindow,updateviewmodel, mainWindow);
            }

            

        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = new MainViewModel() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task Update(string Os, IClassicDesktopStyleApplicationLifetime desktop,  UpdateWindow updatewindow,  UpdateViewModel Updateviewmodel, MainWindow mainWindow)
    {
        UpdateService us = new UpdateService();
        try{
            updatewindow.Show();
            List<Release> releases = await us.GetReleases(InfoModel.GetUrlApi());
            var lastrelease = us.GetLatestRelease(releases);
            var version = Assembly.GetEntryAssembly()
           .GetName().Version?.ToString(3);
            var currentversion = Assembly.GetEntryAssembly().GetName().Version?.ToString(3);
            Console.WriteLine($"current version = {currentversion}");
            if(us.IsNewVersion(currentversion, lastrelease.tag_name)){
                await Updateviewmodel.Update(us.GetDownloadUrl(lastrelease, Os),Os);
            }
            else{
                Console.WriteLine("Обновлять нечего");
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
                updatewindow.Close();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine($"Ошибка обновления: {e.Message}");
            // В случае ошибки всё равно открываем главное окно
            desktop.MainWindow = mainWindow;
            mainWindow.Show();
            updatewindow.Close();
        }
    }
}
