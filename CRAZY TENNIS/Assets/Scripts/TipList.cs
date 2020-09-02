/*
Not technically a script but shh!
This file defines a component that stores a list of strings. 
This makes it possible to store each enemy's list of tips in the editor
(wow imagine using XML files haha nerd)
*/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TipList", menuName = "CRAZY TENNIS/TipList", order = 0)]
public class TipList : ScriptableObject {
    /// <summary> The list of strings that this object encapsulates </summary>
    ///     Is this overkill? Probably! Is it useful practice for a potential 
    ///     dialogue system? Probably!
    public List<string> Tips;
}