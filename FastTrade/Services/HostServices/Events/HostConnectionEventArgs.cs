namespace FastTrade.Services.HostServices.Events
{
    public class HostConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string HostIP { get; set; } = string.Empty;
        public int HostPort { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
