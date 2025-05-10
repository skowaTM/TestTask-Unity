using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MyLogger
{
    private static string logFile = Application.dataPath + "/Log/my_log.log";


    public static void Log(string text)
    {
        if (!Directory.Exists(Path.GetDirectoryName(logFile)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logFile));
        }

        string textLog = $"[{DateTime.Now}] {text}\n";
        File.AppendAllText(logFile, textLog);
    }


    public static string Get()
    {
        if (!File.Exists(logFile))
        {
            return "";
        }

        return File.ReadAllText(logFile);
    }
}
