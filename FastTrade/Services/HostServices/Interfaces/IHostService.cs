namespace FastTrade.Services.HostServices.Interfaces
{
    public interface IHostService
    {
        bool IsConnected { get; }
        string HostIP { get; }
        int HostPort { get; }

        Task<bool> ConnectAsync(string hostIP, int hostPort, int timeoutMs = 5000);
        Task DisconnectAsync();
        Task<HostResponse> SendRequestAsync(HostRequest request, int timeoutMs = 30000);
        Task<HostResponse> PushHostAsync(string data, DataFormat format, string? printerName = null,
            string? designPath = null, bool design = false, bool print = true);
        event EventHandler<HostConnectionEventArgs> ConnectionChanged;
        event EventHandler<HostErrorEventArgs> ErrorOccurred;
    }
}
