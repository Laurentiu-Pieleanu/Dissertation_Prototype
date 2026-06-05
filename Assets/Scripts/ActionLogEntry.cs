using UnityEngine;

[System.Serializable]

public class ActionLogEntry
{
    public string agentType;
    public string actionType;
    public float time;
    public int frame;
    public float posX;
    public float posZ;
    public float rotY;

    public ActionLogEntry(string agentType, string actionType, float time, int frame,  Transform actorTransform)
    {
        this.agentType = agentType;
        this.actionType = actionType;
        this.time = time;
        this.frame = frame;
        
        posX = actorTransform.position.x;
        posZ = actorTransform.position.z;
        rotY = actorTransform.eulerAngles.y;
    }

    public override string ToString()
    {
        return agentType + "," + actionType + "," + time.ToString("F2") + "," + 
            posX.ToString("F2") + "," +
            posZ.ToString("F2") + "," +
            rotY.ToString("F2") + "," + frame;
    }
}
