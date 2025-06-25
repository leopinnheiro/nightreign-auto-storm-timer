using Gma.System.MouseKeyHook;
using nightreign_auto_storm_timer.Enums;
using nightreign_auto_storm_timer.Models;
using nightreign_auto_storm_timer.Services;
using nightreign_auto_storm_timer.Utils;
using System.Diagnostics;
using System.Windows.Input;
using System.Xml.Linq;

namespace nightreign_auto_storm_timer.Managers;

public class ShortcutManager : IDisposable
{
    private readonly Dictionary<string, Action> _nameToAction = [];
    private readonly Dictionary<Shortcut, string> _shortcutToName = [];

    private readonly HashSet<Key> _keysPressed = [];

    private TaskCompletionSource<Shortcut>? _captureTcs;
    private bool _isCapturing = false;
    private bool _isSuspended = false;

    private IKeyboardMouseEvents _globalHook;

    public ShortcutManager()
    {
        _globalHook = Hook.GlobalEvents();
        _globalHook.KeyDown += OnKeyDown;
        _globalHook.KeyUp += OnKeyUp;

        AppConfigManager.Instance.RegisterForUpdates(OnConfigUpdated);
    }

    public void RegisterNamedShortcut(ShortcutAction action, Action callback)
    {
        string name = ShortcutNames.GetName(action);
        _nameToAction[name] = callback;

        var config = AppConfigManager.Instance.CurrentConfig;
        if (config.Shortcuts.TryGetValue(name, out var configItem) && configItem.Enabled)
        {
            _shortcutToName[configItem.Shortcut] = name;
        }
    }

    public Task<Shortcut> CaptureNextShortcutAsync()
    {
        _isCapturing = true;
        _captureTcs = new TaskCompletionSource<Shortcut>();
        return _captureTcs.Task;
    }

    private void OnKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
    {
        var key = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);
        _keysPressed.Add(key);

        var modifiers = GetCurrentModifiers();
        var shortcut = new Shortcut(key, modifiers);

        if (_isCapturing && _captureTcs is not null)
        {
            if (!IsModifierKey(key))
            {
                _isCapturing = false;
                _captureTcs.TrySetResult(shortcut);
            }
            return;
        }
        
        if (_isSuspended)
            return;

        if (_shortcutToName.TryGetValue(shortcut, out var name)
            && _nameToAction.TryGetValue(name, out var action))
        {
            action?.Invoke();
            e.Handled = true;
        }
    }

    private void OnKeyUp(object? sender, System.Windows.Forms.KeyEventArgs e)
    {
        var key = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);
        _keysPressed.Remove(key);
    }

    public void ExecuteShortcut(ShortcutAction action)
    {
        string name = ShortcutNames.GetName(action);

        if (_nameToAction.TryGetValue(name, out var callback))
            callback.Invoke();
    }

    private void OnConfigUpdated(AppConfigNew config)
    {
        _shortcutToName.Clear();

        foreach (var (name, configItem) in config.Shortcuts)
        {
            if (!configItem.Enabled)
                continue;

            if (_nameToAction.ContainsKey(name))
            {
                _shortcutToName[configItem.Shortcut] = name;
            }
        }
        LogService.Info($"[ShortcutManager] Shortcuts count {_shortcutToName.Count}");
    }

    private static bool IsModifierKey(Key key)
    {
        return key is Key.LeftCtrl or Key.RightCtrl
            or Key.LeftAlt or Key.RightAlt
            or Key.LeftShift or Key.RightShift
            or Key.LWin or Key.RWin;
    }

    private ModifierKeys GetCurrentModifiers()
    {
        ModifierKeys modifiers = ModifierKeys.None;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            modifiers |= ModifierKeys.Control;
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            modifiers |= ModifierKeys.Alt;
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            modifiers |= ModifierKeys.Shift;
        if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
            modifiers |= ModifierKeys.Windows;
        return modifiers;
    }

    public void Dispose()
    {
        _globalHook.KeyDown -= OnKeyDown;
        _globalHook.KeyUp -= OnKeyUp;
        _globalHook.Dispose();
    }

    public void Suspend()
    {
        _isSuspended = true;
    }

    public void Resume()
    {
        _isSuspended = false;
    }
}
