using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{   
    public enum ItemType { Weapon , Item}

    [SerializeField]
    public ItemType itemType;
    public int itemId;
    public int cardLevel;
    public Mesh mesh;
    public MeshRenderer renderer;
    public string cardName;
    public float buffValue;
    public string text;

}
