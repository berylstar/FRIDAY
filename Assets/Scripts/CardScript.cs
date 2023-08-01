using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    [Header("Infomation")]
    public int iBattle;
    public int iRemove;
    public int iDraw;
    public int iLevel1;
    public int iLevel2;
    public int iLevel3;
    public string sName;
    public string sEffect;

    [Header("UI")]
    public Text textBattle;
    public Text textRemove;
    public Text textDraw;
    public Text textLevel1;
    public Text textLevel2;
    public Text textLevel3;
    public Text textName;
    public Text textEffect;

    private void Start()
    {
        textBattle.text = iBattle.ToString();
        textRemove.text = iRemove.ToString();
        textDraw.text = iDraw.ToString();
        textLevel1.text = iLevel1.ToString();
        textLevel2.text = iLevel2.ToString();
        textLevel3.text = iLevel3.ToString();
        textName.text = sName;
        textEffect.text = sEffect;
    }
}
