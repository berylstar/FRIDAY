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
    public Text textEffect;
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

        if (effType == EffectType.STOP)
        {
            GameController.I.nowDraw = 0;
        }
        else if (effType == EffectType.LIFEMinusOne)
        {
            GameController.I.life -= 1;
        }
        else if (effType == EffectType.LIFEMinusTwo)
        {
            GameController.I.life -= 2;
        }
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
        }
    }

    // ButtonRemove
    public void RemoveThis()
    {
        if (cardType == CardType.BATTLE)
        {
            if (GameController.I.removeCount < remove)
                return;

            GameController.I.removeCount -= remove;
            GameController.I.RemoveBattleCard(gameObject.transform.GetSiblingIndex());
        }
    }

    // ButtonEffect
    public void Effect()
    {
        if (GameController.I.BattleCardEffect(effType, gameObject.transform.GetSiblingIndex()))
        {
            // 효과 발동
            buttonEffect.GetComponent<Button>().interactable = false;
            textEffect.color = Color.gray;
        }
    }

    // ButtonPick
    public void Pick()
    {
        GameController.I.idxPickedBattle = gameObject.transform.GetSiblingIndex();
    }
}
