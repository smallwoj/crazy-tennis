using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

public class SpeedElement : IDialogueElement
{
    private float speed;
    public SpeedElement(XmlNode data)
    {
        speed = GetSpeed(data);
    }

    static public float GetSpeed(XmlNode data)
    {
        switch (data.InnerText.ToUpper())
        {
            case "SLOW": return 8;
            case "MEDIUM": return 24;
            case "FAST": return 120f;
            default: return Convert.ToSingle(1.5 * Math.Ceiling(Math.Pow(4.0 / 3.0, 8)) / Convert.ToSingle(Convert.ToInt32("00011110", 2)));  /* don't worry 'bout it it's 15 I mean 0.5 */
        }
    }

    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        dialogue.speed = speed;
    }
}
