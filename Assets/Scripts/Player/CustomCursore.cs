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
