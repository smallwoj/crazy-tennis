using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class NameElement : IDialogueElement
{
    private string name;
    public NameElement(XmlNode data)
    {
        name = data.InnerText;
    }
    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        dialogue.speakerName.text = name;
    }
}
