using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public GameManager gameManager;
    public Transform targetHolder;
    public Transform pieceHolder;
    public GameObject piecePrefab;
    public GameObject targetPrefab;
    string targetWord = "";
    List<Image> targetPieces = new List<Image>();
    PuzzlePiece[] placedPieces;

    Color hideTargetColor = new Color(0f,0f,0f,0f);
    Color baseTargetColor = new Color(0f,0f,0f,0.1f);
    Color hoverTargetColor = new Color(0.5f,1f,0.5f,0.3f);
    Color invalidHoverTargetColor = new Color(1f,0.5f,0.5f,0.3f);
    int hoveringOver = -1;
    string alphabet = "abcdefghijklmnopqrstuvwxyz";
    public bool blockPuzzleInteraction = false;
    int transitionStage = 0; // 0 nothing, 1 fadeout pieces, 2 pause, 3 fade out riddle+correct, 4 setup new level
    Timer placedPieceTimer = new Timer(0.4f);
    int successIndex = 0; // index of letter to highlight with success color
    Color successColor = new Color(0.4f,1f,0.4f);
    Timer basePieceTimer = new Timer(0.2f);
    Timer pauseTimer = new Timer(1f);
    public float riddleFadeSpeed = 0.6f;
    List<int> placedIndices = new List<int>();
    List<int> hiddenPieceIndices = new List<int>();
    
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < 20; i++) {
            GameObject go = Instantiate(targetPrefab,Vector3.zero,Quaternion.identity,targetHolder);
            targetPieces.Add(go.GetComponent<Image>());
            go.SetActive(false);
        }
        // begin initial transition
        Color c = gameManager.riddleText.color;
        gameManager.riddleText.color = new Color(c.r,c.g,c.b,0f);
    }

    void Update()
    {
        // greening out correct answer, destroying leftover pieces
        if (transitionStage == 1) {
            if (successIndex < placedPieces.Length) placedPieceTimer.Increment(Time.deltaTime);
            if (placedPieceTimer.IsReached()) {
                placedPieceTimer.Reset();
                while (placedPieces[successIndex] == null) successIndex++;
                placedPieces[successIndex].SetColor(successColor);
                successIndex++;
            }
            if (pieceHolder.childCount > 0) basePieceTimer.Increment(Time.deltaTime);
            if (basePieceTimer.IsReached()) {
                basePieceTimer.Reset();
                int i = Random.Range(0,pieceHolder.childCount);
                Destroy(pieceHolder.GetChild(i).gameObject);
            }
            if (successIndex >= placedPieces.Length && pieceHolder.childCount == 0) {
                transitionStage = 2;
                pauseTimer.Reset();
                
            }
        }
        // pause
        else if (transitionStage == 2) {
            pauseTimer.Increment(Time.deltaTime);
            if (pauseTimer.IsReached()) {
                pauseTimer.Reset();
                transitionStage = 3;
                placedIndices.Clear();
                for (int i = 0; i < placedPieces.Length; i++) if (placedPieces[i] != null) {
                    placedIndices.Add(i);
                    targetPieces[i].color = hideTargetColor;
                }
            }
        }
        // destroying correct pieces, fading out riddle text
        else if (transitionStage == 3) {
            if (placedIndices.Count > 0) {
                placedPieceTimer.Increment(Time.deltaTime);
                if (placedPieceTimer.IsReached()) {
                    placedPieceTimer.Reset();
                    int j = placedIndices[Random.Range(0,placedIndices.Count)];
                    Destroy(placedPieces[j].gameObject);
                    placedPieces[j] = null;
                    placedIndices.Remove(j);
                }
            }
            if (gameManager.riddleText.color.a > 0f) {
                Color c = gameManager.riddleText.color;
                gameManager.riddleText.color = new Color(c.r,c.g,c.b,Mathf.Max(c.a - Time.deltaTime * riddleFadeSpeed,0f));
            }
            if (placedIndices.Count == 0 && gameManager.riddleText.color.a == 0f) {
                hiddenPieceIndices.Clear();
                gameManager.CorrectlyGuessedRiddle();
                for (int i = 0; i < GetTotalPieceCount(); i++) hiddenPieceIndices.Add(i);
            }
        }
        // fading in riddle text, revealing pieces
        else if (transitionStage == 4) {
            if (gameManager.riddleText.color.a < 1f) {
                Color c = gameManager.riddleText.color;
                gameManager.riddleText.color = new Color(c.r,c.g,c.b,Mathf.Min(c.a + Time.deltaTime * riddleFadeSpeed,1f));
            }
            if (hiddenPieceIndices.Count > 0) {
                basePieceTimer.Increment(Time.deltaTime);
                if (basePieceTimer.IsReached()) {
                    basePieceTimer.Reset();
                    int i = Random.Range(0,hiddenPieceIndices.Count);
                    pieceHolder.GetChild(hiddenPieceIndices[i]).GetComponent<PuzzlePiece>().SetAlpha(1f);
                    hiddenPieceIndices.RemoveAt(i);
                }
            }

            if (hiddenPieceIndices.Count == 0 && gameManager.riddleText.color.a == 1f) {
                // show target word container
                for (int i = 0; i < targetWord.Length; i++) targetPieces[i].color = targetWord[i] == ' ' ? hideTargetColor : baseTargetColor;
                // foreach (Image target in targetPieces) target.color = baseTargetColor;
                // TODO enable input
                blockPuzzleInteraction = false;
            }
        }
        if (blockPuzzleInteraction) return;
        if (gameManager.movingPiece != null) {
            int closestIndex = -1;
            float closestDist = 100f;
            for (int i = 0; i < targetWord.Length; i++) {
                float d = Vector3.Distance(targetPieces[i].transform.position,Input.mousePosition);
                if (d < closestDist) {
                    closestDist = d;
                    closestIndex = i;
                }
            }
            if (closestIndex != hoveringOver && hoveringOver != -1) targetPieces[hoveringOver].color = baseTargetColor;
            if (closestIndex != -1 && targetWord[closestIndex] != ' ') {
                targetPieces[closestIndex].color = CanPlacePiece(gameManager.movingPiece,closestIndex) ? hoverTargetColor : invalidHoverTargetColor;
            }
            hoveringOver = closestIndex;
        }
    }

    public void SetupWord(string word, bool firstRiddle)
    {
        transitionStage = 4;
        targetWord = word;
        // clean up previous placed pieces
        foreach (Transform piece in pieceHolder) Destroy(piece.gameObject);
        // place new puzzle pieces
        int lastRight = Random.Range(0,2);
        SpawnPiece(word.Substring(0,1),2,lastRight);
        for (int i = 1; i < word.Length - 1; i++) {
            if (word[i] != ' ') {
                int newRight = Random.Range(0,2);
                if (i + 1 < word.Length && word[i+1] == ' ') newRight = 2;
                SpawnPiece(word.Substring(i,1),lastRight == 2 ? 2 : (lastRight == 0 ? 1 : 0),newRight);
                lastRight = newRight;
            }
            else {
                lastRight = 2;
            }
        }
        SpawnPiece(word.Substring(word.Length-1),lastRight == 0 ? 1 : 0,2);
        // spawn a couple start pieces
        for (int i = 0; i < 3; i++) {
            SpawnPiece(alphabet.Substring(Random.Range(0,alphabet.Length),1),2,Random.Range(0,2));
        }
        // spawn a couple end pieces
        for (int i = 0; i < 3; i++) {
            SpawnPiece(alphabet.Substring(Random.Range(0,alphabet.Length),1),Random.Range(0,2),2);
        }
        // spawn a couple random inner pieces
        for (int i = 0; i < GetRandomInnerCount(); i++) {
            SpawnPiece(alphabet.Substring(Random.Range(0,alphabet.Length),1),Random.Range(0,2),Random.Range(0,2));
        }

        for (int i = 0; i < targetHolder.childCount; i++) {
            GameObject go = targetHolder.GetChild(i).gameObject;
            if (i >= word.Length) go.SetActive(false);
            else {
                // setup look etc
                go.SetActive(true);
                targetPieces[i].color = hideTargetColor;
            }
        }
        placedPieces = new PuzzlePiece[word.Length];
        if (firstRiddle) {
            hiddenPieceIndices.Clear();
            for (int i = 0; i < GetTotalPieceCount(); i++) hiddenPieceIndices.Add(i);
            transitionStage = 4;
            blockPuzzleInteraction = true;
        }
    }

    int GetRandomInnerCount()
    {
        return targetWord.Length;
    }

    int GetTotalPieceCount()
    {
        return pieceHolder.childCount;
    }

    void SpawnPiece(string letter, int leftMode, int rightMode)
    {
        // TODO check position not right on top of already existing piece: some minDist
        Vector3 pos = new Vector3(Screen.width * Random.Range(0.2f,0.8f),Screen.height * Random.Range(0.15f,0.35f),1f);
        const float minDist = 50f;
        int retryAttempts = 0;
        while (retryAttempts  < 20) {
            bool valid = true;
            foreach (Transform piece in pieceHolder) {
                float d = Vector3.Distance(piece.position,pos);
                if (d < minDist) {
                    valid = false;
                    retryAttempts++;
                    break;
                }
            }
            if (valid) break;
            pos = new Vector3(Screen.width * Random.Range(0.2f,0.8f),Screen.height * Random.Range(0.15f,0.35f),1f);
        }
        GameObject go = Instantiate(piecePrefab,pos,Quaternion.identity,pieceHolder);
        PuzzlePiece pp = go.GetComponent<PuzzlePiece>();
        pp.SetLetter(letter,leftMode,rightMode);
    }

    public void PickedUpPiece(PuzzlePiece piece)
    {
        if (piece.placedIndex != -1) placedPieces[piece.placedIndex] = null;
        gameManager.movingPiece = piece;
    }

    public void LetGoOfPiece(PuzzlePiece piece)
    {
        if (hoveringOver != -1 && CanPlacePiece(piece,hoveringOver)) {
            piece.transform.position = targetPieces[hoveringOver].transform.position;
            placedPieces[hoveringOver] = piece;
            targetPieces[hoveringOver].color = baseTargetColor;
            piece.placedIndex = hoveringOver;
            hoveringOver = -1;
            CheckWord();
        }
        else if (hoveringOver != -1) {
            // TODO maybe put it back somewhere?
            piece.transform.position = new Vector3(Screen.width / 2f, Screen.height * 0.2f,0f);
            piece.placedIndex = -1;
            targetPieces[hoveringOver].color = baseTargetColor;
            hoveringOver = -1;
        }
        gameManager.movingPiece = null;
    }

    bool CanPlacePiece(PuzzlePiece piece, int index)
    {
        if (placedPieces[index] != null) return false;
        if (index > 0 && placedPieces[index-1] != null && !IsPuzzleOpposite(piece.left.mode,placedPieces[index-1].right.mode)) return false;
        else if (index == 0 && piece.left.mode != 2) return false;
        if (index + 1 < placedPieces.Length && placedPieces[index+1] != null && !IsPuzzleOpposite(piece.right.mode,placedPieces[index+1].left.mode)) return false;
        else if (index == placedPieces.Length-1 && piece.right.mode != 2) return false;
        return true;
    }

    // returns true if given a pair of outer/inner
    public static bool IsPuzzleOpposite(int mode1, int mode2)
    {
        return (mode1 == 0 &&  mode2 == 1) || (mode1 == 1 && mode2 == 0);
    }

    // returns opposite (0 if 1, 1 if 0)
    public static int GetOpposite(int mode)
    {
        return mode == 0 ? 1 : 0;
    }

    void CheckWord()
    {
        Debug.Log("checking word");
        string word = "";
        for (int i = 0; i < placedPieces.Length; i++) {
            if (placedPieces[i] == null && targetWord[i] != ' ') return; // incomplete word, nothing to check
            if (placedPieces[i] != null) word += placedPieces[i].GetLetter();
            else word += ' ';
        }
        Debug.Log($"word is {word}, target is {targetWord}");
        if (word.ToLower().Equals(targetWord.ToLower())) {
            // TODO guessed word, transition
            // gameManager.CorrectlyGuessedRiddle();
            BeginSuccessTransition();
        }
        else {
            Debug.Log("not equal");
            // TODO somehow indicate this is not right
            // TODO maybe blink red all placed pieces? (take away input control, blink, give back control)
        }
    }

    void BeginSuccessTransition()
    {
        blockPuzzleInteraction = true;
        placedPieceTimer.Reset();
        basePieceTimer.Reset();
        pauseTimer.Reset();
        transitionStage = 1;
        successIndex = 0;
        foreach (PuzzlePiece piece in placedPieces) if (piece != null) piece.transform.parent = piece.transform.parent.parent;
    }

    public void GameFinished()
    {
        transitionStage = 10;
    }
}
