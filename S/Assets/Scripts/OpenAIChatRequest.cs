using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIChatRequest : MonoBehaviour
{
    private const string OpenAIEndpoint = "https://api.openai.com/v1/chat/completions";
    private const string OpenAIModel = "gpt-4o-mini";
    private const string OpenAIAuthorizationHeader = ""; /// Insert your OpenAI API key here
    /// <summary>
    /// Exposed bool to act as a button in inspector
    /// </summary>
    [SerializeField] private bool debugSendRequest = false;

    private void Update()
    {
        if (debugSendRequest)
        {
            StartCoroutine(SendChatRequest());
            debugSendRequest = false;
        }
    }

    public IEnumerator SendChatRequest()
    {
        // Create the request payload
        var requestData = new
        {
            model = OpenAIModel,
            messages = new[]
            {
                new { role = "user", content = "Hello!" }
                // Add more messages here if needed
            }
        };

        string requestDataJson = JsonUtility.ToJson(requestData);

        // Create and send the web request
        var request = UnityWebRequest.Post(OpenAIEndpoint, requestDataJson);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", OpenAIAuthorizationHeader);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Request successful
            Debug.Log("Chat request successful");
            string responseJson = request.downloadHandler.text;
            // Process the response JSON as needed
            Debug.Log(responseJson);
        }
        else
        {
            // Request failed
            Debug.LogError("Chat request failed: " + request.error);
        }
    }
}
