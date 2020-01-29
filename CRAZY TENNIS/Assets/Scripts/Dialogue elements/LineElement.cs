using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class LineElement : IDialogueElement
{
    private string line;
    public LineElement(XmlNode data)
    {
        line = data.InnerText;
    }
    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        dialogue.speech = line;
    }
}
