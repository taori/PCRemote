using Microsoft.Xaml.Behaviors;

namespace Amusoft.PCR.App.WindowsAgent.Behaviors;

public class FocusOnLoadBehavior : Behavior<FrameworkElement>
{
	protected override void OnAttached()
	{
		this.AssociatedObject.Loaded += AssociatedObjectOnLoaded;
		base.OnAttached();
	}

	private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is FrameworkElement focusable)
			focusable.Focus();
		this.AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
	}
}