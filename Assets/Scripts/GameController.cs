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
    public List<GameObject> deckList = new List<GameObject>();
    public List<GameObject> threatList = new List<GameObject>();
    public List<GameObject> oldList = new List<GameObject>();

    private int idxDeck = 0;
    private int idxThreat = 0;

    private void Awake()
    {
        I = this;
    }

    private void ShuffleCardList(List<GameObject> list)
    {
        int random1, random2;
        GameObject temp;

        for (int i = 0; i < deckList.Count; i++)
        {
            random1 = Random.Range(0, deckList.Count);
            random2 = Random.Range(0, deckList.Count);

            temp = deckList[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }
    }

    public void DrawCard(GameObject go, Transform parent, Vector2 pos)
    {
        GameObject inst = Instantiate(go) as GameObject;
        inst.transform.SetParent(parent);
        inst.GetComponent<RectTransform>().anchoredPosition = pos;
        inst.transform.localScale = new Vector3(1, 1, 1);
    }

    public void DrawCardFromDeck()
    {
        //GameObject inst = Instantiate(deckList[indexDrawCard]) as GameObject;
        //inst.transform.SetParent(battlesBox.transform);
        //inst.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50 * indexDrawCard);
        //inst.transform.localScale = new Vector3(1, 1, 1);

        DrawCard(deckList[idxDeck], battleField.transform, new Vector2(0, -50 * idxDeck));

        idxDeck += 1;
    }

    public void SetThreats()
    {
        DrawCard(threatList[idxThreat], threatField.transform, new Vector2(-300, 100));
        DrawCard(threatList[idxThreat+1], threatField.transform, new Vector2(-300, -100));

        idxThreat += 2;
    }

    public void ClearField()
    {
        for (int i = 0; i < battleField.transform.childCount; i++)
        {
            Destroy(battleField.transform.GetChild(i).gameObject);
        }

        idxDeck = 0;
        ShuffleCardList(deckList);

        for (int i = 0; i < threatField.transform.childCount; i++)
        {
            Destroy(threatField.transform.GetChild(i).gameObject);
        }

        idxThreat = 0;
        ShuffleCardList(threatList);
    }

    public void TEST()
    {
        ShuffleCardList(deckList);
    }
}
