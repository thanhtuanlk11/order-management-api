using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Linq;

namespace WebApplication1.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _path;
        private readonly bool _append;
        private readonly long? _fileSizeLimitBytes;
        private readonly int? _maxRollingFiles;

        public FileLoggerProvider(string path, bool append = false, long? fileSizeLimitBytes = null, int? maxRollingFiles = null)
        {
            _path = path;
            _append = append;
            _fileSizeLimitBytes = fileSizeLimitBytes;
            _maxRollingFiles = maxRollingFiles;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _path, _append, _fileSizeLimitBytes, _maxRollingFiles);
        }

        public void Dispose()
        {
        }
    }

    public class FileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _path;
        private readonly bool _append;
        private readonly long? _fileSizeLimitBytes;
        private readonly int? _maxRollingFiles;
        private readonly object _lock = new object();

        public FileLogger(string categoryName, string path, bool append, long? fileSizeLimitBytes, int? maxRollingFiles)
        {
            _categoryName = categoryName;
            _path = path;
            _append = append;
            _fileSizeLimitBytes = fileSizeLimitBytes;
            _maxRollingFiles = maxRollingFiles;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
                return;

            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName}: {message}";
            if (exception != null)
                logEntry += $"{Environment.NewLine}{exception}";

            lock (_lock)
            {
            
                RollFiles();

                File.AppendAllText(_path, logEntry + Environment.NewLine);
            }
        }

        private void RollFiles()
        {
            if (!_fileSizeLimitBytes.HasValue || !_maxRollingFiles.HasValue)
                return;

            try
            {
                var fileInfo = new FileInfo(_path);
                if (!fileInfo.Exists)
                    return;

                if (fileInfo.Length < _fileSizeLimitBytes.Value)
                    return;

                var directory = Path.GetDirectoryName(_path);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_path);
                var extension = Path.GetExtension(_path);

                var files = Directory.GetFiles(directory, $"{fileNameWithoutExtension}-*{extension}")
                    .OrderBy(f => new FileInfo(f).CreationTime)
                    .ToList();

                while (files.Count >= _maxRollingFiles.Value)
                {
                    File.Delete(files[0]);
                    files.RemoveAt(0);
                }

                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}-{timestamp}{extension}");
                File.Move(_path, newFilePath);

               
                File.Create(_path).Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rolling log files: {ex.Message}");
            }
        }
    }
}
