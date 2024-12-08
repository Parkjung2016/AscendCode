using UnityEngine;

namespace PJH.Manager
{
    public class CursorManager
    {
        private Texture2D _defaultCursorTexture;

        public void Init()
        {
            _defaultCursorTexture = Managers.Addressable.Load<Texture2D>("DefaultCursor");
            SetCursor(_defaultCursorTexture);
        }

        public void SetCursor(Texture2D cursorTexture)
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}