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

    public void RemoveThis()
    {
        if (cardType == CardType.BATTLE)
        {
            GameController.I.life -= remove;
            GameController.I.RemoveBattleCard(gameObject.transform.GetSiblingIndex());
        }
    }
}
