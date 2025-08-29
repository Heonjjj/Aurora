using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtenScript : MonoBehaviour
{
    public int keypadNumver = 1;

    public UnityEvent KeypadClicked;



    private void OnMouseDown()
    {
       
        KeypadClicked.Invoke();
    }
}
