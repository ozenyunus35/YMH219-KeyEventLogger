using System.Globalization;
using KeyEventLogger.Helpers;

namespace KeyEventLogger.Models;

public enum KeyEventKind
{
    KeyDown,
    KeyUp,
    KeyPress
}

/// <summary>
/// Yakalanan tek bir klavye olayı. Oluşturulduktan sonra değiştirilemez.
/// </summary>
public sealed class KeyLogEntry
{
    public const string TimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    private const string Separator = " | ";

    public KeyLogEntry(DateTime timestamp, KeyEventKind kind, Keys keyCode, Keys modifiers = Keys.None,
                       char? character = null, bool isRepeat = false)
    {
        if (kind == KeyEventKind.KeyPress && character is null)
            throw new ArgumentException("KeyPress entries must have a character.", nameof(character));

        Timestamp = timestamp;
        Kind = kind;
        KeyCode = keyCode;
        Modifiers = modifiers;
        Character = character;
        IsRepeat = isRepeat;
    }

    public DateTime Timestamp { get; }
    public KeyEventKind Kind { get; }
    public Keys KeyCode { get; }
    public Keys Modifiers { get; }
    public char? Character { get; }

    /// <summary>Tuş basılı tutulunca Windows KeyDown'ı tekrar gönderiyor, o durumda true.</summary>
    public bool IsRepeat { get; }

    public string DisplayText => Kind == KeyEventKind.KeyPress
        ? KeyNameFormatter.FormatChar(Character!.Value)
        : KeyNameFormatter.FormatCombination(KeyCode, Modifiers);

    /// <summary>
    /// Hem ekranda hem .txt dosyasında kullanılan satır biçimi:
    /// 2026-11-23 14:05:12.345 | KeyDown  | Ctrl + C | k=67;m=131072;r=0
    /// Son sütunda ham değerler var, böylece dosya tekrar yüklenebiliyor.
    /// </summary>
    public string ToLogLine()
    {
        string raw = Kind == KeyEventKind.KeyPress
            ? $"c={(int)Character!.Value}"
            : $"k={(int)KeyCode};m={(int)Modifiers};r={(IsRepeat ? 1 : 0)}";

        string text = DisplayText + (IsRepeat ? " (tekrar)" : "");

        return string.Join(Separator,
            Timestamp.ToString(TimeFormat, CultureInfo.InvariantCulture),
            Kind.ToString().PadRight(8),
            text,
            raw);
    }

    public static bool TryParse(string? line, out KeyLogEntry? entry)
    {
        entry = null;
        if (string.IsNullOrWhiteSpace(line))
            return false;

        string[] parts = line.Split(Separator);
        if (parts.Length != 4)
            return false;

        if (!DateTime.TryParseExact(parts[0], TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
            return false;

        if (!Enum.TryParse(parts[1].Trim(), out KeyEventKind kind) || !Enum.IsDefined(kind))
            return false;

        Dictionary<string, int> values = new();
        foreach (string pair in parts[3].Split(';'))
        {
            string[] kv = pair.Split('=');
            if (kv.Length != 2 || !int.TryParse(kv[1], out int number))
                return false;
            values[kv[0].Trim()] = number;
        }

        if (kind == KeyEventKind.KeyPress)
        {
            if (!values.TryGetValue("c", out int c) || c < 0 || c > char.MaxValue)
                return false;
            entry = new KeyLogEntry(time, kind, Keys.None, Keys.None, (char)c);
            return true;
        }

        if (!values.TryGetValue("k", out int key) || !values.TryGetValue("m", out int mod))
            return false;

        values.TryGetValue("r", out int repeat);
        entry = new KeyLogEntry(time, kind, (Keys)key, (Keys)mod, null, repeat == 1);
        return true;
    }

    public override string ToString() => ToLogLine();
}
