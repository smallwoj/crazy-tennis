/*
Alex Kitt and Josh Smallwood
July 17, 2019 (Wax Emoji Day  蠟  蠟  蠟  蠟 )
Project ☣️
DialogueBehaviour: This one defines the  蠟 behaviour for the  蠟  dialogue  蠟  蠟  蠟 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;

public class DialogueBehaviour : MonoBehaviour
{
    public TextAsset script; // XML file containing the script for this dialogue
    public Text speakerName; // The name of the character who is talking (the "speaker")
    public string speech; // The text to be displayed (whatever the speaker is speaking)
    public Text displayedSpeech;    // The text currently being displayed
    public Image bust;  // The image of the talking character
    public Font font;   // The font the speakers's speech is rendered in. Defaults to size-25 Arial
    public AudioSource voice;   // The speaker's voice
    public float speed;   // The rate at which characters appear

    private Queue<IDialogueElement> elements;    // The elements (lines, busts, sounds, etc.) of the dialogue
    private IDialogueElement currentElement;    // The dialogue element current applying its changes to the dialogue canvas
    private float revealedChars = 0;  // How many characters of the current line/string are visible
    /// <summary> The thing that makes it go whoosh </summary>
    private Animator anim;
    /// <summary> Little animated picture telling the player to press a button to advance </summary>
    private GameObject buttonPrompt;

    // Start is uhhhh I forget the rest
    void Start()
    {
        XmlDocument scriptXml = new XmlDocument();
        scriptXml.LoadXml(script.text);
        LoadElements();

        // Uncomment this if necessary ;)
        // // Dequeue the first element
        // if (elements.Count != 0)    // (or don't, if LoadElements didn't do anything)
        // {
        //     currentElement = elements.Dequeue();
        // }

        anim = GetComponent<Animator>();
        buttonPrompt = transform.Find("Parent transform/Button prompt").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Epic workaround for the fact that I didn't understand time back when I wrote this
        if (Time.deltaTime == 0)
            return;

        // Advance the current element if the current element isn't a line or the player presses the button at the end of a line
        // if (currentElement isn't LineElement) // darn (don't delete this is funny)
        if (!(currentElement is LineElement) || (Input.GetButtonDown("Submit") && revealedChars >= speech.Length))
        {
            buttonPrompt.SetActive(false);

            // Dequeue an element to serve as the current element
            if (elements.Count != 0)
            {
                revealedChars = 0;
                currentElement = elements.Dequeue();
            }
            // If there are no more elements, end the dialogue
            else
            {
                // End the dialogue
                anim.SetTrigger("Exit");
            }

            // Have the current element apply its changes to the dialogue canvas
            currentElement.ApplyChanges(this); 
        }
        else
        {
            // Reveal dat speech
            if (revealedChars < speech.Length)
            {
                // Determine whether to play the sound effect for the character's voice
                if (Convert.ToInt32(revealedChars + speed) > Convert.ToInt32(revealedChars))
                {
                    // "bork"
			        voice.Play();
                }

                // Reveal text incrementally
                revealedChars += speed * Time.deltaTime;
                
                // If the player presses the button while speec text is currently being revealed,
                // or revealedChars exceeds the length of the speech, go ahead and reveal the whole thing
                if (Input.GetButtonDown("Submit") || revealedChars > speech.Length)
                {
                    revealedChars = speech.Length;
                    buttonPrompt.SetActive(true);
                }

                // Set the displayed text
                displayedSpeech.text = speech.Substring(0, Convert.ToInt32(revealedChars));
            }
        }
    }

    void LoadElements()
    {
        elements = new Queue<IDialogueElement>();
        // Load the dialogue, Frosk!
        XmlDocument dialogueInfo = new XmlDocument();
        dialogueInfo.LoadXml(script.text);
        
        // The fun part - create CutsceneElement objects for each element contained in dialogueInfo
        foreach (XmlNode currentElement in dialogueInfo.FirstChild.ChildNodes)
        {
            // (this is funny)
            switch (currentElement.Name.ToString().ToUpper())
            {
                case "SPEAKER": elements.Enqueue(new SpeakerElement(currentElement)); break;
                case "NAME": elements.Enqueue(new NameElement(currentElement));		  break;
                // case "BUST": elements.Enqueue(new BustElement(currentElement));		  break;
                //case "SIDE": elements.Enqueue(new SideElement(currentElement));		  break;
                case "SPEED": elements.Enqueue(new SpeedElement(currentElement));	  break;
                case "VOICE": elements.Enqueue(new VoiceElement(currentElement));	  break;
                case "LINE": elements.Enqueue(new LineElement(currentElement));		  break;
                //case "STRING": elements.Enqueue(new StringElement(currentElement));	  break;
                //case "CLEAR": elements.Enqueue(new ClearElement(currentElement));     break;
                //case "CHOICE": elements.Enqueue(new ChoiceElement(currentElement));	  break;
                case "#COMMENT": break;

                default: throw new Exception("Sorry to inconvenience you but you probably misspelt something :(");
            }
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
