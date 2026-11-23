using KeyEventLogger.Helpers;
using KeyEventLogger.Models;

namespace KeyEventLogger.Tests;

public class KeyLogEntryTests
{
    private static readonly DateTime T = new(2026, 11, 23, 14, 5, 12, 345);

    [Fact]
    public void LogLine_ContainsTimeKindAndKeyName()
    {
        var entry = new KeyLogEntry(T, KeyEventKind.KeyDown, Keys.C, Keys.Control);

        string line = entry.ToLogLine();

        Assert.StartsWith("2026-11-23 14:05:12.345 | KeyDown", line);
        Assert.Contains("Ctrl + C", line);
    }

    [Fact]
    public void KeyDown_RoundTripsThroughText()
    {
        var original = new KeyLogEntry(T, KeyEventKind.KeyDown, Keys.S, Keys.Control | Keys.Shift, null, isRepeat: true);

        bool ok = KeyLogEntry.TryParse(original.ToLogLine(), out var parsed);

        Assert.True(ok);
        Assert.Equal(original.Timestamp, parsed!.Timestamp);
        Assert.Equal(KeyEventKind.KeyDown, parsed.Kind);
        Assert.Equal(Keys.S, parsed.KeyCode);
        Assert.Equal(Keys.Control | Keys.Shift, parsed.Modifiers);
        Assert.True(parsed.IsRepeat);
    }

    [Theory]
    [InlineData('a')]
    [InlineData('Ğ')]
    [InlineData(' ')]
    [InlineData('|')]
    [InlineData('\r')]
    public void KeyPress_RoundTripsThroughText(char c)
    {
        var original = new KeyLogEntry(T, KeyEventKind.KeyPress, Keys.None, Keys.None, c);

        bool ok = KeyLogEntry.TryParse(original.ToLogLine(), out var parsed);

        Assert.True(ok);
        Assert.Equal(c, parsed!.Character);
        Assert.Equal(KeyEventKind.KeyPress, parsed.Kind);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("tamamen çöp")]
    [InlineData("2026-11-23 14:05:12.345 | KeyDown | A")]
    [InlineData("2026-13-45 99:99:99.000 | KeyDown | A | k=65;m=0;r=0")]
    [InlineData("2026-11-23 14:05:12.345 | Uçmak | A | k=65;m=0;r=0")]
    [InlineData("2026-11-23 14:05:12.345 | KeyDown | A | k=abc;m=0")]
    [InlineData("2026-11-23 14:05:12.345 | KeyPress | 'a' | k=65")]
    [InlineData("2026-11-23 14:05:12.345 | KeyPress | ? | c=999999")]
    public void TryParse_RejectsInvalidLines(string line)
    {
        Assert.False(KeyLogEntry.TryParse(line, out var parsed));
        Assert.Null(parsed);
    }

    [Fact]
    public void KeyPressWithoutCharacter_Throws()
    {
        Assert.Throws<ArgumentException>(() => new KeyLogEntry(T, KeyEventKind.KeyPress, Keys.A));
    }

    [Fact]
    public void RepeatEntry_IsMarkedInLogLine()
    {
        var entry = new KeyLogEntry(T, KeyEventKind.KeyDown, Keys.A, Keys.None, null, isRepeat: true);

        Assert.Contains("(tekrar)", entry.ToLogLine());
    }
}

public class KeyNameFormatterTests
{
    [Theory]
    [InlineData(Keys.D1, "1")]
    [InlineData(Keys.D0, "0")]
    [InlineData(Keys.NumPad5, "Num 5")]
    [InlineData(Keys.Return, "Enter")]
    [InlineData(Keys.Space, "Space")]
    [InlineData(Keys.Back, "Backspace")]
    [InlineData(Keys.ControlKey, "Ctrl")]
    [InlineData(Keys.ShiftKey, "Shift")]
    [InlineData(Keys.Menu, "Alt")]
    [InlineData(Keys.OemPeriod, ".")]
    [InlineData(Keys.Oemcomma, ",")]
    [InlineData(Keys.A, "A")]
    [InlineData(Keys.F5, "F5")]
    public void FormatKey_GivesReadableNames(Keys key, string expected)
    {
        Assert.Equal(expected, KeyNameFormatter.FormatKey(key));
    }

    [Fact]
    public void Combination_ListsModifiersInOrder()
    {
        Assert.Equal("Ctrl + Shift + Alt + S", KeyNameFormatter.FormatCombination(Keys.S, Keys.Control | Keys.Shift | Keys.Alt));
    }

    [Fact]
    public void Combination_DoesNotRepeatModifierKeyItself()
    {
        // Ctrl tuşuna basınca KeyCode=ControlKey, Modifiers=Control geliyor -> "Ctrl + Ctrl" olmamalı
        Assert.Equal("Ctrl", KeyNameFormatter.FormatCombination(Keys.ControlKey, Keys.Control));
        Assert.Equal("Shift", KeyNameFormatter.FormatCombination(Keys.ShiftKey, Keys.Shift));
    }

    [Theory]
    [InlineData(Keys.ShiftKey, true)]
    [InlineData(Keys.RControlKey, true)]
    [InlineData(Keys.Menu, true)]
    [InlineData(Keys.A, false)]
    [InlineData(Keys.Space, false)]
    public void IsModifierKey(Keys key, bool expected)
    {
        Assert.Equal(expected, KeyNameFormatter.IsModifierKey(key));
    }

    [Theory]
    [InlineData('a', "'a'")]
    [InlineData(' ', "' ' (boşluk)")]
    [InlineData('\r', "\\r (Enter)")]
    [InlineData('\u0003', "0x03")]
    public void FormatChar(char c, string expected)
    {
        Assert.Equal(expected, KeyNameFormatter.FormatChar(c));
    }
}
