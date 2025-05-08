using System.Text.Json;

public static class Utilities
{
	public static Settings ReadSettings()
	{
		string settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
		if (!File.Exists(settingsPath))
			throw new FileNotFoundException($"File not found: {settingsPath}");

		using var stream = File.OpenRead(settingsPath);
		return JsonSerializer.Deserialize<Settings>(stream) ?? new Settings();
	}

	public static List<T> ReadJson<T>(string filePath) where T : class
	{
		var settings = ReadSettings();
		string fullPath = Path.Combine(settings!.BaseJsonFileFolder, filePath);

		if (!File.Exists(fullPath))
			throw new FileNotFoundException($"File not found: {filePath}");

		using var stream = File.OpenRead(fullPath);
		return JsonSerializer.Deserialize<List<T>>(stream) ?? new List<T>();
	}

	public static void WriteJson<T>(string filePath, List<T> data) where T : class
	{
		var settings = ReadSettings();
		string fullPath = Path.Combine(settings!.BaseJsonFileFolder, filePath);

		if (!File.Exists(fullPath))
			throw new FileNotFoundException($"File not found: {filePath}");

		using var stream = File.Create(fullPath);
		JsonSerializer.Serialize(stream, data, new JsonSerializerOptions { WriteIndented = true });
	}
}