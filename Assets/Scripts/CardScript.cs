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
    private bool isFlipped = false;

    [Header("BATTLE")]
    public GameObject objectBattle;
    public int remove;
    public int battle;

    private Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();
    }

    public void ShowFlip()
    {
        ani.SetTrigger("Flip");

        if (cardType == CardType.THREAT)
        {
            isFlipped = !isFlipped;
            objectBattle.SetActive(isFlipped);
            objectThreat.SetActive(!isFlipped);
        }
    }
}
