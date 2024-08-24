using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using System.Text;

public class CSVWriter : MonoBehaviour
{
    #region Data Objects
    /// <summary>
    /// CSV format ready data about questions
    /// </summary>
    [System.Serializable]
    public class QuestionData
    {
        public string question;
        public string answer;
        public int timeToAsk;
        public int guesses;
        public int cleared;
    }

    /// <summary>
    /// CSV friendly format for the PersonData of the guesses
    /// </summary>
    [System.Serializable]
    public class GuessData
    {
        public float positionX, positionY, positionZ;
        public List<string> properties = new List<string>();
        public int timeToGuess = 0;
    }

    /// <summary>
    /// CSV friendly format for the PersonData of the suspects
    /// </summary>
    [System.Serializable]
    public class SuspectsData
    {
        public float positionX, positionY, positionZ;
        public List<string> properties = new List<string>();
        public bool is_target;
    }
    #endregion

    #region Variables
    /// <summary>
    /// The file name for the CSV output
    /// </summary>
    private string _fileName = "";

    /// <summary>
    /// Unique IDs for both the player and the game instance.
    /// </summary>
    public string _playerID, _gameUID;

    ///<summary>
    /// The Version Of the Game
    ///</summary>
    public string _gameVersion = "1.7v";

    ///<summary>
    /// The prolific ID/ ID of participants in study
    /// </summary>
    public string _prolificID;

    ///<summary>
    /// The age of the player in years
    /// </summary>
    public int _age;

    ///<summary>
    /// The county of origin of the player
    /// </summary>
    public string _country;

    ///<summary>
    /// The amount of years of higher education of the player
    /// </summary>
    public int _yearsInHigherEducation;

    ///<summary>
    /// The sex of the player
    /// </summary>
    public string _sex;

    ///<summary>
    /// The creativity of the player as he considers himself
    /// </summary>
    public int _creativity;

    ///<summary>
    /// Did the player Win?
    /// </summary>
    public bool _won;

    ///<summary>
    /// The feedback written by the user at the end of the game
    /// </summary>
    public string _userFeedback;

    /// <summary>
    /// Timer tracking floats
    /// </summary>
    private float _lastLoggedQuestionTime = 0f, _lastLoggedGuessTime = 0f, _runningTimer = 0f;

    /// <summary>
    /// Copies of data from the last asked question, to compare to once the next question is asked
    /// </summary>
    private int _lastCleared, _guessCounter = 0, _currentQuestion = 0;

    /// <summary>
    /// Reference to the Game Manager
    /// </summary>
    private GameManager _gm;

    /// <summary>
    /// A list of all asked questions' related data
    /// </summary>
    public List<QuestionData> questionsList;

    /// <summary>
    /// A list of all guesses' related data
    /// </summary>
    public List<GuessData> guessesList;

    /// <summary>
    /// A list of all suspects' related data
    /// </summary>
    public List<SuspectsData> suspectsList;
    #endregion

    /// <summary>
    /// Initialize the lists, and create a gameUID and CSV file name containing the starting time of the game
    /// </summary>
    private void Start()
    {
        guessesList = new List<GuessData>();
        questionsList = new List<QuestionData>();
        suspectsList = new List<SuspectsData>();
        _gm = FindObjectOfType<GameManager>();
        _gameUID = System.DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
        _prolificID = UIManager.prolificID;
        _age = UIManager.age;
        _country = UIManager.country;
        _yearsInHigherEducation = UIManager.yearsInHigherEducation;
        _sex = UIManager.sex;
        _creativity = UIManager.creativity;
        _fileName = Application.dataPath + "/playData_" + _gameUID + ".csv";
        _playerID = CreateMD5Hash(SystemInfo.deviceName + SystemInfo.deviceModel + SystemInfo.deviceType + SystemInfo.graphicsDeviceType + SystemInfo.graphicsDeviceID);
        _userFeedback = "";
        StartCoroutine(LateStart());
    }

    /// <summary>
    /// Delayed commands at the start of game to insure valid data
    /// </summary>
    /// <returns></returns>
    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        _lastCleared = _gm.remainingSuspects;
    }

    /// <summary>
    /// Create a 5MD Hash from a string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string CreateMD5Hash(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Keeps a running timer running to create timestamps from
    /// </summary>
    private void Update()
    {
        _runningTimer += Time.deltaTime;
    }

    /// <summary>
    /// Convert a person data to a CSV parsable GuessData
    /// </summary>
    /// <param name="input"></param>
    public void AddGuess(PersonData input)
    {
        GuessData newData = new GuessData();
        newData.timeToGuess = (int)(_runningTimer - _lastLoggedGuessTime);
        _lastLoggedGuessTime = _runningTimer;
        newData.positionX = input.gameObject.transform.position.x;
        newData.positionY = input.gameObject.transform.position.y;
        newData.positionZ = input.gameObject.transform.position.z;

        //Manually add each Enum as a new string value
        newData.properties.Add(System.Enum.GetName(typeof(Gender), input._gender));
        newData.properties.Add(System.Enum.GetName(typeof(FaceAccessories), input._faceAccessoriesType));
        newData.properties.Add(System.Enum.GetName(typeof(HairLength), input._hairLength));
        newData.properties.Add(System.Enum.GetName(typeof(HairType), input._hairType));
        newData.properties.Add(System.Enum.GetName(typeof(PropertyColor), input._hairColor));
        newData.properties.Add(System.Enum.GetName(typeof(FacialHairType), input._facialHairType));
        newData.properties.Add(System.Enum.GetName(typeof(PropertyColor), input._facialHairColor));
        newData.properties.Add(System.Enum.GetName(typeof(ClothesType), input._clothesType));
        newData.properties.Add(System.Enum.GetName(typeof(PropertyColor), input._clothesColor));

        guessesList.Add(newData);
        _guessCounter++;

    }

    /// <summary>
    /// Convert a person data to a CSV parsable SuspectsData
    /// </summary>
    /// <param name="input"></param>
    public void AddSuspect(PersonData input, bool is_target)
    {
        SuspectsData newData = new SuspectsData();
        newData.positionX = input.gameObject.transform.position.x;
        newData.positionY = input.gameObject.transform.position.y;
        newData.positionZ = input.gameObject.transform.position.z;
        newData.is_target = is_target;

        //Manually add each Enum as a new string value
        newData.properties.Add(System.Enum.GetName(typeof(Gender), input._gender));
        newData.properties.Add(System.Enum.GetName(typeof(FaceAccessories), input._faceAccessoriesType));
        newData.properties.Add(System.Enum.GetName(typeof(HairLength), input._hairLength));
        newData.properties.Add(System.Enum.GetName(typeof(HairType), input._hairType));
        newData.properties.Add(System.Enum.GetName(typeof(PropertyColor), input._hairColor));
        newData.properties.Add(System.Enum.GetName(typeof(FacialHairType), input._facialHairType));
        newData.properties.Add(System.Enum.GetName(typeof(PropertyColor), input._facialHairColor));
        newData.properties.Add(System.Enum.GetName(typeof(ClothesType), input._clothesType));
        newData.properties.Add(System.Enum.GetName(typeof(PropertyColor), input._clothesColor));

        suspectsList.Add(newData);
    }

    /// <summary>
    /// Create a new question data set and fill it with the inputted question, the time since the last question, the number of guesses since the last question and the number of cleared suspects since the last question.
    /// </summary>
    /// <param name="input"></param>
    public void AddQuestion(string input)
    {
        QuestionData newData = new QuestionData();
        newData.question = input.Replace(',', ';');
        //Security measure, in case the ChatManager related coroutine fails
        newData.answer = "Corrupted Data";
        newData.timeToAsk = (int)(_runningTimer - _lastLoggedQuestionTime);
        _lastLoggedQuestionTime = _runningTimer;
        newData.guesses = _guessCounter;
        _guessCounter = 0;
        newData.cleared = _lastCleared - _gm.remainingSuspects;
        _lastCleared = _gm.remainingSuspects;
        questionsList.Add(newData);
    }

    /// <summary>
    /// Recieve the delayed answer from the responder and adds it to the last question data object.
    /// </summary>
    /// <param name="input"></param>
    public void AddAnswer(string input)
    {
        questionsList[_currentQuestion].answer = input.Replace(',',';');
        _currentQuestion++;
    }

    public void setWon(bool won)
    {
        _won = won;
    }

    public void setUserFeedback(string feedback)
    {
        _userFeedback = feedback;
    }

    //TODO: FIX JSON TO INPUT ALL DATA ACCORDINGLY
    public void WriteJson()
    {
        Debug.Log("Got Here!");
        string jsonData = JsonUtility.ToJson(this, true);
        string name = gameObject.name;
        FirebaseFirestore.GetDocumentsInCollection("Games", name, "DisplayJson", "DisplayErrorMessage");
        FirebaseFirestore.AddDocument("Games", JsonUtility.ToJson(this), name ,"DisplayJson", "DisplayErrorMessage");
    }

    private void DisplayJson(string output)
    {
        Debug.Log(output);
    }

    //TODO: CHANGE the var name "output" to "error"
    private void DisplayErrorMessage(string output)
    {
        Debug.LogWarning(output);
    }
}
