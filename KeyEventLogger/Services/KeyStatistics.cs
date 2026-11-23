using KeyEventLogger.Helpers;
using KeyEventLogger.Models;

namespace KeyEventLogger.Services;

/// <summary>
/// Kayıtlar üzerinden istatistik hesaplar. Sadece gerçek tuş basışları sayılıyor:
/// KeyUp, KeyPress ve basılı tutmaktan gelen tekrarlar hariç.
/// </summary>
public static class KeyStatistics
{
    private static IEnumerable<KeyLogEntry> RealPresses(IEnumerable<KeyLogEntry> entries) =>
        entries.Where(e => e.Kind == KeyEventKind.KeyDown && !e.IsRepeat);

    public static int TotalKeyPresses(IEnumerable<KeyLogEntry> entries) => RealPresses(entries).Count();

    public static int DistinctKeys(IEnumerable<KeyLogEntry> entries) =>
        RealPresses(entries).Select(e => KeyNameFormatter.FormatKey(e.KeyCode)).Distinct().Count();

    /// <summary>En çok basılan tuşlar, çoktan aza. Eşitlikte alfabetik.</summary>
    public static IReadOnlyList<(string Key, int Count)> TopKeys(IEnumerable<KeyLogEntry> entries, int count)
    {
        if (count <= 0)
            return Array.Empty<(string, int)>();

        return RealPresses(entries)
            .GroupBy(e => KeyNameFormatter.FormatKey(e.KeyCode))
            .Select(g => (Key: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Key, StringComparer.Ordinal)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Dakikadaki tuş sayısı. İlk ve son basış arası 5 saniyeden kısaysa
    /// anlamsız büyük sayılar çıkmasın diye 0 dönüyor.
    /// </summary>
    public static double KeysPerMinute(IEnumerable<KeyLogEntry> entries)
    {
        var presses = RealPresses(entries).ToList();
        if (presses.Count < 2)
            return 0;

        TimeSpan span = presses.Max(e => e.Timestamp) - presses.Min(e => e.Timestamp);
        if (span.TotalSeconds < 5)
            return 0;

        return presses.Count / span.TotalMinutes;
    }
}
