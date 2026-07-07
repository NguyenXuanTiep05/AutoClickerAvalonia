using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System;
using System.Threading.Tasks;

namespace AutoClickerAvalonia.src;

public class MouseLinux : IMouse
{
	private string _selectedWindow = "";
	public string SearchWindow(string title)
	{
		var windowId = Run("xdotool", $"search --name \".*{title}.*\"")
			.Split('\n')[0].Trim();

		if (string.IsNullOrEmpty(windowId))
			return "";

		_selectedWindow = windowId;

		return Run("xdotool", $"getwindowname {_selectedWindow}");
	}
	public Task ClickAsync(string clickButton)
	{
		if (string.IsNullOrEmpty(_selectedWindow))
			return Task.CompletedTask;

		int button = clickButton == "Left" ? 1 : 3;
		Run("xdotool", $"mousedown --window {_selectedWindow} {button}");
		Run("xdotool", $"mouseup --window {_selectedWindow} {button}");

		return Task.CompletedTask;
	}

	public Task HoldAsync(string clickButton)
	{
		if (string.IsNullOrEmpty(_selectedWindow))
			return Task.CompletedTask;

		int button = clickButton == "Left" ? 1 : 3;
		Run("xdotool", $"mousedown --window {_selectedWindow} {button}");

		return Task.CompletedTask;
	}

	public Task ReleaseAsync(string clickButton)
	{
		if (string.IsNullOrEmpty(_selectedWindow))
			return Task.CompletedTask;

		int button = clickButton == "Left" ? 1 : 3;
		Run("xdotool", $"mouseup --window {_selectedWindow} {button}");

		return Task.CompletedTask;
	}
	private string Run(string cmd, string args = "")
	{
		var p = Process.Start(new ProcessStartInfo(cmd, args)
		{
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false
		})!;
		var output = p.StandardOutput.ReadToEnd().Trim();
		p.WaitForExit();
		return output;
	}
}
