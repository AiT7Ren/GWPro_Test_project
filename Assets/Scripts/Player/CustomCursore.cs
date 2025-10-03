using System;
using UnityEngine;

public class CustomCursore:IDisposable
{
    Texture2D _cursorTexture; 
    private Vector2 _hotSpot = Vector2.zero; 
    private CursorMode _cursorMode = CursorMode.Auto;
    public CustomCursore(Texture2D cursorTexture)
    {
        _cursorTexture = cursorTexture;
        _hotSpot = new Vector2(
            cursorTexture.width * 0.5f,
            cursorTexture.height * 0.5f);
        SetCustomCursore();
    }
    private void SetCustomCursore()
    {
        Cursor.SetCursor(_cursorTexture, _hotSpot, _cursorMode);
    }
    public void Dispose()
    {
        // TODO back to normal
    }
    
    
    
    
}
