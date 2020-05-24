using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [HideInInspector] public bool Pressed;
    [HideInInspector] public bool Up;
    public void OnPointerDown(PointerEventData eventData) {
        Pressed = true;
        Up = false;
    }

    public void OnPointerUp(PointerEventData eventData) {
        Pressed = false;
        Up= true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Up = true;
        Pressed = false;
    }

}
