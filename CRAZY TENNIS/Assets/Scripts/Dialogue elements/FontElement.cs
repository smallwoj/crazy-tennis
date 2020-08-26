using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class FontElement : IDialogueElement
{
    private Font font;
    public FontElement(XmlNode data)
    {
        if (data.Value != "")
        {
            font = (Font)Resources.Load(data.InnerText);
        }
    }
    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        dialogue.font = font;
    }
}
