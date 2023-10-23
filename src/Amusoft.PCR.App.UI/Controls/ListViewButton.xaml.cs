using System.Windows.Input;

namespace Amusoft.PCR.App.UI.Controls;

public partial class ListViewButton : ContentView
{
	public ListViewButton()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty TextProperty =
		BindableProperty.Create("Text", typeof(string), typeof(ListViewButton), string.Empty);

	public string Text
	{
		get => (string) GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create("Command", typeof(ICommand), typeof(ListViewButton), default);

	public ICommand Command
	{
		get => (ICommand) GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}
}