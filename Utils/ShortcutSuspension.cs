using nightreign_auto_storm_timer.Managers;

namespace nightreign_auto_storm_timer.Utils;

public sealed class ShortcutSuspension : IDisposable
{
    public ShortcutSuspension()
    {
        AppManager.Instance.ShortcutManager.Suspend();
    }

    public void Dispose()
    {
        AppManager.Instance.ShortcutManager.Resume();
    }
}

