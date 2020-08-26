using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class BustElement : IDialogueElement
{
    private Sprite bust;
    public BustElement(XmlNode data)
    {
        bust = Resources.Load<Sprite>(data.InnerText);
    }
    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        dialogue.bust.sprite = bust;
        // Stop the current animation
        // dialogue.bust.GetComponent<Animator>().Play("Don't");
    }
}
