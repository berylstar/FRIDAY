using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    BATTLE,
    THREAT,
}

public class CardScript : MonoBehaviour
{
    public CardType cardType;
    public int serialNumber;

    [Header("THREAT")]
    public GameObject objectThreat;
    public int draw;
    public List<int> level = new List<int>() { 0, 0, 0 };

    [Header("BATTLE")]
    public GameObject objectBattle;
    public Button buttonEffect;
    public Button buttonRemove;
    public Button buttonPick;
    public int remove;
    public int battle;

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
            buttonEffect.gameObject.SetActive(isClicked);
            buttonRemove.gameObject.SetActive(isClicked);
            buttonPick.gameObject.SetActive(isClicked);
        }
    }

    // Event Trigger를 통해 Drag할 때 실행
    public void MoveToMouse()
    {
        if (cardType == CardType.BATTLE)
        {
            Vector3 pos = Input.mousePosition;

            if (pos.x < 120 || pos.x > 904 || pos.y < 70 || pos.y > 550)
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
}
