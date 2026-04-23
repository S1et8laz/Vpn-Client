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
            MainViewModel mainviewmodel = new MainViewModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainviewmodel  
            };
            UpdateViewModel updateviewmodel = new UpdateViewModel();
            UpdateWindow updatewindow = new UpdateWindow
            {
                DataContext =  updateviewmodel
            };

            
            if(OperatingSystem.IsLinux())
            {
                Update("linux",desktop.MainWindow, updatewindow, updateviewmodel);
            }
            else if( OperatingSystem.IsWindows()){
                Update("linux",desktop.MainWindow, updatewindow, updateviewmodel);
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

    private async void Update(string Os, Window main, Window updatewindow,ViewModelBase Updateviewmodel)
    {
        var updateviewmodel = (UpdateViewModel) Updateviewmodel;
        UpdateService us = new UpdateService();
        try{
            List<Release> releases = await us.GetReleases(InfoModel.GetUrlApi());
            var lastrelease = us.GetLatestRelease(releases);
            var version = Assembly.GetEntryAssembly()
           .GetName().Version?.ToString(3);
            var currentversion = Assembly.GetEntryAssembly().GetName().Version?.ToString(3);
            Console.WriteLine($"current version = {currentversion}");
            if(us.IsNewVersion(currentversion, lastrelease.tag_name)){
                updatewindow.Show();
                await updateviewmodel.Update(us.GetDownloadUrl(lastrelease, Os),Os);
            }
            else{
                updatewindow.Close();
                main.Show();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
