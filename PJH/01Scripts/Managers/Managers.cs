
namespace PJH.Manager
{
    public static class Managers
    {
        public static AddressableManager Addressable { get; } = new();
        public static SceneManagerEx Scene { get; } = new();
        public static ResourceManager Resource { get; } = new();
        public static PopupTextManager PopupText { get; } = new();
        public static CursorManager Cursor { get; } = new();
        public static FMODSoundManager FMODSound { get; } = new();
    }
}