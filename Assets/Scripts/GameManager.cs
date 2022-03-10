using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Card> cDeck = new List<Card>(); //carb deck
    public List<Card> cDiscard = new List<Card>(); //carb discard
    public List<Card> pDeck = new List<Card>(); //protein deck
    public List<Card> pDiscard = new List<Card>(); //protein discard

    public Transform[] handSlots;
    public bool[] availableHandSlots;
    public FieldSlot[] fieldSlots;

    public bool overweight; 

    //Meters
    public Meter cMeter;
    public Meter pMeter;
    public int maxMeterValue = 100;
    public int cMeterValue;
    public int pMeterValue;
    

    public void DrawCard(int cardType){
        //Debug.Log(cardType);
        Card cardDrawn = null;
        if(cardType == 0 && cDeck.Count >= 1){
            cardDrawn = cDeck[Random.Range(0, cDeck.Count)]; //later, deck will be ordered randomly and we will draw off the top
        }else if(cardType == 1 && pDeck.Count >= 1){
            cardDrawn = pDeck[Random.Range(0, pDeck.Count)]; //later, deck will be ordered randomly and we will draw off the top
        } 
        if(cardDrawn == null){
            return;
        }
        for(int i = 0; i < availableHandSlots.Length; i++){
            if(availableHandSlots[i] == true){
                cardDrawn.gameObject.SetActive(true);
                cardDrawn.handIndex = i;
                cardDrawn.transform.position = new Vector3(handSlots[i].position.x, handSlots[i].position.y, handSlots[i].position.z - 1);
                cardDrawn.isPlayed = false;
                availableHandSlots[i] = false;
                if(cardType == 0){
                cDeck.Remove(cardDrawn);
                    return;
                }else if(cardType == 1){
                    pDeck.Remove(cardDrawn);
                    return;
                }
            }
        }
    }

    public void PlayCard(Card card){
        for(int i = 0; i < fieldSlots.Length; i++){
            if(fieldSlots[i].availablility == true){
                card.transform.position = new Vector3(fieldSlots[i].pos.position.x, fieldSlots[i].pos.position.y, fieldSlots[i].pos.position.z - 1);
                card.isPlayed = true;
                card.cardSlot = fieldSlots[i]; //sets card's position variable of card on field
                availableHandSlots[card.handIndex] = true;
                card.handIndex = -1;
                fieldSlots[i].availablility = false;

                adjustMeter(card, true);

                return;
            }
        }
    }

    public void Discard(Card card){
        if(card.type == 0){ //carb
            cDiscard.Add(card);
        }else if(card.type == 1){ //protein
            pDiscard.Add(card);
        }

        adjustMeter(card, false);
        
        card.gameObject.SetActive(false);
        //set field position to available after discard
        card.cardSlot.availablility = true;
    }

    public void initializeMeters(){
        cMeter.SetMaxValue(maxMeterValue);
        pMeter.SetMaxValue(maxMeterValue);

        if(overweight){ //carb and protein intake too high
            cMeterValue = (int)(maxMeterValue * 0.8);
            cMeter.SetValue(cMeterValue);
            pMeterValue = (int)(maxMeterValue * 0.8);
            pMeter.SetValue(pMeterValue);
        }else{ //intakes too low
            cMeterValue = (int)(maxMeterValue * 0.20);
            cMeter.SetValue(cMeterValue);
            pMeterValue = (int)(maxMeterValue * 0.20);
            pMeter.SetValue(pMeterValue);
        }
    }

    //Change corresponding meter's value depending on card's calorie count
    //addition == true means we are adding the value to the meter. false means we subtract
    public void adjustMeter(Card card, bool addition){
        int weight = card.calories / 10;
        if(addition && card.type == 0){ // add carb
            cMeterValue += weight;
            cMeter.SetValue(cMeterValue);
        }else if(addition && card.type == 1){ // add protein
            pMeterValue += weight;
            pMeter.SetValue(pMeterValue);
        }else if(!addition && card.type == 0){ // subtract carb
            cMeterValue -= weight;
            cMeter.SetValue(cMeterValue);
        }else{ // subtract protein
            pMeterValue -= weight;
            pMeter.SetValue(pMeterValue);
        }
    }

    void Start(){
        overweight = Random.Range(0, 2) == 1; //overweight = true, then high carb/protein start
        initializeMeters();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            cMeterValue -= 10;
            pMeterValue -= 10;
            cMeter.SetValue(cMeterValue);
            pMeter.SetValue(pMeterValue);
        }
    }
}