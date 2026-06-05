using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombatLogManager : MonoBehaviour
{
    private string participantID;

    public string levelName;
    public string aiType;
    
    private List<ActionLogEntry> allActions = new List<ActionLogEntry>();
    private bool hasExported = false;

    private void Awake()
    {
        participantID = SystemInfo.deviceUniqueIdentifier;
    }

    public void LogAction(ActionLogEntry entry)
    {
        allActions.Add(entry);
    }

    public void ExportLog()
    {
        if(hasExported)
            return;

        string folderPath = Path.Combine(Application.dataPath, "../AttackLogs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        string fileName =
            participantID + "_" +
            levelName + "_" +
            aiType + "_" +
            timestamp +
            "_AllActions.csv";
        
        string path = Path.Combine(folderPath, fileName);
        
        List<string> lines = new List<string>();
        lines.Add("AgentType,ActionType,Time,PosX,PosZ,RotY,Frame");

        foreach (ActionLogEntry entry in allActions)
        {
            lines.Add(entry.ToString());
        }
        
        File.WriteAllLines(path, lines);
        
        hasExported = true;
        
        Debug.Log("Combined combat log exported to: " + path);
    }
}
