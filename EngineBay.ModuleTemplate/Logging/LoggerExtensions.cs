namespace EngineBay.ModuleTemplate
{
    // High Performance logging extensions
    // https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> MyLogLineValue = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: 1, // NB these eventIds must be unique per namespace
            formatString: "Logging something...");

        public static void MyLogLine(this ILogger logger)
        {
            MyLogLineValue(logger, null);
        }
    }
}