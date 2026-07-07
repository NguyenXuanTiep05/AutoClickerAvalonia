using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System;
using System.Threading.Tasks;

namespace AutoClickerAvalonia.src;

public class WindowsMouse : IMouse
{
	private IntPtr _windowHandle = IntPtr.Zero;

	public string SearchWindow(string title)
	{
		string foundWindowTitle = "";
		IntPtr? found = null;

		EnumWindows((hWnd, _) =>
		{
			if (!IsWindowVisible(hWnd))
				return true;

			StringBuilder sb = new(256);

			GetWindowText(
				hWnd,
				sb,
				sb.Capacity);

			string currentTitle = sb.ToString();

			if (currentTitle.Contains(
				title,
				StringComparison.OrdinalIgnoreCase))
			{
				found = hWnd;
				foundWindowTitle = currentTitle;

				return false;
			}

			return true;

		}, IntPtr.Zero);

		if (found != null)
		{
			_windowHandle = found.Value;

			Debug.WriteLine(
				$"Window found: {foundWindowTitle}");

			return foundWindowTitle;
		}

		return "";
	}

	public Task ClickAsync()
	{
		IntPtr center = GetCenterLParam();
		if (_windowHandle == IntPtr.Zero)
			throw new Exception(
				"No window selected.");

		PostMessage(
			_windowHandle,
			WM_LBUTTONDOWN,
			IntPtr.Zero,
			center);

		PostMessage(
			_windowHandle,
			WM_LBUTTONUP,
			IntPtr.Zero,
			center);

		return Task.CompletedTask;
	}

	public Task HoldAsync()
	{
		IntPtr center = GetCenterLParam();
		if (_windowHandle == IntPtr.Zero)
			throw new Exception(
				"No window selected.");

		PostMessage(
			_windowHandle,
			WM_LBUTTONDOWN,
			IntPtr.Zero,
			center);

		return Task.CompletedTask;
	}

	public Task ReleaseAsync()
	{
		IntPtr center = GetCenterLParam();
		if (_windowHandle == IntPtr.Zero)
			throw new Exception(
				"No window selected.");

		PostMessage(
			_windowHandle,
			WM_LBUTTONUP,
			IntPtr.Zero,
			center);

		return Task.CompletedTask;
	}
	private IntPtr GetCenterLParam()
	{
		GetClientRect(_windowHandle, out RECT rect);

		int width = rect.Right - rect.Left;
		int height = rect.Bottom - rect.Top;

		int x = width / 2;
		int y = height / 2;

		return (IntPtr)((y << 16) | (x & 0xFFFF));
	}

	#region Win32

	private delegate bool EnumWindowsProc(
		IntPtr hWnd,
		IntPtr lParam);

	private const uint WM_LBUTTONDOWN = 0x0201;
	private const uint WM_LBUTTONUP = 0x0202;

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(
		EnumWindowsProc lpEnumFunc,
		IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(
		IntPtr hWnd);

	[DllImport("user32.dll",
		CharSet = CharSet.Unicode)]
	private static extern int GetWindowText(
		IntPtr hWnd,
		StringBuilder lpString,
		int nMaxCount);

	[DllImport("user32.dll")]
	private static extern bool PostMessage(
		IntPtr hWnd,
		uint msg,
		IntPtr wParam,
		IntPtr lParam);

	[StructLayout(LayoutKind.Sequential)]
	private struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[DllImport("user32.dll")]
	private static extern bool GetClientRect(
		IntPtr hWnd,
		out RECT lpRect);
	#endregion
}
