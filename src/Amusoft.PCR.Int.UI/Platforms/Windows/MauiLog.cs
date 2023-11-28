using System.Diagnostics;
using NLog;
using NLog.Internal.Xamarin;
using NLog.Targets;
using Layout = NLog.Layouts.Layout;

namespace Amusoft.PCR.Int.UI;

[Target("MauiLog")]
[Preserve]
public partial class MauiLog
{
    public MauiLog()
    {
        Layout = "${message}";
        Category = "${logger}";
    }

    /// <inheritdoc/>
    protected override void InitializeTarget()
    {
        base.InitializeTarget();

        if (Header != null)
        {
            var logEvent = LogEventInfo.CreateNullEvent();
            logEvent.Level = LogLevel.Info;
            logEvent.LoggerName = "Starting";
            DebugWriteLine(Header, logEvent);
        }
    }

    /// <inheritdoc/>
    protected override void CloseTarget()
    {
        if (Footer != null)
        {
            var logEvent = LogEventInfo.CreateNullEvent();
            logEvent.Level = LogLevel.Info;
            logEvent.LoggerName = "Closing";
            DebugWriteLine(Footer, logEvent);
        }

        base.CloseTarget();
    }

    /// <inheritdoc/>
    protected override void Write(LogEventInfo logEvent)
    {
        DebugWriteLine(Layout, logEvent);
    }

    private void DebugWriteLine(Layout layout, LogEventInfo logEvent)
    {
        var message = RenderLogEvent(layout, logEvent) ?? string.Empty;
        var category = RenderLogEvent(Category, logEvent);
        if (string.IsNullOrEmpty(category))
            category = null;

        if (Debugger.IsLogging())
        {
            Debugger.Log(logEvent.Level.Ordinal, category, message + Environment.NewLine);
        }
        else
        {
            Console.WriteLine(message);
        }
    }
}