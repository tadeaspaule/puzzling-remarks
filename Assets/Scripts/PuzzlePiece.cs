using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzlePiece : MonoBehaviour
{
    bool beingMoved = false;
    PuzzleManager puzzleManager;
    public TextMeshProUGUI pieceLetter;
    public int placedIndex = -1;
    public Color forceStayColor;
    
    [System.Serializable]
    public class PuzzleQuarter
    {
        public GameObject outerPart;
        public GameObject hidePart;
        public int mode;

        public void Set(int mode)
        {
            this.mode = mode;
            outerPart.SetActive(mode == 0);
            hidePart.SetActive(mode != 1);
        }

        public void SetAlpha(Color color, float alpha)
        {
            outerPart.GetComponent<Image>().color = new Color(color.r,color.g,color.b,alpha);
            hidePart.GetComponent<Image>().color = new Color(color.r,color.g,color.b,alpha);
        }
    }
    public Image mainImage;
    public Image extraImage;
    public PuzzleQuarter left;
    public PuzzleQuarter right;
    public PuzzleQuarter top;
    public PuzzleQuarter bottom;
    public float initAlpha = 0f;
    Color color;

    void Awake()
    {
        color = new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f),initAlpha);
        if (forceStayColor.g != 0f) color = forceStayColor;
        foreach (Transform child in transform) {
            Image i = child.GetComponent<Image>();
            if (i && i != extraImage) i.color = color;
        }
        mainImage.color = color;
        pieceLetter.color = new Color(pieceLetter.color.r,pieceLetter.color.g,pieceLetter.color.b,initAlpha);
        puzzleManager = FindObjectOfType<PuzzleManager>();
    }

    void Update()
    {
        if (beingMoved) transform.position = Input.mousePosition;
    }
    
    public void PieceOnMouseDown()
    {
        if (puzzleManager.blockPuzzleInteraction) return;
        transform.SetAsLastSibling();
        beingMoved = true;
        puzzleManager.PickedUpPiece(this);
    }
    
    public void PieceOnMouseUp()
    {
        if (puzzleManager.blockPuzzleInteraction) return;
        beingMoved = false;
        puzzleManager.LetGoOfPiece(this);
    }

    public string GetLetter()
    {
        return pieceLetter.text;
    }

    public void SetLetter(string letter)
    {
        pieceLetter.text = letter;
    }

    // 0 -> outer, 1 -> inner, 2 -> flat
    public void SetLetter(string letter, int leftMode, int rightMode)
    {
        SetLetter(letter,leftMode,rightMode,2,2);
    }

    // 0 -> outer, 1 -> inner, 2 -> flat
    public void SetLetter(string letter, int leftMode, int rightMode, int topMode, int bottomMode)
    {
        pieceLetter.text = letter;
        left.Set(leftMode);
        right.Set(rightMode);
        top.Set(topMode);
        bottom.Set(bottomMode);
    }

    public void SetAlpha(float alpha)
    {
        SetColorAlpha(color,alpha);
    }

    public void SetColor(Color color)
    {
        SetColorAlpha(color,1f);
    }

    public void SetColorAlpha(Color color, float alpha)
    {
        if (forceStayColor.g != 0f) color = forceStayColor;
        left.SetAlpha(color,alpha);
        right.SetAlpha(color,alpha);
        top.SetAlpha(color,alpha);
        bottom.SetAlpha(color,alpha);
        mainImage.color = new Color(color.r,color.g,color.b,alpha);
        if (extraImage != null) extraImage.color = new Color(extraImage.color.r,extraImage.color.g,extraImage.color.b,alpha);
        pieceLetter.color = new Color(pieceLetter.color.r,pieceLetter.color.g,pieceLetter.color.b,alpha);
    }
}
