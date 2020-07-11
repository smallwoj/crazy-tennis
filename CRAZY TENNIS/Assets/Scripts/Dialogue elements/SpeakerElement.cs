using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

public class SpeakerElement : IDialogueElement
{
    private List<IDialogueElement> elements = new List<IDialogueElement>();

    public SpeakerElement(XmlNode data)
    {
        foreach (XmlNode currentElement in data.ChildNodes)
        {
            switch (currentElement.Name)
            {
            case "Name": elements.Add(new NameElement(currentElement)); break;
            case "Bust": elements.Add(new BustElement(currentElement)); break;
            case "AnimatedBust": elements.Add(new AnimatedBustElement(currentElement)); break;
            //case "Side": elements.Add(new SideElement(currentElement)); break;
            case "Font": elements.Add(new FontElement(currentElement)); break;
            case "Speed": elements.Add(new SpeedElement(currentElement)); break;
            case "Voice": elements.Add(new VoiceElement(currentElement)); break;

            default: throw new Exception("Sorry to inconvenience you but you probably misspelt something :(");
            }
        }
    }

    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        // Applies the changes.
        foreach (IDialogueElement element in elements)
            element.ApplyChanges(dialogue);
    }
}