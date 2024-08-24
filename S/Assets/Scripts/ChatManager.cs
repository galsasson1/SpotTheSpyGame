using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// A list of all the messages sent between the guesser and the responder
    /// </summary>
    private List<Message> _messageList = new List<Message>();

    /// <summary>
    /// The UI element that houses the chat history
    /// </summary>
    [SerializeField] private GameObject _chatPanel;

    /// <summary>
    /// The parent object to the entire collapsable chat UI
    /// </summary>
    [SerializeField] private GameObject _chatPanelGraphics;

    /// <summary>
    /// The text message prefab
    /// </summary>
    [SerializeField] private GameObject _textObject;

    /// <summary>
    /// The input field for the player to write questions in
    /// </summary>
    [SerializeField] private TMP_InputField _inputField;

    /// <summary>
    /// The color for the player's text messages
    /// </summary>
    [SerializeField] private Color _playerColor;

    /// <summary>
    /// The color for the responder's text messages
    /// </summary>
    [SerializeField] private Color _aiColor;


    private CSVWriter csvWriter;

    private AIChatResponder ai;
    #endregion

    /// <summary>
    /// On initializing, assign the appropriate reference for the CSVWriter
    /// </summary>
    private void Start()
    {
        csvWriter = FindObjectOfType<CSVWriter>();
        ai = FindObjectOfType<AIChatResponder>();
        SendMessageToChat("Hey agent! what would you like to know? Remember, you can only ask me yes or no questions.", _aiColor);
    }

    /// <summary>
    /// Enables keyboard controls for the UI:
    /// Enter to send a message,
    /// Tab to toggle chat window visability.
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SendMessageFromInputField();
        }
    }

    /// <summary>
    /// If the input field is not empty, expand the chat UI, add the question to the CSV log and display the message on the chat window.
    /// Also clears the input field and starts the AI response.
    /// </summary>
    public void SendMessageFromInputField()
    {
        if(_inputField.text != "")
        {
            csvWriter.AddQuestion(_inputField.text);
            SendMessageToChat("Player: " + _inputField.text, _playerColor);
            ai.SendClicked(_inputField.text);
            _inputField.text = "";
        }
    }

    /// <summary>
    /// Wait 1.5f seconds, send a response to the chat logfrom the Responder ID, and add the response to the CSV log for the game
    /// </summary>
    /// <returns></returns>
    public void Respond(string answer)
    {
        SendMessageToChat("0R-C4L3: " + answer, _aiColor);
        csvWriter.AddAnswer(answer);
    }

    /// <summary>
    /// Expands the chat UI and sends a new response from the responder, either confirming or disproving the accusation.
    /// </summary>
    /// <param name="isRight"></param>
    public void AccusationResponse(bool isRight)
    {
        if (isRight)
        {
            SendMessageToChat("0R-C4L3: Yes! We got them!", _aiColor);
        }
        else
        {
            SendMessageToChat("0R-C4L3: That's not the target. keep looking!", _aiColor);
        }
    }

    /// <summary>
    /// Create a message from input text, and adds it to the list of all messages
    /// </summary>
    /// <param name="text"></param>
    /// <param name="textColor"></param>
    public void SendMessageToChat(string text, Color textColor)
    {
        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(_textObject, _chatPanel.transform);
        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = textColor;
        _messageList.Add(newMessage);
    }
}

/// <summary>
/// Data object for a chat message, containing both the raw text and a UI element to display it
/// </summary>
[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI textObject;
}
