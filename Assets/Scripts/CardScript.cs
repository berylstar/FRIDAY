using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    BATTLE,
    THREAT,
}

public enum EffectType
{
    NORMAL,
    LIFEPlusOne,
    LIFEMinusOne,
    LIFEMinusTwo,
    DRAWOne,
    DRAWTwo,
    DESTROY,
    DOUBLE,
    COPY,
    STEP,
    SORT,
    EXCHANGEOne,
    EXCHANGETwo,
    BELOW,
    MAX,
    STOP
}

public class CardScript : MonoBehaviour
{
    public CardType cardType;

    [Header("THREAT")]
    public GameObject objectThreat;
    public int draw;
    public List<int> danger = new List<int>() { 0, 0, 0 };

    [Header("BATTLE")]
    public GameObject objectBattle;
    public GameObject buttonEffect;
    public GameObject buttonRemove;
    public GameObject buttonPick;
    public int battle;
    public int remove;
    public EffectType effType;

    private Animator ani;
    private RectTransform rt;

    private bool isClicked = false;

    private void Start()
    {
        ani = GetComponent<Animator>();
        rt = GetComponent<RectTransform>();

        buttonEffect.GetComponent<Button>().interactable = (effType != EffectType.NORMAL);
    }

    public void ClickCard()
    {
        ani.SetTrigger("Flip");
        isClicked = !isClicked;

        if (cardType == CardType.THREAT)
        {
            objectBattle.SetActive(isClicked);
            objectThreat.SetActive(!isClicked);
        }
        else
        {
            buttonEffect.SetActive(isClicked);
            buttonPick.SetActive(isClicked);
        }
    }

    // Event Trigger를 통해 Drag할 때 실행
    public void MoveToMouse()
    {
        if (cardType == CardType.BATTLE)
        {
            Vector3 pos = Input.mousePosition;

            if (pos.x < 400 || pos.x > 900 || pos.y < 70 || pos.y > 510)
                return;

            rt.anchoredPosition = pos;
        }        
    }

    public void ChangeThreatMode()
    {
        if (cardType == CardType.BATTLE)
        {
            cardType = CardType.THREAT;
            objectThreat.SetActive(true);
            objectBattle.SetActive(false);
            isClicked = false;
        }
    }

    public void ChangeBattleMode()
    {
        if (cardType == CardType.THREAT)
        {
            cardType = CardType.BATTLE;
            objectThreat.SetActive(false);
            objectBattle.SetActive(true);
            isClicked = false;
        }
    }

    // ButtonRemove
    public void RemoveThis()
    {
        if (cardType == CardType.BATTLE)
        {
            GameController.I.life -= remove;
            GameController.I.RemoveBattleCard(gameObject.transform.GetSiblingIndex());
        }
    }

    // ButtonEffect
    public void Effect()
    {
        if (effType == EffectType.LIFEPlusOne)
        {
            GameController.I.life += 1;
        }
        else if (effType == EffectType.LIFEMinusOne)
        {
            GameController.I.life -= 1;
        }
        else if (effType == EffectType.LIFEMinusTwo)
        {
            GameController.I.life -= 2;
        }
        else if (effType == EffectType.DRAWOne)
        {
            GameController.I.nowDraw += 1;
        }
        else if (effType == EffectType.DRAWTwo)
        {
            GameController.I.nowDraw += 2;
        }
        else if (effType == EffectType.DESTROY)
        {
            if (GameController.I.pickedThreat == null)
                return;

            GameController.I.EffectDestroy();
        }
        else if (effType == EffectType.DOUBLE)
        {
            if (GameController.I.pickedThreat == null)
                return;

            GameController.I.nowBattle += GameController.I.pickedThreat.GetComponent<CardScript>().battle;
        }
        else if (effType == EffectType.COPY)
        {
            if (GameController.I.pickedThreat == null)
                return;

            effType = GameController.I.pickedThreat.GetComponent<CardScript>().effType;
            return;
        }
        else if (effType == EffectType.STEP)
        {

        }
        else if (effType == EffectType.SORT)
        {

        }
        else if (effType == EffectType.EXCHANGEOne)
        {
            if (GameController.I.pickedThreat == null)
                return;

            GameController.I.EffectExchange();
        }
        else if (effType == EffectType.EXCHANGETwo)
        {
            if (GameController.I.pickedThreat == null)
                return;

            GameController.I.EffectExchange();
            effType = EffectType.EXCHANGEOne;
            return;
        }
        else if (effType == EffectType.BELOW)
        {

        }
        else if (effType == EffectType.MAX)
        {

        }
        else if (effType == EffectType.STOP)
        {

        }
        else
            return;

        buttonEffect.GetComponent<Button>().interactable = false;
    }

    // ButtonPick
    public void Pick()
    {
        GameController.I.pickedThreat = this.gameObject;

        int idx = GameObject.Find("CanvasField/BattleField").transform.Find(this.gameObject.name).GetSiblingIndex();
        print(idx);
    }
}
