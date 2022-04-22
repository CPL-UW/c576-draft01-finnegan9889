using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private GameManager gm;
    public bool isPlayed;
    //public bool isSelected;
    public int handIndex;

    public string title;
    public int calories;

    public FieldSlot cardSlot; //The field slot where this card is placed (position)

    private void OnMouseDown(){
        gm.selectedCard = this;
        gm.DisplayCard(this);
    }

    private void Start(){
        gm = FindObjectOfType<GameManager>();
    }

    private void OnMouseOver(){
        if(!isPlayed){
            if(Input.GetMouseButtonDown(1)){ //right click discards
                gm.Discard(this);
            } 
        }
        
    }
}