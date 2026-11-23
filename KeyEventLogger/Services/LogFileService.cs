using System.Text;
using KeyEventLogger.Models;

namespace KeyEventLogger.Services;

public sealed record LoadResult(IReadOnlyList<KeyLogEntry> Entries, int SkippedLines);

/// <summary>
/// Kayıtları .txt dosyasına yazar ve geri okur.
/// '#' ile başlayan satırlar açıklama satırı, yüklerken atlanıyor.
/// Hata olursa exception fırlatıyor, kullanıcıya mesajı form gösteriyor.
/// </summary>
public static class LogFileService
{
    public static void Save(string path, IReadOnlyCollection<KeyLogEntry> entries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using var writer = new StreamWriter(path, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        writer.WriteLine("# Key Event Logger - YMH219");
        writer.WriteLine($"# Oluşturulma: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        writer.WriteLine($"# Kayıt sayısı: {entries.Count}");
        writer.WriteLine("# Biçim: zaman | olay | tuş | ham değerler");
        writer.WriteLine("#");

        foreach (var entry in entries)
            writer.WriteLine(entry.ToLogLine());
    }

    public static LoadResult Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var entries = new List<KeyLogEntry>();
        int skipped = 0;

        using var reader = new StreamReader(path, Encoding.UTF8);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (KeyLogEntry.TryParse(line, out KeyLogEntry? entry))
                entries.Add(entry!);
            else
                skipped++;
        }

        return new LoadResult(entries, skipped);
    }

    public static string SuggestFileName(DateTime now) => $"tus-kaydi-{now:yyyy-MM-dd_HHmm}.txt";
}
