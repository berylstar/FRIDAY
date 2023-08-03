using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController I;
    
    public int level = 0;
    public int life = 20;

    public List<GameObject> deck = new List<GameObject>();
    public GameObject onField;

    private int indexDrawCard = 0;

    private void Awake()
    {
        I = this;
    }

    private void ShuffleDeck()
    {
        int random1, random2;
        GameObject temp;

        for (int i = 0; i < deck.Count; i++)
        {
            random1 = Random.Range(0, deck.Count);
            random2 = Random.Range(0, deck.Count);

            temp = deck[random1];
            deck[random1] = deck[random2];
            deck[random2] = temp;
        }
    }

    public void DrawCardFromDeck()
    {
        GameObject inst = Instantiate(deck[indexDrawCard]) as GameObject;
        inst.transform.SetParent(onField.transform);
        inst.transform.localScale = new Vector3(1, 1, 1);

        indexDrawCard += 1;
    }

    public void ClearOnCard()
    {
        for (int i = 0; i < onField.transform.childCount; i++)
        {
            Destroy(onField.transform.GetChild(i).gameObject);
        }

        indexDrawCard = 0;
        ShuffleDeck();
    }

    public void TEST()
    {
        ShuffleDeck();
    }
}
