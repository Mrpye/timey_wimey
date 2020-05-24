using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [HideInInspector] public bool Pressed;
    public void OnPointerDown(PointerEventData eventData) {
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        Pressed = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
