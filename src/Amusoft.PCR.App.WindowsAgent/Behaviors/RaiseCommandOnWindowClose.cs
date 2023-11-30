using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Amusoft.PCR.App.WindowsAgent.Behaviors;

public class RaiseCommandOnWindowClose : Behavior<Window>
{
	public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
		nameof(Command), typeof(ICommand), typeof(RaiseCommandOnWindowClose), new PropertyMetadata(default(ICommand)));

	public ICommand Command
	{
		get { return (ICommand)GetValue(CommandProperty); }
		set { SetValue(CommandProperty, value); }
	}
	
	protected override void OnAttached()
	{
		AssociatedObject.Closing += Closing;
		base.OnAttached();
	}

	private void Closing(object? sender, CancelEventArgs e)
	{
		Command?.Execute(null);
	}

	protected override void OnDetaching()
	{
		AssociatedObject.Closing -= Closing;
		base.OnDetaching();
	}
}