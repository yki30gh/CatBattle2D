using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterConfig;

public class AttributeConfig : MonoBehaviour
{
    

    public enum AttributeName
    {
        Red,
        Blue,
        Green,
        yellow

    }

    [Serializable]
    public struct AttributeMap
    {
        public AttributeName name;
        public Sprite cardBackGroundSprite;
        public Sprite cardMarkSprite;

    }

    [SerializeField] private List<AttributeMap> attributeMaps = new List<AttributeMap>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
