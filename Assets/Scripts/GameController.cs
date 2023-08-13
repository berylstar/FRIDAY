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
    public int removeCount;
    [HideInInspector] public int idxPickedBattle = -1;

    [Header("CARD")]
    public int battleDeckCounter;
    public List<GameObject> battleDeckList = new List<GameObject>();
    public List<GameObject> battleFieldList = new List<GameObject>();
    public List<GameObject> battlePassedList = new List<GameObject>();
    public List<GameObject> battleRemovedList = new List<GameObject>();

    public int threatDeckCounter;
    public List<GameObject> threatDeckList = new List<GameObject>();
    public List<GameObject> threatFieldList = new List<GameObject>();
    public List<GameObject> threatPassedList = new List<GameObject>();

    public List<GameObject> agingList = new List<GameObject>();
    public List<GameObject> oldList = new List<GameObject>();

    private readonly Vector2 _stThreatPos = new Vector2(150, 400);
    private readonly Vector2 _ndThreatPos = new Vector2(150, 100);
    private readonly Vector2 _pickedThreatPos = new Vector2(150, 350);
    private readonly Vector2 _battleCardPos = new Vector2(500, 450);

    public bool isMinusOne = false;
    public bool isMinusTwo = false;
    public bool isHighZero = false;
    public bool isStop = false;

    [Header("For Effect Sort")]
    public GameObject canvasSort;
    public Transform transformForSort;
    public GameObject[] buttonSorts;
    private int indexSort = 0;
    private List<GameObject> sortList = new List<GameObject>();

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
        ShuffleList(agingList);
        ShuffleList(oldList);

        battleDeckCounter = battleDeckList.Count;
        threatDeckCounter = threatDeckList.Count;
    }

    private void Update()
    {
        UIController.I.buttonDrawBattle.GetComponent<Image>().color = (nowDraw < 1 || isStop) ? new Color32(200, 100, 100, 255) : new Color32(255, 255, 255, 255);

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

        if (type == CardType.THREAT)
        {
            card.GetComponent<CardScript>().ChangeThreatMode();
        }
        else
        {
            card.GetComponent<CardScript>().ChangeBattleMode();
        }

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
            UIController.I.textNowBattlePoint.gameObject.SetActive(true);
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
            if (nowDraw < 1 || isStop)
                life -= 1;
            else
                nowDraw -= 1;

            DrawCard(CardType.BATTLE, _battleCardPos);
        }
        else
        {
            AddAging();
            ResetDeck(CardType.BATTLE);
        }

        nowBattle = CalculateBattle();
    }

    private int CalculateBattle()
    {
        int sumBattle = 0;
        int maxx = -6;
        int val;

        foreach (Transform child in battleField.transform)
        {
            val = child.GetComponent<CardScript>().battle;
            sumBattle += val;

            if (maxx < val) { maxx = val; }
        }

        if (isHighZero)
            sumBattle -= maxx;

        print("CALC");
        return sumBattle;
    }

    private void AddAging()
    {
        if (agingList.Count > 0)
        {
            battleDeckList.Add(agingList[0]);
            agingList.RemoveAt(0);
        }
        else if (oldList.Count > 0)
        {
            battleDeckList.Add(oldList[0]);
            oldList.RemoveAt(0);
        }
    }

    // nowBattle >= nowDanger 일 때 위협 해결
    public void ResolveThreat()
    {
        if (nowThreat == null)
            return;

        // threatFieldList[0].GetComponent<CardScript>().ChangeBattleMode();
        battlePassedList.Add(threatFieldList[0]);
        threatFieldList.Clear();
        threatDeckCounter -= 1;
        battleDeckCounter += 1;
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
        UIController.I.buttonGiveup.SetActive(false);
        UIController.I.textRemoveCount.gameObject.SetActive(true);
        UIController.I.buttonNextThreat.SetActive(true);
    }

    // 위협 포기 후 배틀필드에서 카드 제거
    public void RemoveBattleCard(int idx)
    {
        battleRemovedList.Add(battleFieldList[idx]);
        battleFieldList.RemoveAt(idx);
        Destroy(battleField.transform.GetChild(idx).gameObject);
        battleDeckCounter -= 1;
    }

    // 다음 위협을 위해 필드 초기화
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

        if (isMinusOne) { life -= 1; }
        if (isMinusTwo) { life -= 2; }

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

        isMinusOne = false;
        isMinusTwo = false;
        isHighZero = false;
        isStop = false;

        UIController.I.buttonDrawThreat.interactable = true;
        UIController.I.buttonDrawBattle.interactable = false;
        UIController.I.textNowBattlePoint.gameObject.SetActive(false);
        UIController.I.buttonResolve.SetActive(false);
        UIController.I.buttonGiveup.SetActive(false);
        UIController.I.textRemoveCount.gameObject.SetActive(false);
        UIController.I.buttonNextThreat.SetActive(false);
    }

    public bool BattleCardEffect(EffectType effType, int idxEffector)
    {
        if (canvasSort.activeInHierarchy)
            return false;

        if (effType == EffectType.LIFEPlusOne)
        {
            life += 1;
        }
        else if (effType == EffectType.LIFEPlusTwo)
        {
            life += 2;
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
            if (idxPickedBattle < 0 || (idxEffector == idxPickedBattle))
                return false;

            RemoveBattleCard(idxPickedBattle);            
        }
        else if (effType == EffectType.DOUBLE)
        {
            if (idxPickedBattle < 0 || (idxEffector == idxPickedBattle))
                return false;

            battleField.transform.GetChild(idxEffector).GetComponent<CardScript>().battle *= 2;
            battleField.transform.GetChild(idxEffector).GetComponent<CardScript>().ApplyBattleText();
        }
        else if (effType == EffectType.COPY)
        {
            if (idxPickedBattle < 0 || (idxEffector == idxPickedBattle))
                return false;

            battleField.transform.GetChild(idxEffector).GetComponent<CardScript>().effType = battleFieldList[idxPickedBattle].GetComponent<CardScript>().effType;
            battleField.transform.GetChild(idxEffector).GetComponent<CardScript>().textEffect.text = battleFieldList[idxPickedBattle].GetComponent<CardScript>().textEffect.text;
            return false;
        }
        else if (effType == EffectType.STEP)
        {
            nowDanger = nowThreat.danger[level - 1];
        }
        else if (effType == EffectType.SORT)
        {
            if (battleDeckList.Count < 2)
                return false;

            EffectSort();
        }
        else if (effType == EffectType.EXCHANGEOne)
        {
            if (idxPickedBattle < 0 || (idxEffector == idxPickedBattle))
                return false;

            EffectExchange();
        }
        else if (effType == EffectType.EXCHANGETwo)
        {
            if (idxPickedBattle < 0 || (idxEffector == idxPickedBattle))
                return false;

            EffectExchange();
            battleField.transform.GetChild(idxEffector).GetComponent<CardScript>().effType = EffectType.EXCHANGEOne;
            battleField.transform.GetChild(idxEffector).GetComponent<CardScript>().textEffect.text = "EXCHANGE x1";
            return false;
        }
        else if (effType == EffectType.BELOW)
        {
            if (idxPickedBattle < 0 || (idxEffector == idxPickedBattle))
                return false;

            battleDeckList.Add(battleFieldList[idxPickedBattle]);
            battleFieldList.RemoveAt(idxPickedBattle);
            Destroy(battleField.transform.GetChild(idxPickedBattle).gameObject);
        }

        nowBattle = CalculateBattle();
        idxPickedBattle = -1;
        return true;
    }

    private void EffectExchange()
    {
        battlePassedList.Add(battleFieldList[idxPickedBattle]);
        battleFieldList.RemoveAt(idxPickedBattle);
        Destroy(battleField.transform.GetChild(idxPickedBattle).gameObject);

        DrawBattleCard();
    }

    public void EffectSort()
    {
        int lim = Mathf.Min(battleDeckList.Count, 3);

        for (int i = 0; i < lim; i++)
        {
            GameObject card = Instantiate(battleDeckList[0]) as GameObject;
            card.transform.SetParent(transformForSort, false);
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2 (-200 + 250 * i, 0);
            card.transform.localScale = new Vector3(1, 1, 1);

            sortList.Add(battleDeckList[0]);
            battleDeckList.RemoveAt(0);

            indexSort = i + 1;
        }

        canvasSort.SetActive(true);
    }

    public void ButtonForSort(int idx)
    {
        battleDeckList.Insert(0, sortList[idx]);
        buttonSorts[idx].SetActive(false);
        transformForSort.GetChild(idx).gameObject.SetActive(false);

        if (indexSort == 1)
        {
            foreach (Transform child in transformForSort)
            {
                Destroy(child.gameObject);
            }

            indexSort = 0;

            foreach (GameObject button in buttonSorts)
            {
                button.SetActive(true);
            }

            sortList.Clear();

            canvasSort.SetActive(false);
        }
        
        indexSort -= 1;
    }
}