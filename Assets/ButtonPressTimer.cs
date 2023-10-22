using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Elixir]
public class ButtonPressTimer : MonoBehaviour
{
    public float LastButtonPress = 0;
    public float TimeOffset = 2f;
    void Start()
    {
        LastButtonPress = Time.time;
    }

    public bool CheckTime()
    {
        if (Time.time - LastButtonPress >= TimeOffset)
        {
            LastButtonPress = Time.time;
            return true;
        }
        else
        {
            return false;
        }

    }
}
