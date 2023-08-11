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
    public int pickedBattle = -1;
    public int removeCount;

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

    private readonly Vector2 _stThreatPos = new Vector2(150, 450);
    private readonly Vector2 _ndThreatPos = new Vector2(150, 150);
    private readonly Vector2 _pickedThreatPos = new Vector2(150, 400);
    private readonly Vector2 _battleCardPos = new Vector2(500, 500);

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

    private void Update()
    {
        UIController.I.buttonDrawBattle.GetComponent<Image>().color = (nowDraw > 0) ? new Color32(255, 255, 255, 255) : new Color32(200, 100, 100, 255);

        UIController.I.buttonResolve.GetComponent<Button>().interactable = (nowBattle >= nowDanger) && (battleFieldList.Count > 0);
        UIController.I.textRemoveCount.text = "REMOVE COUNT\n: " + removeCount;
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
            DrawCard(CardType.THREAT, _stThreatPos);
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
            DrawCard(CardType.THREAT, _ndThreatPos);
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

            threatField.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = _pickedThreatPos;

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
            if (nowDraw  < 1)
                life -= 1;
            else
                nowDraw -= 1;

            nowBattle += battleDeckList[0].GetComponent<CardScript>().battle;

            DrawCard(CardType.BATTLE, _battleCardPos);
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
        {
            removeCount = (nowDanger - nowBattle);
            life -= removeCount;
        }
        else
        {
            removeCount = 0;
        }
            

        foreach (Transform child in battleField.transform)
        {
            child.GetComponent<CardScript>().buttonRemove.SetActive(true);
        }

        UIController.I.buttonDrawBattle.interactable = false;
        UIController.I.buttonResolve.SetActive(false);
        UIController.I.textRemoveCount.gameObject.SetActive(true);
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
        UIController.I.textRemoveCount.gameObject.SetActive(false);
        UIController.I.buttonNextThreat.SetActive(false);
    }

    public bool BattleCardEffect(EffectType effType, int effector)
    {
        if (effType == EffectType.LIFEPlusOne)
        {
            life += 1;
        }
        else if (effType == EffectType.LIFEMinusOne)
        {
            // CardScript.Start()
        }
        else if (effType == EffectType.LIFEMinusTwo)
        {
            // CardScript.Start()
        }
        else if (effType == EffectType.DRAWOne)
        {
            nowDraw += 1;
        }
        else if (effType == EffectType.DRAWTwo)
        {
            nowDraw += 2;
        }
        else if (effType == EffectType.DESTROY)
        {
            if (pickedBattle < 0)
                return false;

            RemoveBattleCard(pickedBattle);

            pickedBattle = -1;
        }
        else if (effType == EffectType.DOUBLE)
        {
            if (pickedBattle < 0)
                return false;

            nowBattle += battleFieldList[pickedBattle].GetComponent<CardScript>().battle;
        }
        else if (effType == EffectType.COPY)
        {
            if (pickedBattle < 0)
                return false;

            battleField.transform.GetChild(effector).GetComponent<CardScript>().effType = battleFieldList[pickedBattle].GetComponent<CardScript>().effType;
            return false;
        }
        else if (effType == EffectType.STEP)
        {
            nowDanger = nowThreat.danger[level - 1];
        }
        else if (effType == EffectType.SORT)
        {

        }
        else if (effType == EffectType.EXCHANGEOne)
        {
            if (pickedBattle < 0)
                return false;

            EffectExchange();
        }
        else if (effType == EffectType.EXCHANGETwo)
        {
            if (pickedBattle < 0)
                return false;

            EffectExchange();
            battleField.transform.GetChild(effector).GetComponent<CardScript>().effType = EffectType.EXCHANGEOne;
            return false;
        }
        else if (effType == EffectType.BELOW)
        {
            battleDeckList.Add(battleFieldList[pickedBattle]);
            battleFieldList.RemoveAt(pickedBattle);
            Destroy(battleField.transform.GetChild(pickedBattle).gameObject);
        }
        else if (effType == EffectType.MAX)
        {

        }
        else if (effType == EffectType.STOP)
        {
            // CardScript.Start()
        }

        return true;
    }

    public void EffectExchange()
    {
        battlePassedList.Add(battleFieldList[pickedBattle]);
        battleFieldList.RemoveAt(pickedBattle);
        Destroy(battleField.transform.GetChild(pickedBattle).gameObject);

        nowDraw += 1;
        pickedBattle = -1;
    }
}
