using KeyEventLogger.Models;
using KeyEventLogger.Services;

namespace KeyEventLogger.Tests;

public class LogFileServiceTests : IDisposable
{
    private readonly string dir = Path.Combine(Path.GetTempPath(), "kel-tests-" + Guid.NewGuid().ToString("N"));
    private static readonly DateTime T = new(2026, 11, 23, 12, 0, 0);

    public LogFileServiceTests() => Directory.CreateDirectory(dir);

    public void Dispose()
    {
        if (Directory.Exists(dir))
            Directory.Delete(dir, recursive: true);
    }

    private static List<KeyLogEntry> Sample() => new()
    {
        new KeyLogEntry(T, KeyEventKind.KeyDown, Keys.H, Keys.Shift),
        new KeyLogEntry(T.AddMilliseconds(5), KeyEventKind.KeyPress, Keys.None, Keys.None, 'H'),
        new KeyLogEntry(T.AddMilliseconds(80), KeyEventKind.KeyUp, Keys.H, Keys.Shift),
        new KeyLogEntry(T.AddMilliseconds(400), KeyEventKind.KeyDown, Keys.I, Keys.None, null, isRepeat: true),
        new KeyLogEntry(T.AddMilliseconds(410), KeyEventKind.KeyPress, Keys.None, Keys.None, 'ş'),
    };

    [Fact]
    public void SaveThenLoad_GivesSameEntries()
    {
        string path = Path.Combine(dir, "log.txt");
        var entries = Sample();

        LogFileService.Save(path, entries);
        LoadResult result = LogFileService.Load(path);

        Assert.Equal(0, result.SkippedLines);
        Assert.Equal(entries.Count, result.Entries.Count);
        for (int i = 0; i < entries.Count; i++)
        {
            Assert.Equal(entries[i].ToLogLine(), result.Entries[i].ToLogLine());
        }
    }

    [Fact]
    public void SavedFile_HasHeaderAndOneLinePerEntry()
    {
        string path = Path.Combine(dir, "log.txt");

        LogFileService.Save(path, Sample());
        string[] lines = File.ReadAllLines(path);

        Assert.StartsWith("#", lines[0]);
        Assert.Contains(lines, l => l.Contains("Kayıt sayısı: 5"));
        Assert.Equal(5, lines.Count(l => !l.StartsWith('#')));
    }

    [Fact]
    public void Load_SkipsBrokenLines_AndComments()
    {
        string path = Path.Combine(dir, "broken.txt");
        File.WriteAllLines(path, new[]
        {
            "# yorum satırı",
            Sample()[0].ToLogLine(),
            "bozuk satır",
            "",
            Sample()[1].ToLogLine(),
            "2026-11-23 12:00:00.000 | KeyDown | A | k=x"
        });

        LoadResult result = LogFileService.Load(path);

        Assert.Equal(2, result.Entries.Count);
        Assert.Equal(2, result.SkippedLines);
    }

    [Fact]
    public void Load_MissingFile_Throws()
    {
        Assert.Throws<FileNotFoundException>(() => LogFileService.Load(Path.Combine(dir, "yok.txt")));
    }

    [Fact]
    public void Save_EmptyPath_Throws()
    {
        Assert.Throws<ArgumentException>(() => LogFileService.Save(" ", Sample()));
    }

    [Fact]
    public void SuggestedFileName_ContainsDateAndTxt()
    {
        string name = LogFileService.SuggestFileName(new DateTime(2026, 11, 23, 14, 5, 0));

        Assert.Equal("tus-kaydi-2026-11-23_1405.txt", name);
    }
}
