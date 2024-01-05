namespace Amusoft.PCR.App.UI.Controls;

public partial class LoadingView : ContentView
{
	public LoadingView()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty IsLoadingProperty =
		BindableProperty.Create("IsLoading", typeof(bool), typeof(LoadingView), default);

	public bool IsLoading
	{
		get => (bool)GetValue(IsLoadingProperty);
		set => SetValue(IsLoadingProperty, value);
	}
}