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

    private int idxBattle = 0;
    private int idxThreat = 0;

    private void Awake()
    {
        I = this;
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
        DrawCard(battleDeckList[idxBattle], battleField.transform, new Vector2(500, 550 - 20 * idxBattle), CardType.BATTLE);

        idxBattle += 1;
    }

    public void SetThreats()
    {
        DrawCard(threatDeckList[idxThreat], threatField.transform, new Vector2(150, 500), CardType.THREAT);
        DrawCard(threatDeckList[idxThreat+1], threatField.transform, new Vector2(150, 300), CardType.THREAT);

        idxThreat += 2;
    }

    public void ClearField()
    {
        for (int i = 0; i < battleField.transform.childCount; i++)
        {
            Destroy(battleField.transform.GetChild(i).gameObject);
        }

        idxBattle = 0;
        ShuffleCardList(battleDeckList);

        for (int i = 0; i < threatField.transform.childCount; i++)
        {
            Destroy(threatField.transform.GetChild(i).gameObject);
        }

        idxThreat = 0;
        ShuffleCardList(threatDeckList);
    }
}
