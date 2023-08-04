using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController I;
    
    public int level = 0;
    public int life = 20;

    public GameObject battlesBox;
    public GameObject threatsBox;

    [Header("CARD")]
    public List<GameObject> deckList = new List<GameObject>();
    public List<GameObject> threatList = new List<GameObject>();
    public List<GameObject> oldList = new List<GameObject>();

    private int indexDrawCard = 0;

    private void Awake()
    {
        I = this;
    }

    private void ShuffleDeck()
    {
        int random1, random2;
        GameObject temp;

        for (int i = 0; i < deckList.Count; i++)
        {
            random1 = Random.Range(0, deckList.Count);
            random2 = Random.Range(0, deckList.Count);

            temp = deckList[random1];
            deckList[random1] = deckList[random2];
            deckList[random2] = temp;
        }
    }

    public void DrawCardFromDeck()
    {
        GameObject inst = Instantiate(deckList[indexDrawCard], new Vector3(0, -50 * indexDrawCard, 1), Quaternion.identity) as GameObject;
        inst.transform.SetParent(battlesBox.transform);
        inst.transform.localScale = new Vector3(1, 1, 1);

        indexDrawCard += 1;
    }

    public void ClearBattles()
    {
        for (int i = 0; i < battlesBox.transform.childCount; i++)
        {
            Destroy(battlesBox.transform.GetChild(i).gameObject);
        }

        indexDrawCard = 0;
        ShuffleDeck();
    }

    public void TEST()
    {
        ShuffleDeck();
    }
}
