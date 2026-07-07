using System.Runtime.InteropServices;

namespace AutoClickerAvalonia.src;

public static class GlobalHotkey
{
    private const int VK_F6 = 0x75;

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static bool IsStopKeyDown() => (GetAsyncKeyState(VK_F6) & 0x8000) != 0;
}
