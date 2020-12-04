using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour
{
    PuzzlePiece[,] pieces = new PuzzlePiece[8,3];
    List<PuzzlePiece> piecePool = new List<PuzzlePiece>();
    float timer = 0f;
    float maxTimer = 0.3f;
    bool transition = true;
    bool reverseTransition = false;
    string sceneTarget;
    public GameObject byText;
    public GameObject forText;

    
    // Start is called before the first frame update
    void Start()
    {
        string text = "puzzlingremarks";
        for (int i = 0; i < 16; i++) {
            PuzzlePiece piece = transform.GetChild(i).GetComponent<PuzzlePiece>();
            int x = i%8;
            int y = i/8;
            piecePool.Add(piece);
            string l = i < text.Length ? text.Substring(i,1) : " ";
            int left = x == 0 ? 2 : PuzzleManager.GetOpposite(pieces[x-1,y].right.mode);
            int right = x == 7 ? 2 : Random.Range(0,2);
            int top = y == 0 ? 2 : PuzzleManager.GetOpposite(pieces[x,y-1].bottom.mode);
            int bottom = y == 1 ? 2 : Random.Range(0,2);
            piece.SetLetter(l,left,right,top,bottom);
            pieces[x,y] = piece;
            piece.SetAlpha(MenuTransitionInformation.instant ? 1f : 0f);
        }
        transition = !MenuTransitionInformation.instant;
        byText.SetActive(false);
        forText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!transition && !reverseTransition) return;
        timer += Time.deltaTime;
        if (transition && timer >= maxTimer) {
            timer = 0f;
            int i = Random.Range(0,piecePool.Count);
            PuzzlePiece p = piecePool[i];
            piecePool.RemoveAt(i);
            p.SetAlpha(1f);
            if (piecePool.Count == 0) {
                transition = false;
                byText.SetActive(true);
                forText.SetActive(true);
            }
        }
        else if (reverseTransition && timer >= maxTimer) {
            timer = 0f;
            int i = Random.Range(0,piecePool.Count);
            PuzzlePiece p = piecePool[i];
            piecePool.RemoveAt(i);
            p.SetAlpha(0f);
            if (piecePool.Count == 0) {
                SceneManager.LoadScene(sceneTarget);
            }
        }
    }

    public void ClickedPlay()
    {
        if (transition || reverseTransition) return;
        TransitionToScene("GameScene");
    }

    void TransitionToScene(string sceneName)
    {
        sceneTarget = sceneName;
        reverseTransition = true;
        byText.SetActive(false);
        forText.SetActive(false);
        maxTimer = 0.1f; // make fade out faster
        timer = 0f;
        piecePool.Clear();
        for (int i = 0; i < 16; i++) piecePool.Add(transform.GetChild(i).GetComponent<PuzzlePiece>());
    }

    public void ClickedQuit()
    {
        if (transition) return;
        Application.Quit();
    }
}
