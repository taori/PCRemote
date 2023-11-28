using NLog.Targets;

// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.App.UI.Platforms;

public partial class MauiLog : TargetWithLayoutHeaderAndFooter
{
	/// <summary>
	/// Source of a log message
	/// </summary>
	public NLog.Layouts.Layout Category { get; set; }
}