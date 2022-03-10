using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool isPlayed;
    public int handIndex;
    private GameManager gm;

    public string title;
    public int type; //0 = Carb, 1 = Protein
    public int calories;

    public FieldSlot cardSlot; //The field slot where this card is placed (position)

    private void Start(){
        gm = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown(){
        if(isPlayed == false){
            gm.PlayCard(this);
        }else{
            gm.Discard(this);
        }
    }
}