using KeyEventLogger.Models;
using KeyEventLogger.Services;

namespace KeyEventLogger.Tests;

public class KeyLogSessionTests
{
    private DateTime now = new(2026, 11, 23, 10, 0, 0);
    private KeyLogSession NewSession() => new(() => now);

    [Fact]
    public void NothingIsRecorded_BeforeStart()
    {
        var session = NewSession();

        var entry = session.RecordKeyDown(Keys.A, Keys.None);

        Assert.Null(entry);
        Assert.Empty(session.Entries);
        Assert.False(session.IsRecording);
    }

    [Fact]
    public void Start_RecordsEvents_WithTimestamp()
    {
        var session = NewSession();
        session.Start();

        session.RecordKeyDown(Keys.A, Keys.None);
        session.RecordKeyPress('a');
        session.RecordKeyUp(Keys.A, Keys.None);

        Assert.Equal(3, session.Entries.Count);
        Assert.All(session.Entries, e => Assert.Equal(now, e.Timestamp));
        Assert.Equal(new[] { KeyEventKind.KeyDown, KeyEventKind.KeyPress, KeyEventKind.KeyUp },
                     session.Entries.Select(e => e.Kind));
        Assert.Equal(now, session.RecordingStartedAt);
    }

    [Fact]
    public void Stop_KeepsOldEntries_ButIgnoresNewOnes()
    {
        var session = NewSession();
        session.Start();
        session.RecordKeyDown(Keys.A, Keys.None);

        session.Stop();
        session.RecordKeyDown(Keys.B, Keys.None);

        Assert.Single(session.Entries);
        Assert.False(session.IsRecording);
    }

    [Fact]
    public void HoldingKey_MarksFollowingKeyDownsAsRepeat()
    {
        var session = NewSession();
        session.Start();

        var first = session.RecordKeyDown(Keys.A, Keys.None);
        var second = session.RecordKeyDown(Keys.A, Keys.None);
        var third = session.RecordKeyDown(Keys.A, Keys.None);
        session.RecordKeyUp(Keys.A, Keys.None);
        var again = session.RecordKeyDown(Keys.A, Keys.None);

        Assert.False(first!.IsRepeat);
        Assert.True(second!.IsRepeat);
        Assert.True(third!.IsRepeat);
        Assert.False(again!.IsRepeat);
    }

    [Fact]
    public void ResetPressedKeys_AfterFocusLoss_NextKeyDownIsNotRepeat()
    {
        var session = NewSession();
        session.Start();
        session.RecordKeyDown(Keys.A, Keys.None);   // KeyUp hiç gelmedi (pencere değişti)

        session.ResetPressedKeys();
        var next = session.RecordKeyDown(Keys.A, Keys.None);

        Assert.False(next!.IsRepeat);
    }

    [Fact]
    public void Events_AreRaised()
    {
        var session = NewSession();
        int added = 0, stateChanges = 0, cleared = 0;
        session.EntryAdded += (_, _) => added++;
        session.RecordingStateChanged += (_, _) => stateChanges++;
        session.Cleared += (_, _) => cleared++;

        session.Start();
        session.Start();                    // ikinci kez başlatmak olay üretmemeli
        session.RecordKeyDown(Keys.A, Keys.None);
        session.Stop();
        session.Clear();

        Assert.Equal(1, added);
        Assert.Equal(2, stateChanges);
        Assert.Equal(1, cleared);
        Assert.Empty(session.Entries);
    }

    [Fact]
    public void InRange_FiltersByTime()
    {
        var session = NewSession();
        session.Start();
        for (int i = 0; i < 10; i++)
        {
            session.RecordKeyDown(Keys.A + i, Keys.None);
            now = now.AddMinutes(1);
        }

        var middle = session.InRange(new DateTime(2026, 11, 23, 10, 3, 0), new DateTime(2026, 11, 23, 10, 6, 0)).ToList();

        Assert.Equal(4, middle.Count);
        Assert.Equal(Keys.D, middle[0].KeyCode);
    }

    [Fact]
    public void InRange_RejectsReversedRange()
    {
        var session = NewSession();
        Assert.Throws<ArgumentException>(() => session.InRange(now, now.AddMinutes(-1)).ToList());
    }

    [Fact]
    public void Import_AddsEntries_EvenWhenNotRecording()
    {
        var session = NewSession();
        var loaded = new[] { new KeyLogEntry(now, KeyEventKind.KeyDown, Keys.X) };

        session.Import(loaded);

        Assert.Single(session.Entries);
    }
}

public class KeyStatisticsTests
{
    private static readonly DateTime T0 = new(2026, 11, 23, 9, 0, 0);

    private static List<KeyLogEntry> Sample()
    {
        var list = new List<KeyLogEntry>();
        void Down(Keys k, int sec, bool repeat = false) =>
            list.Add(new KeyLogEntry(T0.AddSeconds(sec), KeyEventKind.KeyDown, k, Keys.None, null, repeat));

        Down(Keys.A, 0); Down(Keys.A, 5); Down(Keys.A, 10, repeat: true);
        Down(Keys.B, 15); Down(Keys.Space, 20); Down(Keys.Space, 25); Down(Keys.A, 30);
        list.Add(new KeyLogEntry(T0.AddSeconds(31), KeyEventKind.KeyUp, Keys.A));
        list.Add(new KeyLogEntry(T0.AddSeconds(32), KeyEventKind.KeyPress, Keys.None, Keys.None, 'a'));
        return list;
    }

    [Fact]
    public void Total_CountsOnlyRealKeyDowns()
    {
        Assert.Equal(6, KeyStatistics.TotalKeyPresses(Sample()));
    }

    [Fact]
    public void Distinct_CountsDifferentKeys()
    {
        Assert.Equal(3, KeyStatistics.DistinctKeys(Sample()));
    }

    [Fact]
    public void TopKeys_SortedByCount()
    {
        var top = KeyStatistics.TopKeys(Sample(), 2);

        Assert.Equal(2, top.Count);
        Assert.Equal(("A", 3), top[0]);
        Assert.Equal(("Space", 2), top[1]);
    }

    [Fact]
    public void TopKeys_ZeroOrNegative_ReturnsEmpty()
    {
        Assert.Empty(KeyStatistics.TopKeys(Sample(), 0));
    }

    [Fact]
    public void KeysPerMinute_UsesFirstAndLastPress()
    {
        // 6 basış, 0. ve 30. saniye arası = 0.5 dakika -> 12 tuş/dk
        Assert.Equal(12, KeyStatistics.KeysPerMinute(Sample()), 3);
    }

    [Fact]
    public void KeysPerMinute_TooShortOrEmpty_IsZero()
    {
        Assert.Equal(0, KeyStatistics.KeysPerMinute(Array.Empty<KeyLogEntry>()));
        var quick = new[]
        {
            new KeyLogEntry(T0, KeyEventKind.KeyDown, Keys.A),
            new KeyLogEntry(T0.AddSeconds(1), KeyEventKind.KeyDown, Keys.B)
        };
        Assert.Equal(0, KeyStatistics.KeysPerMinute(quick));
    }
}
