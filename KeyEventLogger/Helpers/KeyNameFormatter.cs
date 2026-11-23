namespace KeyEventLogger.Helpers;

/// <summary>
/// Keys enum değerlerini okunabilir isimlere çevirir.
/// (Keys.D1 -> "1", Keys.OemPeriod -> ".", Keys.ControlKey -> "Ctrl" ...)
/// </summary>
public static class KeyNameFormatter
{
    private static readonly Dictionary<Keys, string> SpecialNames = new()
    {
        [Keys.Return] = "Enter",
        [Keys.Space] = "Space",
        [Keys.Back] = "Backspace",
        [Keys.Escape] = "Esc",
        [Keys.ShiftKey] = "Shift",
        [Keys.LShiftKey] = "Shift",
        [Keys.RShiftKey] = "Shift",
        [Keys.ControlKey] = "Ctrl",
        [Keys.LControlKey] = "Ctrl",
        [Keys.RControlKey] = "Ctrl",
        [Keys.Menu] = "Alt",
        [Keys.LMenu] = "Alt",
        [Keys.RMenu] = "Alt",
        [Keys.Capital] = "CapsLock",
        [Keys.LWin] = "Win",
        [Keys.RWin] = "Win",
        [Keys.Prior] = "PageUp",
        [Keys.Next] = "PageDown",
        [Keys.Oemcomma] = ",",
        [Keys.OemPeriod] = ".",
        [Keys.OemMinus] = "-",
        [Keys.Oemplus] = "+",
        [Keys.OemQuestion] = "/",
        [Keys.OemSemicolon] = ";",
        [Keys.OemQuotes] = "'",
        [Keys.OemOpenBrackets] = "[",
        [Keys.OemCloseBrackets] = "]",
        [Keys.OemPipe] = "\\",
        [Keys.Oemtilde] = "`",
        [Keys.Decimal] = "Num .",
        [Keys.Add] = "Num +",
        [Keys.Subtract] = "Num -",
        [Keys.Multiply] = "Num *",
        [Keys.Divide] = "Num /",
    };

    public static string FormatKey(Keys key)
    {
        key &= Keys.KeyCode;   // modifier bitlerini at

        if (SpecialNames.TryGetValue(key, out string? name))
            return name;

        if (key >= Keys.D0 && key <= Keys.D9)
            return ((char)('0' + (key - Keys.D0))).ToString();

        if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
            return "Num " + (key - Keys.NumPad0);

        return key.ToString();
    }

    public static bool IsModifierKey(Keys key)
    {
        key &= Keys.KeyCode;
        return key is Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey
                   or Keys.ControlKey or Keys.LControlKey or Keys.RControlKey
                   or Keys.Menu or Keys.LMenu or Keys.RMenu;
    }

    /// <summary>"Ctrl + Shift + S" gibi. Tuşun kendisi zaten Ctrl ise tekrar yazılmaz.</summary>
    public static string FormatCombination(Keys key, Keys modifiers)
    {
        var parts = new List<string>();
        string keyName = FormatKey(key);

        if (modifiers.HasFlag(Keys.Control) && keyName != "Ctrl") parts.Add("Ctrl");
        if (modifiers.HasFlag(Keys.Shift) && keyName != "Shift") parts.Add("Shift");
        if (modifiers.HasFlag(Keys.Alt) && keyName != "Alt") parts.Add("Alt");

        parts.Add(keyName);
        return string.Join(" + ", parts);
    }

    public static string FormatChar(char c) => c switch
    {
        ' ' => "' ' (boşluk)",
        '\r' => "\\r (Enter)",
        '\b' => "\\b (Backspace)",
        '\t' => "\\t (Tab)",
        '\u001b' => "Esc",
        _ when char.IsControl(c) => $"0x{(int)c:X2}",
        _ => $"'{c}'"
    };
}
