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
    LIFEPlusTwo,
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

    LIFEMinusOne,
    LIFEMinusTwo,
    HIGHZero,
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
    public Text textEffect;
    public Text textBattlePoint;
    public GameObject buttonEffect;   
    public GameObject buttonPick;
    public GameObject buttonRemove;
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

        if      (effType == EffectType.LIFEMinusOne)    { GameController.I.isMinusOne = true; }
        else if (effType == EffectType.LIFEMinusTwo)    { GameController.I.isMinusTwo = true; }
        else if (effType == EffectType.HIGHZero)        { GameController.I.isHighZero = true; }
        else if (effType == EffectType.STOP)            { GameController.I.isStop = true; }
    }

    private void OnDestroy()
    {
        if      (effType == EffectType.LIFEMinusOne)    { GameController.I.isMinusOne = false; }
        else if (effType == EffectType.LIFEMinusTwo)    { GameController.I.isMinusTwo = false; }
        else if (effType == EffectType.HIGHZero)        { GameController.I.isHighZero = false; }
        else if (effType == EffectType.STOP)            { GameController.I.isStop = false; }
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

            if (pos.x < 400 || pos.x > 900 || pos.y < 70 || pos.y > 460)
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


            buttonEffect.GetComponent<Button>().interactable = (effType != EffectType.NORMAL);
        }
    }

    // BattleDeckList 에서의 인덱스 반환 (BattleDeck에 생성된 인덱스와 같음)
    public int ReturnIndex()
    {
        return gameObject.transform.GetSiblingIndex();
    }

    // ButtonRemove
    public void RemoveThis()
    {
        if (cardType == CardType.BATTLE)
        {
            if (GameController.I.removeCount < remove)
                return;

            GameController.I.removeCount -= remove;
            GameController.I.RemoveBattleCard(ReturnIndex());
        }
    }

    // ButtonEffect
    public void Effect()
    {
        if (GameController.I.BattleCardEffect(effType, ReturnIndex()))
        {
            // 효과 발동
            buttonEffect.GetComponent<Button>().interactable = false;
            textEffect.color = Color.gray;
        }
    }

    // ButtonPick
    public void Pick()
    {
        if (GameController.I.canvasSort.activeInHierarchy)
            return;

        GameController.I.idxPickedBattle = ReturnIndex();
    }

    public void ApplyBattleText()
    {
        textBattlePoint.text = battle.ToString();
    }
}
