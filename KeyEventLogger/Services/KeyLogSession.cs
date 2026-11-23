using KeyEventLogger.Models;

namespace KeyEventLogger.Services;

/// <summary>
/// Kaydedilen olayları ve "kayıt açık/kapalı" durumunu tutar.
/// Form tuş olaylarını sadece buraya iletiyor, saklanıp saklanmayacağına bu sınıf karar veriyor.
/// </summary>
public class KeyLogSession
{
    private readonly List<KeyLogEntry> entries = new();
    private readonly HashSet<Keys> pressedKeys = new();
    private readonly Func<DateTime> clock;

    public KeyLogSession() : this(() => DateTime.Now) { }

    /// <param name="clock">Testlerde sabit zaman vermek için.</param>
    public KeyLogSession(Func<DateTime> clock)
    {
        this.clock = clock;
    }

    public bool IsRecording { get; private set; }
    public DateTime? RecordingStartedAt { get; private set; }
    public IReadOnlyList<KeyLogEntry> Entries => entries;

    public event EventHandler<KeyLogEntry>? EntryAdded;
    public event EventHandler? RecordingStateChanged;
    public event EventHandler? Cleared;

    /// <summary>Dosyadan toplu yükleme bitti (binlerce EntryAdded yerine tek olay).</summary>
    public event EventHandler? Imported;

    public void Start()
    {
        if (IsRecording)
            return;

        IsRecording = true;
        RecordingStartedAt = clock();
        pressedKeys.Clear();
        RecordingStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        if (!IsRecording)
            return;

        IsRecording = false;
        pressedKeys.Clear();
        RecordingStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        entries.Clear();
        Cleared?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Pencere odaktan çıkınca basılı tuş bilgisi geçersiz olur.</summary>
    public void ResetPressedKeys() => pressedKeys.Clear();

    public KeyLogEntry? RecordKeyDown(Keys keyCode, Keys modifiers)
    {
        // Tuş zaten basılıysa Windows'un otomatik tekrar ettiği KeyDown'dır
        bool isRepeat = !pressedKeys.Add(keyCode);
        return Add(new KeyLogEntry(clock(), KeyEventKind.KeyDown, keyCode, modifiers, null, isRepeat));
    }

    public KeyLogEntry? RecordKeyUp(Keys keyCode, Keys modifiers)
    {
        // NumLock kapalıyken Shift+Num4 KeyDown'da "Left", KeyUp'ta "NumPad4" olarak geliyor.
        // Basılmamış görünen bir tuş bırakılıyorsa basılı tuş listesi güvenilmez: temizleniyor,
        // yoksa "Left" sonsuza kadar basılı sanılıp sonraki gerçek basışlar "tekrar" sayılıyordu.
        if (!pressedKeys.Remove(keyCode) && (modifiers & (Keys.Shift | Keys.Control | Keys.Alt)) == Keys.None)
            pressedKeys.Clear();
        return Add(new KeyLogEntry(clock(), KeyEventKind.KeyUp, keyCode, modifiers));
    }

    public KeyLogEntry? RecordKeyPress(char character)
    {
        return Add(new KeyLogEntry(clock(), KeyEventKind.KeyPress, Keys.None, Keys.None, character));
    }

    /// <summary>Dosyadan yüklenen kayıtları eklemek için (kayıt kapalı olsa da).</summary>
    public void Import(IEnumerable<KeyLogEntry> loaded, bool replace = false)
    {
        if (replace)
            entries.Clear();
        entries.AddRange(loaded);
        // Farklı zamanlarda kaydedilmiş dosyalar birleştirilince liste zaman sırasında kalsın (sıralama kararlı)
        var sorted = entries.OrderBy(e => e.Timestamp).ToList();
        entries.Clear();
        entries.AddRange(sorted);
        Imported?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerable<KeyLogEntry> InRange(DateTime from, DateTime to)
    {
        if (to < from)
            throw new ArgumentException("Bitiş zamanı başlangıçtan önce olamaz.");

        return entries.Where(e => e.Timestamp >= from && e.Timestamp <= to);
    }

    private KeyLogEntry? Add(KeyLogEntry entry)
    {
        if (!IsRecording)
            return null;

        entries.Add(entry);
        EntryAdded?.Invoke(this, entry);
        return entry;
    }
}
