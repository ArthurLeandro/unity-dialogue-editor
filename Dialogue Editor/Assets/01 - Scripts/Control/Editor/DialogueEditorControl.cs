using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;

namespace DialogueEditor.Editor
{
    public class DialogueEditorControl : EditorWindow
    {
        Dialogue selectedDialogue = null;
        string testName = "No dialogue selected";

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow() 
        {
            GetWindow(typeof(DialogueEditor.Editor.DialogueEditorView),false,"Dialogue Editor View");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenDialogueAsset(int _instanceID,int _line) 
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(_instanceID) as Dialogue;
            bool response = (dialogue != null) ? true : false;
            if (response)
            {
                ShowEditorWindow();
            }
            return response;
        }



    }
}
