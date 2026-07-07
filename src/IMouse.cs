using System.Threading.Tasks;

namespace AutoClickerAvalonia.src;

public interface IMouse
{
	string SearchWindow(string Title);
	Task ClickAsync(string clickButton);
	Task HoldAsync(string clickButton);
	Task ReleaseAsync(string clickButton);

}