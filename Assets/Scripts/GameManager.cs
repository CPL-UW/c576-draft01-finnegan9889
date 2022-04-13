using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public List<Card> cDeck = new List<Card>(); //carb deck
    public List<Card> cDiscard = new List<Card>(); //carb discard deck
    public List<Card> pDeck = new List<Card>(); //protein deck
    public List<Card> pDiscard = new List<Card>(); //protein discard deck

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
    
    //Object storing information about what happens on any given turn (passed into JSON)
    //Values assigned in adjustMeter()
    public class TurnData
    {
        public string moveType; //Play or Discard
        public int cardType; 
        public int calories; 
        public bool goodPlay; //a good play moves the corresponding meter closer to center

        public TurnData(string moveType, Card card, bool goodPlay){
            this.moveType = moveType;
            this.cardType = card.type;
            this.calories = card.calories;
            this.goodPlay = goodPlay;
        }
    }
    private const string path = "Assets/Logs/turnLog.json";
    private static StreamWriter Writer = new(path, true); //for writing dara to JSON

    public void DrawCard(int cardType){
        //Debug.Log(cardType);
        Card cardDrawn = null;
        if(cardType == 0 && cDeck.Count >= 1){
            cardDrawn = cDeck[UnityEngine.Random.Range(0, cDeck.Count)]; //later, deck will be ordered randomly and we will draw off the top
        }else if(cardType == 1 && pDeck.Count >= 1){
            cardDrawn = pDeck[UnityEngine.Random.Range(0, pDeck.Count)]; //later, deck will be ordered randomly and we will draw off the top
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
                card.cardSlot = fieldSlots[i]; //sets position variable of card on field
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
        bool goodPlay;
        string moveType;
        if(addition){
            moveType = "Play";
        }else{
            moveType = "Discard";
        }
        
        int weight = card.calories / 10;
        if(addition && card.type == 0){ // add carb
            if( Math.Abs(50 - (cMeterValue + weight)) <=  Math.Abs(50 - (cMeterValue))){
                goodPlay = true;
            }else{
                goodPlay = false;
            }
            cMeterValue += weight;
            cMeter.SetValue(cMeterValue);
        }else if(addition && card.type == 1){ // add protein
            if( Math.Abs(50 - (pMeterValue + weight)) <=  Math.Abs(50 - (pMeterValue))){
                goodPlay = true;
            }else{
                goodPlay = false;
            }
            pMeterValue += weight;
            pMeter.SetValue(pMeterValue);
        }else if(!addition && card.type == 0){ // subtract carb
            if( Math.Abs(50 - (cMeterValue - weight)) <=  Math.Abs(50 - (cMeterValue))){
                goodPlay = true;
            }else{
                goodPlay = false;
            }
            cMeterValue -= weight;
            cMeter.SetValue(cMeterValue);
        }else{ // subtract protein
            if( Math.Abs(50 - (pMeterValue - weight)) <=  Math.Abs(50 - (pMeterValue))){
                goodPlay = true;
            }else{
                goodPlay = false;
            }
            pMeterValue -= weight;
            pMeter.SetValue(pMeterValue);
        }

        //write data to JSON
        Writer.WriteLine(JsonUtility.ToJson(new TurnData(moveType, card, goodPlay)));
        Writer.Flush();
    }

    void Start(){
        Writer.WriteLine("Game Start:");
        overweight = UnityEngine.Random.Range(0, 2) == 1; //overweight = true, then high carb/protein start
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