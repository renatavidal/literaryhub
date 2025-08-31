using Google.Cloud.Translation.V2;
using System;
using System.Runtime.Caching;

public class TranslationService
{
    private readonly TranslationClient _client;
    private readonly ObjectCache _cache = MemoryCache.Default;

    public TranslationService(string apiKey)
    {
        _client = TranslationClient.CreateFromApiKey(apiKey);
    }

    public string Translate(string text, string target, string source = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        string key = $"tr::{source ?? "auto"}::{target}::{text.GetHashCode()}";
        if (_cache.Contains(key)) return (string)_cache[key];

        var resp = _client.TranslateText(text, target, sourceLanguage: source);
        var translated = resp.TranslatedText;

        _cache.Add(key, translated, DateTimeOffset.UtcNow.AddDays(7));
        return translated;
    }
}
