using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController I;

    [Header("UI")]
    public Image imgLevel;
    public Text textLife;
    public Text textDeck;
    public GameObject panelPirtate;
    private bool activePirate = false;

    [Header("Source")]
    public Sprite[] spriteLevel;

    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
        imgLevel.sprite = spriteLevel[GameController.I.level];
        textLife.text = "x" + GameController.I.life;
        textDeck.text = "x" + GameController.I.deckList.Count;
    }

    public void ButtonPirate()
    {
        activePirate = !activePirate;
        panelPirtate.SetActive(activePirate);
    }
}
