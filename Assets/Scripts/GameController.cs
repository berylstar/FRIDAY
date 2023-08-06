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
    public List<GameObject> battleDeckList = new List<GameObject>();
    public List<GameObject> battleFieldList = new List<GameObject>();
    public List<GameObject> battlePassedList = new List<GameObject>();

    public List<GameObject> threatDeckList = new List<GameObject>();
    public List<GameObject> threatFieldList = new List<GameObject>();
    public List<GameObject> threatPassedList = new List<GameObject>();

    public List<GameObject> oldList = new List<GameObject>();

    public List<GameObject> allCards = new List<GameObject>();

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        foreach (GameObject card in threatDeckList)
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
        {
            DrawCard(threatDeckList[0], threatField.transform, new Vector2(150, 450), CardType.THREAT);
            UIController.I.buttonPickup.SetActive(true);

            UIController.I.buttonThreat.interactable = false;
        }
        else
        {
            if (level < 3)
                level += 1;
            return;
        }

        if (threatDeckList.Count > 0)
        {
            DrawCard(threatDeckList[0], threatField.transform, new Vector2(150, 150), CardType.THREAT);
            UIController.I.buttonPickdown.SetActive(true);
        }
            
    }

    public void PickThreat(int i)
    {
        UIController.I.buttonPickup.SetActive(false);
        UIController.I.buttonPickdown.SetActive(false);

        if (i == 1 && threatFieldList.Count == 1)
        {
            ClearField();
            return;
        }

        threatField.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(150, 400);

        if (i == 1 || (i == 0 && threatFieldList.Count == 2))
        {
            int ri = i == 0 ? 1 : 0;
            int sn = threatField.transform.GetChild(ri).GetComponent<CardScript>().serialNumber;

            threatDeckList.Add(allCards[sn]);
            threatFieldList.Remove(allCards[sn]);
            Destroy(threatField.transform.GetChild(ri).gameObject);
        }
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
        GameObject card;

        while (battleFieldList.Count > 0)                   // 배틀 카드 정리
        {
            card = battleFieldList[0];
            battleFieldList.Remove(card);
            battleDeckList.Add(card);
        }

        foreach (Transform child in battleField.transform)
        {
            Destroy(child.gameObject);
        }

        ShuffleCardList(battleDeckList);
        
        //////////

        while (threatFieldList.Count > 0)                   // 위협 카드 정리
        {
            card = threatFieldList[0];
            threatFieldList.Remove(card);
            threatDeckList.Add(card);
        }

        foreach (Transform child in threatField.transform)
        {
            Destroy(child.gameObject);
        }

        ShuffleCardList(threatDeckList);

        UIController.I.buttonThreat.interactable = true;
    }
}
