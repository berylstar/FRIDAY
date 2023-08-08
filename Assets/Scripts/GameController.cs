using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController I;
    
    public int level = 0;
    public int life = 20;

    public GameObject battleField;
    public GameObject threatField;

    public CardScript nowThreat = null;
    public int nowDraw;
    public int nowDanger;
    public int nowBattle;
    public GameObject pickedThreat = null;

    [Header("CARD")]
    public List<GameObject> battleDeckList = new List<GameObject>();
    public List<GameObject> battleFieldList = new List<GameObject>();
    public List<GameObject> battlePassedList = new List<GameObject>();
    public List<GameObject> battleRemovedList = new List<GameObject>();

    public List<GameObject> threatDeckList = new List<GameObject>();
    public List<GameObject> threatFieldList = new List<GameObject>();
    public List<GameObject> threatPassedList = new List<GameObject>();

    public List<GameObject> oldList = new List<GameObject>();
    public List<GameObject> TooOldList = new List<GameObject>();

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

        ShuffleList(battleDeckList);
        ShuffleList(threatDeckList);
        ShuffleList(oldList);
        ShuffleList(TooOldList);
    }

    // 카드 리스트를 인자로 받아 셔플
    private void ShuffleList(List<GameObject> list)
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
    
    // 덱에 더 이상 카드가 없을 경우 PassedList를 덱으로 합침
    private void ResetDeck(CardType type)
    {
        if (type == CardType.THREAT)
        {
            foreach (GameObject card in threatPassedList)
            {
                threatDeckList.Add(card);
            }
            threatPassedList.Clear();
            ShuffleList(threatDeckList);
        }

        else if (type == CardType.BATTLE)
        {
            foreach (GameObject card in battlePassedList)
            {
                battleDeckList.Add(card);
            }
            battlePassedList.Clear();
            ShuffleList(battleDeckList);
        }
    }

    private void DrawCard(CardType type, Vector2 pos)
    {
        List<GameObject> decklist, tolist;
        Transform parent;

        if (type == CardType.THREAT)
        {
            decklist = threatDeckList;
            tolist = threatFieldList;
            parent = threatField.transform;
        }
        else
        {
            decklist = battleDeckList;
            tolist = battleFieldList;
            parent = battleField.transform;
        }

        GameObject card = Instantiate(decklist[0]) as GameObject;
        card.transform.SetParent(parent, false);
        card.GetComponent<RectTransform>().anchoredPosition = pos;
        card.transform.localScale = new Vector3(1, 1, 1);

        tolist.Add(decklist[0]);
        decklist.RemoveAt(0);
    }

    // 위협 카드 두 장 세트
    public void DrawTwoThreats()
    {
        if (threatDeckList.Count > 0)
        {
            DrawCard(CardType.THREAT, new Vector2(150, 450));
            UIController.I.buttonPickup.SetActive(true);
            UIController.I.buttonPickdown.SetActive(true);

            UIController.I.buttonDrawThreat.interactable = false;
        }
        else
        {
            if (level < 3)
            {
                level += 1;
                ResetDeck(CardType.THREAT);
            }

            return;
        }

        if (threatDeckList.Count > 0)
        {
            DrawCard(CardType.THREAT, new Vector2(150, 150));
        }
    }

    // 세트 된 2장의 위협카드 중 맞설 위협 선택
    public void PickThreat(int i)
    {
        UIController.I.buttonPickup.SetActive(false);
        UIController.I.buttonPickdown.SetActive(false);

        if (i == 1 && threatFieldList.Count == 1)
        {
            ReadyForNextThreat();
        }
        else
        {
            UIController.I.buttonDrawBattle.interactable = true;
            UIController.I.textNowBattle.gameObject.SetActive(true);
            UIController.I.buttonResolve.SetActive(true);
            UIController.I.buttonGiveup.SetActive(true);

            threatField.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(150, 400);

            if (threatFieldList.Count == 2)
            {
                int other = i == 0 ? 1 : 0;

                threatPassedList.Add(threatFieldList[other]);
                threatFieldList.Remove(threatFieldList[other]);
                Destroy(threatField.transform.GetChild(other).gameObject);
            }

            nowThreat = threatFieldList[0].GetComponent<CardScript>();
            nowDraw = nowThreat.draw;
            nowDanger = nowThreat.danger[level];
        }
    }

    // 위협에 맞서기 위해 배틀카드 뽑기
    public void DrawBattleCard()
    {
        if (battleDeckList.Count > 0)
        {
            if (nowDraw <= 0)
                life -= 1;
            else
                nowDraw -= 1;

            nowBattle += battleDeckList[0].GetComponent<CardScript>().battle;

            UIController.I.buttonResolve.GetComponent<Button>().interactable = (nowBattle >= nowDanger);

            DrawCard(CardType.BATTLE, new Vector2(500, 500));
        }
        else
        {
            // 노화 카드 추가
            ResetDeck(CardType.BATTLE);
        }
    }

    // nowBattle >= nowDanger 일 때 위협 해결
    public void ResolveThreat()
    {
        if (nowThreat == null)
            return;

        threatFieldList[0].GetComponent<CardScript>().ChangeBattleMode();
        battlePassedList.Add(threatFieldList[0]);
        threatFieldList.Clear();
        ReadyForNextThreat();
    }

    // 위협 포기
    public void GiveupThreat()
    {
        if (nowThreat == null)
            return;

        if (nowDanger > nowBattle)
            life -= (nowDanger - nowBattle);

        foreach (Transform child in battleField.transform)
        {
            child.GetComponent<CardScript>().buttonRemove.SetActive(true);
        }

        UIController.I.buttonDrawBattle.interactable = false;
        UIController.I.buttonResolve.SetActive(false);
        UIController.I.buttonNextThreat.SetActive(true);
    }

    // 위협 포기 후 배틀필드에서 카드 제거
    public void RemoveBattleCard(int idx)
    {
        battleRemovedList.Add(battleFieldList[idx]);
        battleFieldList.RemoveAt(idx);
        Destroy(battleField.transform.GetChild(idx).gameObject);
    }

    public void ReadyForNextThreat()
    {
        while (battleFieldList.Count > 0)                   // 배틀 카드 정리
        {
            battlePassedList.Add(battleFieldList[0]);
            battleFieldList.Remove(battleFieldList[0]);
        }

        foreach (Transform child in battleField.transform)
        {
            Destroy(child.gameObject);
        }
        
        //////////

        while (threatFieldList.Count > 0)                   // 위협 카드 정리
        {
            threatPassedList.Add(threatFieldList[0]);
            threatFieldList.Remove(threatFieldList[0]);
        }

        foreach (Transform child in threatField.transform)
        {
            Destroy(child.gameObject);
        }

        nowThreat = null;
        nowDraw = 0;
        nowDanger = 0;
        nowBattle = 0;
        UIController.I.buttonDrawThreat.interactable = true;
        UIController.I.buttonDrawBattle.interactable = false;
        UIController.I.textNowBattle.gameObject.SetActive(false);
        UIController.I.buttonResolve.SetActive(false);
        UIController.I.buttonGiveup.SetActive(false);
        UIController.I.buttonNextThreat.SetActive(false);
    }

    public void EffectDestroy()
    {
        int idx = battleField.transform.Find(pickedThreat.name).GetSiblingIndex();

        battleRemovedList.Add(battleFieldList[idx]);
        battleFieldList.RemoveAt(idx);
        Destroy(battleField.transform.GetChild(idx).gameObject);
    }

    public void EffectExchange()
    {
        int idx = battleField.transform.Find(pickedThreat.name).GetSiblingIndex();

        battlePassedList.Add(battleFieldList[idx]);
        battleFieldList.RemoveAt(idx);
        Destroy(battleField.transform.GetChild(idx).gameObject);

        nowDraw += 1;
    }
}
