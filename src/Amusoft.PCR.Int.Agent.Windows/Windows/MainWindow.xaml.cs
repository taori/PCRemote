﻿using System.Windows;

namespace Amusoft.PCR.Int.Agent.Windows.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		Application.Current.MainWindow = this;
	}
}