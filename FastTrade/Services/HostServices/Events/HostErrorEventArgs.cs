namespace FastTrade.Services.HostServices.Events
{
    public class HostErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public HostErrorEventArgs(Exception exception, string message = "")
        {
            Exception = exception;
            Message = string.IsNullOrEmpty(message) ? exception.Message : message;
        }
    }
}
