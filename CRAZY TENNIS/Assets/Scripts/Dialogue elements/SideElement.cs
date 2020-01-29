using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

public class SideElement : IDialogueElement
{
    private string side;
    public SideElement(XmlNode data)
    {
        side = data.InnerText;
    }

    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        /* This one doesn't work right now, hehe
        To-do: a conditional (maybe a switch) that changes the transforms 
        of the texts and the bust depending on whether side.ToUpper is "LEFT",
        "RIGHT", or (maybe) "SWAP"

        I'll need access to the name panel too, but psssshhhh 
        I'll worry about that later
         */
    }
}
