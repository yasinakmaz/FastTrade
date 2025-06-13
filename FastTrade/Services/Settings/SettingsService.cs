namespace FastTrade.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<SettingsService>? _logger;

        public SettingsService(ILogger<SettingsService>? logger = null)
        {
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await SecureStorage.Default.GetAsync(key);
                if (string.IsNullOrEmpty(value))
                    return default(T);

                if (typeof(T) == typeof(string))
                    return (T)(object)value;

                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting value for key: {key}");
                return default(T);
            }
        }

        public async Task<string?> GetStringAsync(string key)
        {
            try
            {
                return await SecureStorage.Default.GetAsync(key);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting string value for key: {key}");
                return null;
            }
        }

        public async Task<int?> GetIntAsync(string key)
        {
            try
            {
                var value = await SecureStorage.Default.GetAsync(key);
                if (string.IsNullOrEmpty(value))
                    return null;

                return int.TryParse(value, out var result) ? result : null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting int value for key: {key}");
                return null;
            }
        }

        public async Task<bool?> GetBoolAsync(string key)
        {
            try
            {
                var value = await SecureStorage.Default.GetAsync(key);
                if (string.IsNullOrEmpty(value))
                    return null;

                return bool.TryParse(value, out var result) ? result : null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting bool value for key: {key}");
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value)
        {
            try
            {
                if (value == null)
                {
                    await RemoveAsync(key);
                    return;
                }

                var serializedValue = typeof(T) == typeof(string)
                    ? value.ToString()
                    : JsonSerializer.Serialize(value);

                await SecureStorage.Default.SetAsync(key, serializedValue);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error setting value for key: {key}");
                throw;
            }
        }

        public async Task SetStringAsync(string key, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    await RemoveAsync(key);
                    return;
                }

                await SecureStorage.Default.SetAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error setting string value for key: {key}");
                throw;
            }
        }

        public async Task SetIntAsync(string key, int value)
        {
            try
            {
                await SecureStorage.Default.SetAsync(key, value.ToString());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error setting int value for key: {key}");
                throw;
            }
        }

        public async Task SetBoolAsync(string key, bool value)
        {
            try
            {
                await SecureStorage.Default.SetAsync(key, value.ToString());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error setting bool value for key: {key}");
                throw;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                SecureStorage.Default.Remove(key);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error removing key: {key}");
                throw;
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                SecureStorage.Default.RemoveAll();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error clearing all settings");
                throw;
            }
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            try
            {
                var value = await SecureStorage.Default.GetAsync(key);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error checking if key exists: {key}");
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetAllAsync()
        {
            var result = new Dictionary<string, string>();

            try
            {
                var keys = new[]
                {
                    PublicServices.MSSQLServer,
                    PublicServices.MSSQLUSERNAME,
                    PublicServices.MSSQLPASSWORD,
                    PublicServices.MSSQLDATABASE,
                    PublicServices.HOSTIP,
                    PublicServices.HOSTPORT,
                    PublicServices.HOSTDEFAULTPRINTER,
                    PublicServices.HOSTDEFAULTDESIGN,
                    PublicServices.FULLSCREEN
                };

                foreach (var key in keys)
                {
                    var value = await SecureStorage.Default.GetAsync(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        result[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting all settings");
            }

            return result;
        }
    }
}
