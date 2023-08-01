using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteController : MonoBehaviour
{
    public Image imageLevel;
    public Sprite[] spriteLevels;

    public void ButtonLevel()
    {
        GameController.I.level += 1;
        imageLevel.sprite = spriteLevels[GameController.I.level];
    }
}
