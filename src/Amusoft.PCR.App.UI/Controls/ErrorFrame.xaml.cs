namespace Amusoft.PCR.App.UI.Controls;

public partial class ErrorFrame : ContentView
{
	public ErrorFrame()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty TextProperty =
		BindableProperty.Create("Text", typeof(string), typeof(ErrorFrame), string.Empty);

	public string Text
	{
		get => (string) GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}
}