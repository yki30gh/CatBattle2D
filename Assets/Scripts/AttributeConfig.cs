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
        Yellow

    }

    [Serializable]
    public struct AttributeMap
    {
        public AttributeName name;
        public Sprite cardBackGroundSprite;
        public Sprite cardMarkSprite;
        public Color colorRGB;

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

    public Sprite GetBackGroundSprite(AttributeName name)
    {
        int index = attributeMaps.FindIndex(item => item.name == name);
        return attributeMaps[index].cardBackGroundSprite;
    }

    public Sprite GetMarkSprite(AttributeName name)
    {
        int index = attributeMaps.FindIndex(item => item.name == name);
        return attributeMaps[index].cardMarkSprite;
    }

    public AttributeMap GetCardAttribute(int num)
    {
        AttributeMap attribute;
        if (num >= 40)
        {
            attribute = attributeMaps[3];
        }
        else if(num>=30)
        {
            attribute = attributeMaps[2];
        }
        else if(num>=20)
        {
            attribute = attributeMaps[1];
        }
        else
        {
            attribute = attributeMaps[0];
        }

        return attribute;
    }

    public string FindPositiveAttibute(string tag)
    {
        string positiveAttribute="";
        AttributeName name;
        AttributeName.TryParse(tag, out name);
        AttributeName[] attributes= (AttributeName[])Enum.GetValues(typeof(AttributeName));

        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] == name)
            {
                int index = (i + 1 == 4) ? 0 : i + 1;
                positiveAttribute = attributes[index].ToString();
            }
        }
        return positiveAttribute;
    }



}
