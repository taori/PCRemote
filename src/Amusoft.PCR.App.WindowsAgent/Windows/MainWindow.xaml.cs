namespace Amusoft.PCR.App.WindowsAgent.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		System.Windows.Application.Current.MainWindow = this;
	}
}