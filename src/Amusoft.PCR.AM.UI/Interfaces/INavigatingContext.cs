namespace Amusoft.PCR.AM.UI.Interfaces;

public interface INavigatingContext
{
	IDeferalScope PauseNavigation();

	NavigationKind NavigationKind { get; }
}

public enum NavigationKind
{
	Unknown
	, Push
	, Pop
	, PopToRoot
	, Insert
	, Remove
	, ShellItemChanged
	, ShellSectionChanged
	, ShellContentChanged
}