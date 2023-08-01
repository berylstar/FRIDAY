using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController I;

    public GameObject card;

    public int level = 0;
    public int life = 20;

    private void Awake()
    {
        I = this;
    }

}
