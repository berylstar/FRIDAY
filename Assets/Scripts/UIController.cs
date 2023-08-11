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
    
    public GameObject panelPirtate;
    private bool activePirate = false;

    public Text textBattleDeckCounter;
    public Button buttonDrawBattle;
    public Text textNowBattlePoint;

    public Text textThreatDeckCounter;
    public Button buttonDrawThreat;
    public GameObject buttonPickup;
    public GameObject buttonPickdown;
    public GameObject buttonResolve;
    public GameObject buttonGiveup;
    public Text textRemoveCount;
    public GameObject buttonNextThreat;

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
        textBattleDeckCounter.text = GameController.I.battleDeckList.Count + "/" + GameController.I.battleDeckCounter;
        textThreatDeckCounter.text = GameController.I.threatDeckList.Count + "/" + GameController.I.threatDeckCounter;
        textNowBattlePoint.text = GameController.I.nowBattle + "p";
    }

    public void ButtonPirate()
    {
        activePirate = !activePirate;
        panelPirtate.SetActive(activePirate);
    }
}
