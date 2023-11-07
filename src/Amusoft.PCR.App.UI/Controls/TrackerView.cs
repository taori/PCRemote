using System.Windows.Input;

namespace Amusoft.PCR.App.UI.Controls;

public class TrackerView : View
{
	public static readonly BindableProperty TapEnabledProperty =
		BindableProperty.Create("TapEnabled", typeof(bool), typeof(TrackerView), true);

	public bool TapEnabled
	{
		get => (bool) GetValue(TapEnabledProperty);
		set => SetValue(TapEnabledProperty, value);
	}

	public static readonly BindableProperty TapCommandProperty =
		BindableProperty.Create("TapCommand", typeof(ICommand), typeof(TrackerView), default);

	public ICommand? TapCommand
	{
		get => (ICommand?) GetValue(TapCommandProperty);
		set => SetValue(TapCommandProperty, value);
	}

	public static readonly BindableProperty TapCommandParameterProperty =
		BindableProperty.Create("TapCommandParameter", typeof(object), typeof(TrackerView), default);

	public object? TapCommandParameter
	{
		get => (object?) GetValue(TapCommandParameterProperty);
		set => SetValue(TapCommandParameterProperty, value);
	}

	public static readonly BindableProperty SensitivityProperty =
		BindableProperty.Create("Sensitivity", typeof(int), typeof(TrackerView), 1000);

	public int Sensitivity
	{
		get => (int) GetValue(SensitivityProperty);
		set => SetValue(SensitivityProperty, value);
	}

	public static readonly BindableProperty MultiTapCommandProperty =
		BindableProperty.Create("MultiTapCommand", typeof(ICommand), typeof(TrackerView), default);

	public ICommand? MultiTapCommand
	{
		get => (ICommand?) GetValue(MultiTapCommandProperty);
		set => SetValue(MultiTapCommandProperty, value);
	}

	public static readonly BindableProperty VelocityOccuredCommandProperty =
		BindableProperty.Create("VelocityOccuredCommand", typeof(ICommand), typeof(TrackerView), default);

	public ICommand? VelocityOccuredCommand
	{
		get => (ICommand?) GetValue(VelocityOccuredCommandProperty);
		set => SetValue(VelocityOccuredCommandProperty, value);
	}
}