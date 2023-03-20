using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterConfig : MonoBehaviour
{

    public enum CharacterName
    {
        アガー,
        ドリー,
        ライド,
        ごはんちゃん
    }


    [Serializable]
    //キャラクター情報管理クラス
    public class CharacterMap
    {
        [SerializeField] private CharacterName name;
        [SerializeField] private Sprite sprite;
        [SerializeField] private AttributeConfig.AttributeName attribute;
        public int HP;
        [SerializeField] private string skillInfo;
        [SerializeField] private string AbilityInfo;

        public CharacterName getName()
        {
            return this.name;
        }

        public int getHP()
        {
            return this.HP;
        }

    }


    [SerializeField] private List<CharacterMap> characterMaps =new List<CharacterMap>();




// Start is called before the first frame update
    void Start()
    {
        
        
    }

    //指定されたプレイヤーのキャラクタークラスを返す
    public CharacterMap GetPlayerMap(CharacterName name)
    {
        int index = characterMaps.FindIndex(item => item.getName() == name);
        return characterMaps[index];
    }
    


    


    
}
