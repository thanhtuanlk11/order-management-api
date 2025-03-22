using Microsoft.Extensions.Logging;

namespace WebApplication1.Logging
{
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string path, bool append = false, long? fileSizeLimitBytes = null, int? maxRollingFiles = null)
        {
            builder.AddProvider(new FileLoggerProvider(path, append, fileSizeLimitBytes, maxRollingFiles));
            return builder;
        }
    }
}
