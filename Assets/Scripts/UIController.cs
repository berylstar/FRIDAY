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
    public Button buttonDrawThreat;
    public GameObject buttonPickup;
    public GameObject buttonPickdown;
    public Button buttonDrawBattle;
    public Text textNowBattle;
    public GameObject buttonResolve;
    public GameObject buttonGiveup;

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
        textDeck.text = "x" + GameController.I.battleDeckList.Count;
        textNowBattle.text = GameController.I.nowBattle + "p";
    }

    public void ButtonPirate()
    {
        activePirate = !activePirate;
        panelPirtate.SetActive(activePirate);
    }
}
