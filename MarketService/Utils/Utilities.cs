public static class Utilities
{
	public static Settings ReadSettings()
	{
		string settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
		string settingsJson = File.ReadAllText(settingsPath);
		if (!File.Exists(settingsPath))
			throw new FileNotFoundException($"File not found: {settingsPath}");

		return System.Text.Json.JsonSerializer.Deserialize<Settings>(settingsJson) ?? new Settings();
	}

	public static List<T> ReadJson<T>(string filePath) where T : class
	{
		var settings = ReadSettings();
		string fullPath = Path.Combine(settings!.BaseJsonFileFolder, filePath);
		if (!File.Exists(fullPath))
			throw new FileNotFoundException($"File not found: {filePath}");

		string json = File.ReadAllText(fullPath);
		return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
	}

	public static void WriteJson<T>(string filePath, List<T> data) where T : class
	{
		string json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(filePath, json);
	}
}