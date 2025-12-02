using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI DialogueText;
    public Button[] DialogueButtons = new Button[3];

    public Question question;
    
    
    void Start()
    {
        DisplayDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayDialogue()
    {
        speakerNameText.text = question.SpeakerName;
        DialogueText.text = question.questionText;
        for (int i = 0; i < DialogueButtons.Length; i++)
        {
          //  DialogueButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.answers[i];
        }
    }
}
