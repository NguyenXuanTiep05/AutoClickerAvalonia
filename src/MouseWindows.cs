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
		if (_windowHandle == IntPtr.Zero)
			throw new Exception(
				"No window selected.");

		// Hold fire while our own window is in the foreground: the target
		// activates itself on every synthetic click, which would steal focus
		// mid-press and eat the user's click on the Stop button.
		if (IsOwnAppForeground())
			return Task.CompletedTask;

		(IntPtr target, IntPtr lParam) = GetClickTarget();

		PostMessage(
			target,
			WM_LBUTTONDOWN,
			MK_LBUTTON,
			lParam);

		PostMessage(
			target,
			WM_LBUTTONUP,
			IntPtr.Zero,
			lParam);

		return Task.CompletedTask;
	}

	public Task HoldAsync()
	{
		if (_windowHandle == IntPtr.Zero)
			throw new Exception(
				"No window selected.");

		if (IsOwnAppForeground())
			return Task.CompletedTask;

		(IntPtr target, IntPtr lParam) = GetClickTarget();

		PostMessage(
			target,
			WM_LBUTTONDOWN,
			MK_LBUTTON,
			lParam);

		return Task.CompletedTask;
	}

	private static bool IsOwnAppForeground()
	{
		GetWindowThreadProcessId(
			GetForegroundWindow(),
			out uint pid);

		return pid == (uint)Environment.ProcessId;
	}

	public Task ReleaseAsync()
	{
		if (_windowHandle == IntPtr.Zero)
			throw new Exception(
				"No window selected.");

		(IntPtr target, IntPtr lParam) = GetClickTarget();

		PostMessage(
			target,
			WM_LBUTTONUP,
			IntPtr.Zero,
			lParam);

		return Task.CompletedTask;
	}
	// Descend to the deepest child control under the window's center and
	// aim the click straight at it. Posting to a child control instead of
	// the top-level frame makes many apps skip their self-activation on
	// click, which is what caused the window to flash/steal focus.
	private (IntPtr Target, IntPtr LParam) GetClickTarget()
	{
		GetClientRect(_windowHandle, out RECT rect);

		POINT pt = new()
		{
			X = (rect.Right - rect.Left) / 2,
			Y = (rect.Bottom - rect.Top) / 2
		};

		IntPtr target = _windowHandle;

		while (true)
		{
			IntPtr child = ChildWindowFromPointEx(
				target,
				pt,
				CWP_SKIPINVISIBLE | CWP_SKIPTRANSPARENT);

			if (child == IntPtr.Zero || child == target)
				break;

			MapWindowPoints(
				target,
				child,
				ref pt,
				1);

			target = child;
		}

		return (target, (IntPtr)((pt.Y << 16) | (pt.X & 0xFFFF)));
	}

	#region Win32

	private delegate bool EnumWindowsProc(
		IntPtr hWnd,
		IntPtr lParam);

	private const uint WM_LBUTTONDOWN = 0x0201;
	private const uint WM_LBUTTONUP = 0x0202;

	private static readonly IntPtr MK_LBUTTON = (IntPtr)0x0001;

	private const uint CWP_SKIPINVISIBLE = 0x0001;
	private const uint CWP_SKIPTRANSPARENT = 0x0004;

	[StructLayout(LayoutKind.Sequential)]
	private struct POINT
	{
		public int X;
		public int Y;
	}

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

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(
		IntPtr hWnd,
		out uint lpdwProcessId);

	[DllImport("user32.dll")]
	private static extern IntPtr ChildWindowFromPointEx(
		IntPtr hWndParent,
		POINT pt,
		uint flags);

	[DllImport("user32.dll")]
	private static extern int MapWindowPoints(
		IntPtr hWndFrom,
		IntPtr hWndTo,
		ref POINT lpPoints,
		uint cPoints);
	#endregion
}
