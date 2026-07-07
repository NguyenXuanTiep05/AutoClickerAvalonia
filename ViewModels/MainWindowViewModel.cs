using System.Diagnostics;
using AutoClickerAvalonia.src;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using Avalonia.Threading;


namespace AutoClickerAvalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    private readonly IMouse _mouse;
    public MainWindowViewModel()
    {
        if (OperatingSystem.IsWindows())
        {
            _mouse = new MouseWindows();
        }
        else
        {
            _mouse = new MouseLinux();
        }
    }

    private bool _isRunning = false;
    public bool IsNotRunning => !_isRunning;
    public string ButtonText =>
        _isRunning
            ? "Stop AutoClicker"
            : "Start AutoClicker";
    private string _windowTitle = "";
    private string _foundWindow = "";
    private int _delay = 5;
    private string _clickButton = "Left";
    private string _clickType = "Hold";

    private CancellationTokenSource? _cts;

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
            _foundWindow = value;
            OnPropertyChanged();
        }
    }




    public int? Delay
    {
        get => _delay;
        set
        {
            _delay = value ?? 5;
            _delay = _delay < 1 ? 10 : _delay;


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
    private void Run()
    {
        _isRunning = !_isRunning;
        OnPropertyChanged(nameof(ButtonText));
        OnPropertyChanged(nameof(IsNotRunning)); // add this
        if (_isRunning)
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            _ = RunClicker(_cts.Token);
        }
        else
        {
            _cts?.Cancel();
        }
    }

    private async Task RunClicker(CancellationToken token)
    {
        try
        {
            // Run off the UI thread so the click loop never blocks the dispatcher.
            await Task.Run(() => Clicker(token), token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.WriteLine($"AutoClicker stopped: {ex.Message}");
        }
        finally
        {
            // The loop can end on its own (hotkey, cancellation, error) without
            // the user having pressed the button, so reconcile state here too.
            if (_isRunning)
            {
                _isRunning = false;
                _cts?.Cancel();
                Dispatcher.UIThread.Post(() => OnPropertyChanged(nameof(ButtonText)));
                OnPropertyChanged(nameof(IsNotRunning));
            }
        }
    }

    private async Task Clicker(CancellationToken token)
    {
        if (_clickType == "Hold")
        {
            try
            {
                while (!token.IsCancellationRequested)
                {

                    await _mouse.HoldAsync(_clickButton);
                    await Task.Delay(50, token);
                }
            }
            finally
            {
                await _mouse.ReleaseAsync(_clickButton);
            }
            return;
        }

        while (!token.IsCancellationRequested)
        {
            await _mouse.ClickAsync(_clickButton);
            await Task.Delay(_delay, token);
        }
    }






}
