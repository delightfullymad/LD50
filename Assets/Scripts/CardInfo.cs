using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum cardType { Attack, Defence, Heal, Weapon, Armour}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardInfo : ScriptableObject
{
    public string cardName;
    public Color cardColour;
    public Sprite cardImage;
    public string cardDescription;

    public cardType type = cardType.Attack;
    public float heal;
    public float attackMod = 1;


    public Weapon weapon;
    public Armour armour;

    

}
