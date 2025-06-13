namespace FastTrade.Services.HostServices.Interfaces
{
    public interface ISettingsService
    {
        Task<T?> GetAsync<T>(string key);
        Task<string?> GetStringAsync(string key);
        Task<int?> GetIntAsync(string key);
        Task<bool?> GetBoolAsync(string key);
        Task SetAsync<T>(string key, T value);
        Task SetStringAsync(string key, string value);
        Task SetIntAsync(string key, int value);
        Task SetBoolAsync(string key, bool value);
        Task RemoveAsync(string key);
        Task ClearAsync();
        Task<bool> ContainsKeyAsync(string key);
        Task<Dictionary<string, string>> GetAllAsync();
    }
}
