using System;
using UnityEngine;


[ExecuteInEditMode]
[CreateAssetMenu(fileName = "Dialuage", menuName = "Scriptable Objects/Dialuage")]
public class Question : ScriptableObject
{
   public string SpeakerName;
  
   [TextArea]
   public string questionText;
   public Choice[] choices = new Choice[3];
   



   
}