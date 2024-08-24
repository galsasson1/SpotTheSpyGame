using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Public Variables
    /// <summary>
    /// Defualt material for sprites
    /// </summary>
    public Material outlineMat;

    /// <summary>
    /// Striped, dark material to mark cleared people
    /// </summary>
    public Material blackedOutOutlineMat;

    /// <summary>
    /// The reference to the currently marked suspicious person
    /// </summary>
    public PersonInteractions currentSuspect;

    ///<summary>
    /// General Suspect Prefab
    ///</summary>
    public GameObject generalSuspectPrefab;
    /// <summary>
    /// The ID number of the target for this round
    /// </summary>
    public int targetID;

    ///<summary>
    /// The number of suspects
    /// </summary>
    public int suspectsAmount;

    /// <summary>
    /// The number of not cleared suspects currently on the map
    /// </summary>
    public int remainingSuspects;

    ///<summary>
    ///An array of suspect's possible locations on the map
    ///</summary> 
    //TODO: CHANGE TO AN ENUM OR SOMETHING MORE READABLE IN FIREBASE THAN XYZ CORDS
    public Transform[] possibleSuspectsLocations;

    ///<summary>
    ///An array of suspect's locations on the map
    ///</summary> 
    //TODO: CHANGE TO AN ENUM OR SOMETHING MORE READABLE IN FIREBASE THAN XYZ CORDS
    public Transform[] suspectsLocations;

    /// <summary>
    /// An array of the data of all the people currently on the map
    /// </summary>
    public PersonData[] suspects;

    /// <summary>
    /// The button to attempt a guess on a suspect
    /// </summary>
    public Button guessButton;

    /// <summary>
    /// The amount of Guesses avialible
    /// </summary>
    public int maxGuesses;

    /// <summary>
    /// The amount of Guesses left
    /// </summary>
    private int remainingGuesses;

    /// <summary>
    /// The amount of Guesses left Text box
    /// </summary>
    public TMP_Text remainingGuessesText;

    /// <summary>
    /// UI text for the number of not cleared people currently on the map
    /// </summary>
    public TMP_Text remainingSuspectsText;

    /// <summary>
    /// The image component of the "Guess" button
    /// </summary>
    private Image guessButtonImage;

    /// <summary>
    /// The sprite of the deactive (gray) "Guess" button
    /// </summary>
    public Sprite guessButtonDeactiveSprite;

    /// <summary>
    /// The sprite of the active (red) "Guess" button
    /// </summary>
    public Sprite guessButtonActiveSprite;

    /// <summary>
    /// The win screen
    /// </summary>
    public GameObject winScreenPanel; //TODO: check if this can be saved as a panel variable

    /// <summary>
    /// The win screen animator
    /// </summary>
    public Animator winScreenAnimator;

    /// <summary>
    /// The lose screen
    /// </summary>
    public GameObject loseScreenPanel; //TODO: check if this can be saved as a panel variable

    /// <summary>
    /// The win screen animator
    /// </summary>
    public Animator loseScreenAnimator;

    /// <summary>
    /// The feedback screen
    /// </summary>
    public GameObject feedbackScreenPanel; //TODO: check if this can be saved as a panel variable

    /// <summary>
    /// Reference to the chat manager
    /// </summary>
    public ChatManager cm;

    /// <summary>
    /// Reference to the CSVWriter
    /// </summary>
    public CSVWriter csvWriter;

    /// <summary>
    /// The AI response script that uses chat GPT 
    /// </summary>
    public AIChatResponder ai;

    /// <summary>
    /// The list of basic facts/rules for the AI to follow
    /// </summary>
    public List<string> aiBaseFacts;

    ///<summary>
    /// The Eliminating Player sfx
    ///</summary>
    [SerializeField] public AudioClip eliminatePlayerClickSound;

    ///<summary>
    /// The Hovering Player sfx
    ///</summary>
    [SerializeField] public AudioClip targetingAnotherPlayer;

    ///<summary>
    /// The Wrong Guess sfx
    ///</summary>
    [SerializeField] public AudioClip wrongGuessSound;

    ///<summary>
    /// The 'You Won' sfx
    ///</summary>
    [SerializeField] public AudioClip youWonSound;

    ///<summary>
    /// The 'You Lost' sfx
    ///</summary>
    [SerializeField] public AudioClip youLostSound;

    [SerializeField] private TMP_InputField feedbackField;

    //The audio source
    public AudioSource mainAudioSource;

    private BGMManagerScript bgm;

    private int SecondsToDelay = 5;
    public static string UserFeedback;
    public Button NextButtonEndGame;
    public InputField prolificCodeField;
    #endregion

    /// <summary>
    /// Create an array of the data of all the people on the map, and set the UI suspect list to match its length.
    /// </summary>
    private void Start()
    {
        cm = FindObjectOfType<ChatManager>();
        csvWriter = FindObjectOfType<CSVWriter>();
        ai = FindObjectOfType<AIChatResponder>();
        bgm = FindObjectOfType<BGMManagerScript>();
        suspects = new PersonData[suspectsAmount];
        suspectsLocations = new Transform[suspectsAmount];
        remainingSuspects = suspectsAmount;
        remainingSuspectsText.text = "Suspects Left: " + remainingSuspects.ToString("00") + "/" + suspectsAmount.ToString("00");
        remainingGuesses = maxGuesses;
        remainingGuessesText.text = "Guess Attempts Left: " + remainingGuesses + "/" + maxGuesses;
        guessButtonImage = guessButton.GetComponent<Image>();
        ToggleGuessButton();
        InitializeGame();
    }

    /// <summary>
    /// Randomly chooses a person in the scene to be the target, and adds his data to the CSV
    /// </summary>
    private void InitializeGame()
    {
        List<Transform> possibleLocationsLeft = possibleSuspectsLocations.ToList();
        for (int i = 0; i < suspectsAmount; i++)
        {
            int index = Random.Range(0, possibleLocationsLeft.Count);
            suspects[i] = Instantiate(generalSuspectPrefab, possibleLocationsLeft[index]).GetComponent<PersonData>();
            suspectsLocations[i] = possibleLocationsLeft[index];
            possibleLocationsLeft.RemoveAt(index);
            suspects[i].personID = i;
        }
        targetID = Random.Range(1, remainingSuspects);

        foreach (PersonData person in suspects)
        {
            csvWriter.AddSuspect(person, person.personID == targetID);
        }

        foreach (PersonData person in suspects)
        {
            if(person.personID == targetID)
            {
                csvWriter.AddGuess(person);
                SetGameFactsToAI(person);
                return;
            }
        }
    }

    private void SetGameFactsToAI(PersonData target)
    {
        #region Parse PersonData to strings
        //Describe gender
        aiBaseFacts.Add("The target's gender is " + System.Enum.GetName(typeof(Gender), target._gender));

        //Describe face accessory
        string faceAccessory = System.Enum.GetName(typeof(FaceAccessories), target._faceAccessoriesType);
        if(faceAccessory == "NONE")
        {
            aiBaseFacts.Add("The target is not wearing an accessory");
        }
        else
        {
            aiBaseFacts.Add("The target is wearing a " + faceAccessory);
            aiBaseFacts.Add("The target's " + faceAccessory + " color is " + System.Enum.GetName(typeof(PropertyColor), target._faceAccessoriesColor));
        }

        //Describe hair related properties
        string hairLength = System.Enum.GetName(typeof(HairLength), target._hairLength);
        if (hairLength == "BALD")
        {
            aiBaseFacts.Add("The target is bald");
        }
        if (hairLength == "BALDING")
        {
            aiBaseFacts.Add("The target has " + hairLength + " hair");
            aiBaseFacts.Add("The target's hair color is " + System.Enum.GetName(typeof(PropertyColor), target._hairColor));
        }
        else
        {
            aiBaseFacts.Add("The target has " + hairLength + " hair");
            aiBaseFacts.Add("The target has " + System.Enum.GetName(typeof(HairType), target._hairType) + " hair");
            aiBaseFacts.Add("The target's hair color is " + System.Enum.GetName(typeof(PropertyColor), target._hairColor));
        }

        //Describe facial hair
        string facialHairType = System.Enum.GetName(typeof(FacialHairType), target._facialHairType);
        if (facialHairType == "NONE")
        {
            aiBaseFacts.Add("The target does not have facial hair");
        }
        else
        {
            aiBaseFacts.Add("The target has " + facialHairType);
            aiBaseFacts.Add("The target's " + facialHairType + " color is " + System.Enum.GetName(typeof(PropertyColor), target._facialHairColor));
        }

        //Describe Clothes
        aiBaseFacts.Add("The target is wearing a " + System.Enum.GetName(typeof(ClothesType), target._clothesType));
        aiBaseFacts.Add("The target's clothes are colored " + System.Enum.GetName(typeof(PropertyColor), target._clothesColor));
        #endregion
        ai.Facts = aiBaseFacts;
    }

    /// <summary>
    /// If there is a selected suspect, enable the guess button and set the button text opacity to full.
    /// Otherwise, disable the button and the text opacity to 50%.
    /// </summary>
    public void ToggleGuessButton()
    {
        if(currentSuspect == null)
        {
            guessButton.interactable = false;
            guessButtonImage.sprite = guessButtonDeactiveSprite;
        }
        else if(!guessButton.IsInteractable())
        {
            guessButton.interactable = true;
            guessButtonImage.sprite = guessButtonActiveSprite;
        }
    }

    /// <summary>
    /// Sends the guess data to the CSV, and then checks if the suspect is actually the target. Sends an appropriate text message, and if not the target, disables the accused from being selected again.
    /// </summary>
    public void MakeGuess()
    {
        PersonData suspect = currentSuspect.gameObject.GetComponent<PersonData>();
        csvWriter.AddGuess(suspect);
        bool isRight = suspect.personID == targetID;
        remainingGuesses--;
        remainingGuessesText.text = "Guess Attempts Left: " + remainingGuesses + "/" + maxGuesses;
        cm.AccusationResponse(isRight);
        if (!isRight)
        {
            csvWriter.setWon(false);
            currentSuspect.gameObject.GetComponent<Collider>().enabled = false;
            currentSuspect.MarkCleared();
            if(remainingGuesses == 0)
            {
                PlayYouLoseSound();
                bgm.TurnOffBGM();
                loseScreenPanel.SetActive(true);
                loseScreenAnimator.SetTrigger("Lost");
                //Invoke("SwitchScreenToFeedback", SecondsToDelay);
                //NextButtonEndGame.onClick.AddListener(() => EndGame(false));
                EndGame(false);

            }
            else
                PlayWrongGuessSound();

        }
        else
        {
            PlayYouWinSound();
            bgm.TurnOffBGM();
            winScreenPanel.SetActive(true);
            winScreenAnimator.SetTrigger("Won");
            //Invoke("SwitchScreenToFeedback", SecondsToDelay);
            //NextButtonEndGame.onClick.AddListener(() => EndGame(true));
            EndGame(true);
        }
    }

    /// <summary>
    /// Update the suspect tally on the UI
    /// </summary>
    /// <param name="input"></param>
    public void UpdateSuspectsTotal(int input)
    {
        remainingSuspects += input;
        remainingSuspectsText.text = "Suspects Left: " + remainingSuspects.ToString("00") + "/" + suspectsAmount.ToString("00");
    }

    //Plays the player eliminated sound
    public void PlayEliminatePlayerSound()
    {
        if (BGMManagerScript.isVolumeEnabled)
            mainAudioSource.PlayOneShot(eliminatePlayerClickSound);
    }

    //Plays when you target another player
    public void PlayTargetOtherPlayerSound()
    {
        if (BGMManagerScript.isVolumeEnabled)
            mainAudioSource.PlayOneShot(targetingAnotherPlayer);
    }

    //Plays the Wrong Guess Sound
    public void PlayWrongGuessSound()
    {
        if (BGMManagerScript.isVolumeEnabled)
            mainAudioSource.PlayOneShot(wrongGuessSound);
    }

    //Plays the 'You Win' sound
    public void PlayYouWinSound()
    {
        if (BGMManagerScript.isVolumeEnabled)
            mainAudioSource.PlayOneShot(youWonSound);
    }

    //Plays the 'You Lose' sound
    public void PlayYouLoseSound()
    {
        if (BGMManagerScript.isVolumeEnabled)
            mainAudioSource.PlayOneShot(youLostSound);
    }

    // Move to feedback screen
    private void SwitchScreenToFeedback()
    { 
        loseScreenPanel.SetActive(false);
        winScreenPanel.SetActive(false);
        feedbackScreenPanel.SetActive(true);
    }

    private void EndGame(bool win)
    {
        UserFeedback = (feedbackField.text != "") ? feedbackField.text : "Didnt Enter";
        csvWriter.setWon(win);
        csvWriter.setUserFeedback(UserFeedback);
        csvWriter.WriteJson();
    }

    public void CopyProlificCodeToClipboard()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = prolificCodeField.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }

}
