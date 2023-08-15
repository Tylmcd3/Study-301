using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    static bool GameBuild = true;
    public static void LogToConsole(string str, System.ConsoleColor col = System.ConsoleColor.Green)
    {
        if (GameBuild)
        {
            MelonLoader.MelonLogger.Msg(col, str);
        }
        else
        {
            Debug.Log(str);
        }
        
        
    }
}
