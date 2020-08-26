using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class AnimatedBustElement : IDialogueElement
{
    private string animationName;
    public AnimatedBustElement(XmlNode data)
    {
        //animation = Resources.Load<Animation>(data.InnerText);
        animationName = data.InnerText;
    }
    public void ApplyChanges(DialogueBehaviour dialogue)
    {
        // Generalization is hard
        //dialogue.bust.GetComponent<Animation>() = animation;
        //Debug.Log(dialogue.bust.GetComponent<Animator>().runtimeAnimatorController.animationClips[0]);
        //dialogue.bust.GetComponent<Animator>().runtimeAnimatorController.animationClips[0] = animation.clip;
        // dialogue.bust.GetComponent<Animator>().Play(animationName);
    }
}
