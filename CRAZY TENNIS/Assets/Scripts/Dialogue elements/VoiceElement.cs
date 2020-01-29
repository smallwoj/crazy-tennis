using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Xml;

public class VoiceElement : IDialogueElement
{
    private AudioClip voice;
    public VoiceElement(XmlNode data)
    {
        if (data.InnerText != "")
        {
            voice = Resources.Load<AudioClip>(data.InnerText);
        }
        else
        {
            voice = null;
        }
    }
    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        dialogue.voice.clip = voice;
    }
}
