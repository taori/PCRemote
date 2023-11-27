namespace Amusoft.PCR.App.WindowsAgent.Helpers;

/*
 * STA is required for clipboard to work properly. At the time of writing this, getting wpf net5+ to run in STA is not straight forward
 */
internal class ClipboardHelper
{
	public static Task<string> GetClipboardAsync(TextDataFormat format = TextDataFormat.Text)
	{
		var tcs = new TaskCompletionSource<string>();
		var thread = new Thread(() =>
		{
			tcs.SetResult(Clipboard.GetText(format));
		});
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();

		return tcs.Task;
	}

	public static Task SetClipboardAsync(string text, TextDataFormat format = TextDataFormat.Text)
	{
		var tcs = new TaskCompletionSource();
		var thread = new Thread(() =>
		{
			Clipboard.SetText(text, format);
			tcs.SetResult();
		});
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();

		return tcs.Task;
	}
}