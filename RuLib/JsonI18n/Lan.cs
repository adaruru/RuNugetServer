// RuLib 1.0.0
// Copyright (C) 2024, Adaruru

using Newtonsoft.Json;

namespace RuLib.JsonI18n;

public class Lan
{
    /// <summary>
    /// 資源檔必須設為 Content 內容，且建置或發佈設定 CopyToOutputDirectory Always/PreserveNewest 永遠複製/有更新時才複製
    /// </summary>
    public static LanguageObj currentLan = new LanguageObj();
    private static string _currentLanguage = "zh_TW";
    public static void LoadLanguage(string? languageCode = null, string? assetPath = null)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = _currentLanguage;
        }

        string filePath = string.IsNullOrEmpty(assetPath) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", $"{languageCode}.json") : Path.Combine(assetPath, $"{languageCode}.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var languageData = JsonConvert.DeserializeObject<LanguageObj>(json);
            currentLan = languageData;
        }
        else
        {
            throw new FileNotFoundException($"Language file not found: {filePath}");
        }
        _currentLanguage = languageCode;
    }
}

public partial class LanguageObj
{
    public string ConnectionString { get; set; }
}