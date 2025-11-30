using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialuage", menuName = "Scriptable Objects/Dialuage")]
public class Question : ScriptableObject
{
   public string SpeakerName;
  
   [TextArea]
   public string questionText;
   public String[] answers = new String[3];
   



   
}