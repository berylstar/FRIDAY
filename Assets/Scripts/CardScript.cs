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

    [Header("THREAT")]
    public GameObject objectThreat;
    public int draw;
    public int level1;
    public int level2;
    public int level3;

    [Header("BATTLE")]
    public GameObject objectBattle;
    public Button buttonEffect;
    public Button buttonRemove;
    public Button buttonPick;
    public int remove;
    public int battle;

    private Animator ani;
    private bool isClicked = false;

    private void Start()
    {
        ani = GetComponent<Animator>();
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

    // Event Trigger�� ���� Drag�� �� ����
    public void MoveToMouse()
    {
        if (cardType == CardType.BATTLE)
        {
            transform.Translate(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        }        
    }

    // ���� �ذ��ϰ� ���� �ֱ�
    public void Resolve()
    {
        if (cardType == CardType.THREAT)
        {
            cardType = CardType.BATTLE;
            objectThreat.SetActive(false);
            objectBattle.SetActive(true);
            isClicked = false;
            GameController.I.deckList.Add(this.gameObject);
        }
    }
}
