using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public PuzzlePiece movingPiece;
    public TextAsset riddlesJson;
    public TextMeshProUGUI riddleText;
    public ProgressBar progressBar;
    List<Riddle> riddles;
    int index = 0;
    public GameObject transitionScreen;
    bool fadingInRiddleText = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // transitionScreen.SetActive(true);
        riddles = Helper.ReadJsonArray<Riddle>(riddlesJson.ToString());
        index = PlayerPrefs.GetInt("riddleIndex",0);
        progressBar.ForceSetPlace(((float)index) / riddles.Count);
        SetupRiddle(true);
    }

    void SetupRiddle(bool firstRiddle)
    {
        riddleText.text = riddles[index].riddle; // TODO write it out animation maybe?
        puzzleManager.SetupWord(riddles[index].answer,firstRiddle);
    }

    void Update()
    {
        // skip riddles with space for debugging etc
        if (Input.GetKeyDown(KeyCode.Space)) CorrectlyGuessedRiddle();
        if (fadingInRiddleText) {
            Color c = riddleText.color;
            riddleText.color = new Color(c.r,c.g,c.b,Mathf.Min(c.a + Time.deltaTime * puzzleManager.riddleFadeSpeed,1f));
            if (riddleText.color.a == 1f) fadingInRiddleText = false;
        }
    }

    public void CorrectlyGuessedRiddle()
    {
        index++;
        PlayerPrefs.SetInt("riddleIndex",index);
        progressBar.SetTarget(((float)index) / riddles.Count);
        if (index < riddles.Count) SetupRiddle(false);
        else {
            ShowFinalScreen();
            PlayerPrefs.SetInt("riddleIndex",0);
        }
    }

    void ShowFinalScreen()
    {
        riddleText.text = "Congratulations! You've solved all there is to solve, puzzled through all there is to puzzle.\n\nWhat now?";
        fadingInRiddleText = true;
        puzzleManager.GameFinished();
    }

    public void ClickedQuit()
    {
        Application.Quit();
    }
}
