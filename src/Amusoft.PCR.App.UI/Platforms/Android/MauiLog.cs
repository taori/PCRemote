using NLog;
using NLog.Internal.Xamarin;
using NLog.Targets;
using Log = Android.Util.Log;
using Layout = NLog.Layouts.Layout;

namespace Amusoft.PCR.App.UI.Platforms;

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

        Java.Lang.Throwable throwable = null;
        if (logEvent.Exception != null)
        {
            throwable = Java.Lang.Throwable.FromException(logEvent.Exception);
        }

        if (logEvent.Level == LogLevel.Trace)
        {
            Log.Verbose(category, throwable, message);
        }
        else if (logEvent.Level == LogLevel.Debug)
        {
            Log.Debug(category, throwable, message);
        }
        else if (logEvent.Level == LogLevel.Info)
        {
            Log.Info(category, throwable, message);
        }
        else if (logEvent.Level == LogLevel.Warn)
        {
            Log.Warn(category, throwable, message);
        }
        else if (logEvent.Level == LogLevel.Error)
        {
            Log.Error(category, throwable, message);
        }
        else
        {
            Log.Wtf(category, throwable, message);
        }
    }
}