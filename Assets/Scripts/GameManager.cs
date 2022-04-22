using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Decks
    public List<Card> deck = new List<Card>(); //protein deck
    public List<Card> discard = new List<Card>(); //protein discard deck

    //Hand and Field
    public Transform[] handSlots;
    public bool[] availableHandSlots;
    public FieldSlot[] fieldSlots;
    private List<string> fieldAttributes = new List<string>{"High in Calories", "Low in Calories", "Contains Healthy Fats", "Contains Unhealthy Fats", 
                                        "Conducive to Weight/Muscle Growth", "Conducive to Weight/Muscle Loss", "Good Source of _____"};
    
    //Others
    public Card selectedCard;
    // private bool canDiscard = true; //Player can only discard from hand once before making a play

    //Object storing information about what happens on any given turn (passed into JSON)
    //Values assigned in adjustMeter()
    public class TurnData
    {
        public string moveType; //Play or Discard
        public string cardTitle; 
        public int calories; 
        public bool goodPlay; //a good play moves the corresponding meter closer to center

        public TurnData(string moveType, Card card, bool goodPlay){
            this.moveType = moveType;
            this.cardTitle = card.title;
            this.calories = card.calories;
            this.goodPlay = goodPlay;
        }
    }
    private const string path = "Assets/Logs/turnLog.json";
    private static StreamWriter Writer = new(path, true); //for writing data to JSON

    public void GameOver(){
        int count = 0;
        foreach(FieldSlot slot in fieldSlots){
            if(slot.availablility == false){
                count += 1;
            }
        }

        if(count == 6){
            print("GAME OVER");
        }
    }

    public void DisplayCard(Card card){
        return;
    }

    public void AssignFieldAttributes(){
        
        GameObject field = GameObject.Find("Field");
        int i = 0;
        foreach(FieldSlot slot in fieldSlots){
            //give each slot an attribute
            int index = UnityEngine.Random.Range(0, fieldAttributes.Count);
            slot.attribute = fieldAttributes[index];
            fieldAttributes.RemoveAt(index);
        

            //display attribute text onto field slot
            GameObject slotText = field.transform.GetChild(i).GetChild(0).gameObject;
            TextMeshPro textComponent = slotText.GetComponent<TextMeshPro>();
            textComponent.text = slot.attribute;
            i += 1;
        }
    }

    public void DrawCard(){
        Card cardDrawn = null;
        if(deck.Count >= 1){
            cardDrawn = deck[UnityEngine.Random.Range(0, deck.Count)]; //later, deck will be ordered randomly and we will draw off the top
        } 
        if(cardDrawn == null){
            return;
        }

        for(int i = 0; i < availableHandSlots.Length; i++){
            if(availableHandSlots[i] == true){
                cardDrawn.gameObject.SetActive(true);                                                                                      //activate card
                cardDrawn.handIndex = i;                                                                                                   //set card's hand slot
                cardDrawn.transform.position = new Vector3(handSlots[i].position.x, handSlots[i].position.y, handSlots[i].position.z - 1); //place card
                cardDrawn.isPlayed = false; 
                availableHandSlots[i] = false;
                deck.Remove(cardDrawn);                                                                                                    //remove card from deck
                return;
            }
        }
    }

    public void PlayCard(FieldSlot slot){
        selectedCard.transform.position = new Vector3(slot.pos.position.x, slot.pos.position.y - (float)0.5, slot.pos.position.z - 1);
        selectedCard.isPlayed = true;
        selectedCard.cardSlot = slot; //sets position variable of card on field
        availableHandSlots[selectedCard.handIndex] = true;
        selectedCard.handIndex = -1;
        slot.availablility = false;
        GameOver();
        return;
    }

    public void Discard(Card card){
        discard.Add(card); 
        card.gameObject.SetActive(false);
        availableHandSlots[card.handIndex] = true;
        //card.cardSlot.availablility = true; //set field position to available after discard
    }

    void Start(){
        //Writer.WriteLine("Game Start:");
        AssignFieldAttributes();
    }

    void Update(){
        
    }
}