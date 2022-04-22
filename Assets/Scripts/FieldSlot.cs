using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSlot : MonoBehaviour
{
    public GameManager gm;
    public bool availablility = true;
    public Transform pos;
    public string attribute;

    private void Start(){
        gm = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown(){
        if(gm.selectedCard != null && gm.selectedCard.isPlayed == false){
            gm.PlayCard(this);
        }
    }
}
