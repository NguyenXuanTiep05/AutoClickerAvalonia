using System.Threading.Tasks;

namespace AutoClickerAvalonia.src;

public interface IMouse
{
	string SearchWindow(string Title);
	Task ClickAsync(string clickType);
	Task HoldAsync(string clickType);
	Task ReleaseAsync(string clickType);

}