using CommunityToolkit.Mvvm.ComponentModel;

namespace Vpn_Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty]
    private string _greeting;
    public MainViewModel(){
        Greeting = "Скоро будет кнопка .";
    }
    
    
}
