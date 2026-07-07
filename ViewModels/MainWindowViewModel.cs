using System.Diagnostics;
using AutoClickerAvalonia.src;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;


namespace AutoClickerAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    private readonly IMouse _mouse;

    public MainWindowViewModel()
    {
        _mouse = new WindowsMouse();
    }

    private string _windowTitle = "";
    private string _foundWindow = "";
    private int _delay = 5;
    private string _clickButton = "Left";
    private string _clickType = "Hold";


    public string WindowTitle
    {
        get => _windowTitle;
        set
        {
            _windowTitle = value;
            FoundWindow = _mouse.SearchWindow(_windowTitle);

            Debug.WriteLine($"Changed to {value}");

            OnPropertyChanged();
        }
    }

    public string FoundWindow
    {
        get => _foundWindow;
        set
        {
            _foundWindow = "Found Window: " + value;
            OnPropertyChanged();
        }
    }




    public int? Delay
    {
        get => _delay;
        set
        {
            _delay = value ?? 5;


            Debug.WriteLine($"Changed to {_delay}");

            OnPropertyChanged();
        }
    }



    public bool IsLeft
    {
        get => _clickButton == "Left";
        set
        {
            if (value)
            {
                _clickButton = "Left";
                Debug.WriteLine($"Selected: {_clickButton}");
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRight));
            }
        }
    }

    public bool IsRight
    {
        get => _clickButton == "Right";
        set
        {
            if (value)
            {
                _clickButton = "Right";
                Debug.WriteLine($"Selected: {_clickButton}");
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLeft));
            }
        }
    }


    public bool IsClick
    {
        get => _clickType == "Click";
        set
        {
            if (value)
            {
                _clickType = "Click";
                Debug.WriteLine($"Selected: {_clickType}");
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsHold));
            }
        }
    }

    public bool IsHold
    {
        get => _clickType == "Hold";
        set
        {
            if (value)
            {
                _clickType = "Hold";
                Debug.WriteLine($"Selected: {_clickType}");
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsClick));
            }
        }
    }


    [RelayCommand]
    private void OnOff()
    {
        Debug.Write("\tWhat the sigma \t");
    }

}
