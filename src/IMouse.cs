using System.Threading.Tasks;

namespace AutoClickerAvalonia.src;

public interface IMouse
{
	string SearchWindow(string Title);
	Task ClickAsync();
	Task HoldAsync();
	Task ReleaseAsync();

}