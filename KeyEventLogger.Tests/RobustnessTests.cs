using KeyEventLogger.Models;
using KeyEventLogger.Services;

namespace KeyEventLogger.Tests;

/// <summary>Regression tests for issues found during code review.</summary>
public class RobustnessTests
{
    private static readonly DateTime T0 = new(2026, 11, 23, 10, 0, 0);

    private static KeyLogSession RecordingSession()
    {
        var session = new KeyLogSession(() => T0);
        session.Start();
        return session;
    }

    [Theory]
    [InlineData("2026-11-23 01:02:03.456 | KeyDown  | A | k=-5;m=0;r=0")]
    [InlineData("2026-11-23 01:02:03.456 | KeyDown  | A | k=65;m=-1;r=0")]
    [InlineData("2026-11-23 01:02:03.456 | KeyDown  | A | k=65;m=65;r=0")]
    [InlineData("2026-11-23 01:02:03.456 | KeyDown  | A | k=70000;m=0;r=0")]
    [InlineData("2026-11-23 01:02:03.456 | 0        | A | k=65;m=0;r=0")]
    public void TryParse_RejectsOutOfRangeValues(string line)
    {
        Assert.False(KeyLogEntry.TryParse(line, out _));
    }

    [Fact]
    public void TryParse_KeyUpNeverMarkedAsRepeat()
    {
        Assert.True(KeyLogEntry.TryParse("2026-11-23 01:02:03.456 | KeyUp    | A | k=65;m=0;r=1", out var entry));
        Assert.False(entry!.IsRepeat);
    }

    [Fact]
    public void ShiftNumpad_KeyCodeMismatch_DoesNotLeaveKeyStuck()
    {
        var session = RecordingSession();
        // NumLock off: Shift+Num4 arrives as Left on KeyDown, NumPad4 on KeyUp
        session.RecordKeyDown(Keys.Left, Keys.Shift);
        session.RecordKeyUp(Keys.ShiftKey, Keys.None);
        session.RecordKeyUp(Keys.NumPad4, Keys.None);

        var next = session.RecordKeyDown(Keys.Left, Keys.None);

        Assert.False(next!.IsRepeat);
    }

    [Fact]
    public void HeldKey_IsStillDetectedAsRepeat()
    {
        var session = RecordingSession();
        session.RecordKeyDown(Keys.A, Keys.None);
        var again = session.RecordKeyDown(Keys.A, Keys.None);
        Assert.True(again!.IsRepeat);
    }

    [Fact]
    public void Import_RaisesSingleEvent_AndKeepsTimeOrder()
    {
        var session = RecordingSession();
        session.RecordKeyDown(Keys.B, Keys.None);   // at T0
        int added = 0, imported = 0;
        session.EntryAdded += (_, _) => added++;
        session.Imported += (_, _) => imported++;

        var loaded = Enumerable.Range(1, 500)
            .Select(i => new KeyLogEntry(T0.AddSeconds(-i), KeyEventKind.KeyDown, Keys.X))
            .ToList();
        session.Import(loaded);

        Assert.Equal(0, added);
        Assert.Equal(1, imported);
        Assert.Equal(501, session.Entries.Count);
        Assert.True(session.Entries.Zip(session.Entries.Skip(1)).All(p => p.First.Timestamp <= p.Second.Timestamp));
    }

    [Fact]
    public void Import_WithReplace_DropsCurrentEntries()
    {
        var session = RecordingSession();
        session.RecordKeyDown(Keys.B, Keys.None);
        session.Import(new[] { new KeyLogEntry(T0, KeyEventKind.KeyDown, Keys.X) }, replace: true);
        Assert.Single(session.Entries);
        Assert.Equal(Keys.X, session.Entries[0].KeyCode);
    }

    [Fact]
    public void SaveAndLoad_UnderTurkishCulture_RoundTrips()
    {
        var old = System.Globalization.CultureInfo.CurrentCulture;
        string path = Path.Combine(Path.GetTempPath(), $"kel-tr-{Guid.NewGuid():N}.txt");
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            var entries = new[]
            {
                new KeyLogEntry(T0, KeyEventKind.KeyDown, Keys.I, Keys.Shift),
                new KeyLogEntry(T0, KeyEventKind.KeyPress, Keys.None, Keys.None, 'ı'),
                new KeyLogEntry(T0, KeyEventKind.KeyPress, Keys.None, Keys.None, '|'),
            };
            LogFileService.Save(path, entries);
            var result = LogFileService.Load(path);

            Assert.Equal(0, result.SkippedLines);
            Assert.Equal(entries.Select(e => e.ToLogLine()), result.Entries.Select(e => e.ToLogLine()));
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = old;
            File.Delete(path);
        }
    }
}
