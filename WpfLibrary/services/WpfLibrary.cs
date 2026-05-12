namespace WpfLibrary.services.Storage
{
    public interface IDataStorage<T>
    {
        T Load(string filePath, T defaultValue);
        void Save(string filePath, T data);
    }

    public class JsonFileStorage<T> : IDataStorage<T>
    {
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public T Load(string filePath, T defaultValue)
        {
            if (!File.Exists(filePath)) return defaultValue;
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json, _options) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public void Save(string filePath, T data)
        {
            string json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(filePath, json);
        }
    }
}
