using System;
using UnityEngine;

public class RemoteButton : MonoBehaviour,IUseble
{
   public enum ButtonState
   {
      Click,
      Hold
   }
   public event Action<IUseble>OnUsed;
   [SerializeField] public ButtonState ButtonType = ButtonState.Click;
   public bool IsActive{get;private set;}
   
   [SerializeField] private Color _used;
   [SerializeField] private Color _unused;
   private Material _material;
   private void Start()
   {
      _material = GetComponent<Renderer>().material;
      SetUseColor();
   }
   public void Use(IUseCallBack useCallBack=null)
   {
      if (ButtonType == ButtonState.Click)
      {
         IsActive = !IsActive; 
         OnUsed?.Invoke(this); 
         SetUseColor();
      }
      if (ButtonType == ButtonState.Hold)
      {
         useCallBack?.IsHoldCallback(true);
         IsActive = true;
         OnUsed?.Invoke(this);
      }
   }

   public IteractibleType GetInteractiveType()
   {
      return ButtonType == ButtonState.Click ? IteractibleType.ClickButton:IteractibleType.HoldButton;
   }

   public void StopUse()
   {
      if (ButtonType == ButtonState.Hold)
      {
         IsActive = false;
         OnUsed?.Invoke(this);
      }
   }
   public void OnButton()
   {
      IsActive = true;
      SetUseColor();
   }
   public void OffButton()
   {
      IsActive = false;
      SetUseColor();
   }
   private void SetUseColor()
   {
      _material.color = IsActive ? _used : _unused;
   }
}

