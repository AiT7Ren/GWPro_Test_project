
using System;
using UnityEngine;

public class HelpTextSetter : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(SetText), 0.1f);
    }
    private void SetText()
    {
        if(Tutorial.Instance!=null)Tutorial.Instance.SetText(transform.GetChild(0).gameObject.transform);
    }
}
