using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController I;
    
    public int level = 0;
    public int life = 20;

    public GameObject battleField;
    public GameObject threatField;

    [Header("CARD")]
    public List<GameObject> battleFieldList = new List<GameObject>();
    public List<GameObject> threatFieldList = new List<GameObject>();

    public List<GameObject> battleDeckList = new List<GameObject>();
    public List<GameObject> threatDeckList = new List<GameObject>();

    public List<GameObject> oldList = new List<GameObject>();

    public List<GameObject> allCards = new List<GameObject>();

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        foreach (GameObject card in allCards)
        {
            card.GetComponent<CardScript>().ChangeThreatMode();
        }
    }

    private void ShuffleCardList(List<GameObject> list)
    {
        int random1, random2;
        GameObject temp;

        for (int i = 0; i < list.Count; i++)
        {
            random1 = Random.Range(0, list.Count);
            random2 = Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }
    }

    public void DrawCard(GameObject card, Transform parent, Vector2 pos, CardType type)
    {
        GameObject inst = Instantiate(card) as GameObject;
        inst.transform.SetParent(parent, false);
        inst.GetComponent<RectTransform>().anchoredPosition = pos;
        inst.transform.localScale = new Vector3(1, 1, 1);

        if (type == CardType.BATTLE)
        {
            battleFieldList.Add(card);
            battleDeckList.Remove(card);
        }
        else
        {
            threatFieldList.Add(card);
            threatDeckList.Remove(card);
        }
    }

    public void DrawCardFromDeck()
    {
        if (battleDeckList.Count > 0)
            DrawCard(battleDeckList[0], battleField.transform, new Vector2(500, 550), CardType.BATTLE);
    }

    public void SetThreats()
    {
        if (threatDeckList.Count > 0)
            DrawCard(threatDeckList[0], threatField.transform, new Vector2(150, 500), CardType.THREAT);

        if (threatDeckList.Count > 0)
            DrawCard(threatDeckList[0], threatField.transform, new Vector2(150, 300), CardType.THREAT);
    }

    public void ResolveThreat(int serialNumber)
    {
        GameObject rc = allCards[serialNumber];
        rc.GetComponent<CardScript>().ChangeBattleMode();
        battleDeckList.Add(rc);

        threatFieldList.Remove(allCards[serialNumber]);
    }

    public void ClearField()
    {
        while (battleFieldList.Count > 0)
        {
            GameObject card = battleFieldList[0];
            battleFieldList.Remove(card);
            battleDeckList.Add(card);
        }

        foreach (Transform child in battleField.transform)
        {
            Destroy(child.gameObject);
        }

        ShuffleCardList(battleDeckList);
        
        //////////

        while (threatFieldList.Count > 0)
        {
            GameObject card = threatFieldList[0];
            threatFieldList.Remove(card);
            threatDeckList.Add(card);
        }

        foreach (Transform child in threatField.transform)
        {
            Destroy(child.gameObject);
        }

        ShuffleCardList(threatDeckList);
    }
}
