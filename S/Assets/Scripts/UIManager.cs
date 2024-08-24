using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static bool isPaused = false;
    [SerializeField]
    string gameScene;
    [SerializeField]
    string mainMenuScene;
    [SerializeField]
    GameObject panel;
    [SerializeField]
    private TMP_InputField prolificIDField;
    [SerializeField]
    private TMP_InputField ageField;
    [SerializeField]
    private TMP_InputField countryField;
    [SerializeField]
    private TMP_InputField yearsInHigherEducationField;
    [SerializeField]
    private TMP_InputField sexField;
    [SerializeField]
    private TMP_InputField creativityField;
    [SerializeField]
    private TMP_InputField feedbackField;

    public static string prolificID;
    public static int age;
    public static string country;
    public static int yearsInHigherEducation;
    public static string sex;
    public static int creativity;
    public static string feedback;

    public GameObject MainMenuScreen;
    public GameObject OpeningScreen;

    public void StartGame()
    {
        if(ageField != null)
        {
            prolificID = (prolificIDField.text != "") ? prolificIDField.text : "Didnt Enter";
            age = (ageField.text != "") ? int.Parse(ageField.text) : -1;
            country = (countryField.text != "") ? countryField.text : "Prefer Not to say";
            yearsInHigherEducation = (yearsInHigherEducationField.text != "") ? int.Parse(yearsInHigherEducationField.text) : -1;
            sex = (sexField.text != "") ? sexField.text : "Prefer Not to say";
            creativity = (creativityField.text != "") ? int.Parse(creativityField.text) : -1;
        }
        SceneManager.LoadScene(gameScene);
    }

    public void StartMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        if (OpeningScreen != null) OpeningScreen.SetActive(false);
        if (MainMenuScreen != null) MainMenuScreen.SetActive(true);
    }

    public void SetIsPaused(bool isPaused)
    {
        UIManager.isPaused = isPaused;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panel.activeSelf)
            {
                isPaused = false;
                panel.SetActive(false);
            }
            else
            {
                isPaused = true;
                panel.SetActive(true);
            }
        }
    }
}
