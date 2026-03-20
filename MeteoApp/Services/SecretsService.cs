namespace MeteoApp.Services;

public static class SecretsService
{
    private static Dictionary<string, string> _secrets;
    private static readonly string SECRETS_FILE_NAME = "secrets.env";

    public static string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        // Load secrets
        _secrets = Load();

        return _secrets.TryGetValue(key, out var value) ? value : null;
    }

    private static Dictionary<string, string> Load()
    {
        var assembly = typeof(SecretsService).Assembly;

        string resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(r => r.EndsWith(SECRETS_FILE_NAME))
            ?? throw new FileNotFoundException($"{SECRETS_FILE_NAME} not found as Embedded Resource");

        var stream = assembly.GetManifestResourceStream(resourceName)!;
        var reader = new StreamReader(stream);
        var result = new Dictionary<string, string>();

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var parts = line.Split("=", 2);

            if (parts.Length == 2)
                result[parts[0].Trim()] = parts[1].Trim();
        }

        return result;
    }
}