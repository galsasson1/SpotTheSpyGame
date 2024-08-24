using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BitSplash.AI.GPT;

public class AIChatResponder : MonoBehaviour
{
    public ChatManager cm;

    public string NpcDirection = "Answer as a secret agent";
    public List<string> Facts;
    public bool TrackConversation = false;
    public int MaximumTokens = 600;
    [Range(0f, 1f)]
    public float Temperature = 0f;
    ChatGPTConversation Conversation;

    void Start()
    {
        SetUpConversation();
        cm = FindObjectOfType<ChatManager>();
    }
    void SetUpConversation()
    {
        Conversation = ChatGPTConversation.Start(this)
            .MaximumLength(MaximumTokens)
            .SaveHistory(TrackConversation)
            .System(string.Join("\n", Facts) + "\n" + NpcDirection);
        Conversation.Temperature = Temperature;
    }
    public void SendClicked(string question)
    {
        Conversation.Say(question);
    }

    void OnConversationResponse(string answer)
    {
        cm.Respond(answer);
    }
    void OnConversationError(string text)
    {
        Debug.Log("Error : " + text);
        Conversation.RestartConversation();
        cm.Respond("Sorry agent, I got confused for a moment, what did you just ask?");
    }

    private void OnValidate()
    {
        SetUpConversation();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
