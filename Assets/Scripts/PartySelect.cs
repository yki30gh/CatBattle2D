using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PartySelect : MonoBehaviour
{
    [SerializeField] private CharacterConfig.CharacterName[] players = new CharacterConfig.CharacterName[3];


    private CharacterConfig _characterConfig;
    private List<CharacterConfig.CharacterMap> playerMap = new List<CharacterConfig.CharacterMap>();
    
    //private CharacterConfig.CharacterMap[] playerMaps;



    // Start is called before the first frame update
    void Start()
    {
        _characterConfig = GameObject.Find("CharacterMap").GetComponent<CharacterConfig>();

        for (int i = 0; i < players.Length; i++)
        {

            
            if (players[i]>=0)
            {
                playerMap.Add(_characterConfig.GetPlayerMap(players[i]));
                Debug.Log(playerMap[i].getName());
            }
            
        }
        
        



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
