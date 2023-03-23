using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;


public class CardDirector : MonoBehaviour
{
    [SerializeField] private GameObject cardButton;
    private GameObject[] cardButtons = new GameObject[8];

    private GameDirector _gameDirector;
    private AttributeConfig _attributeConfig;

    private bool cardsStatus;//true:選択中　false：停止中

    RectTransform rect;





    // Start is called before the first frame update
    void Awake()
    {
        cardsStatus = false;
        rect = GetComponent<RectTransform>();
        _gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        _attributeConfig = GameObject.Find("AttributeMap").GetComponent<AttributeConfig>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CardsGenerate()
    {
        int _spaceWide = 450;
        int _posX = 0;
        float delayTime = 0f;
        float delaTimeSpeed = 0.5f;


        GameObject _parentObj = cardButton.transform.parent.gameObject;

        


        GameObject _frontObj;
        GameObject _numberObj;
        GameObject _edgeObj;

        
        for (int i = 0; i < 8; i++)
        {
            //カードを生成して配列に格納
            cardButtons[i] = Instantiate(cardButton);
            cardButtons[i].transform.SetParent(_parentObj.transform);

            var rect = cardButtons[i].GetComponent<RectTransform>();
            //初期位置
            rect.anchoredPosition = new Vector2(3800+_posX, 0);


            _frontObj = cardButtons[i].transform.GetChild(1).gameObject;
            _edgeObj = _frontObj.transform.GetChild(0).gameObject;
            _numberObj = _frontObj.transform.GetChild(1).gameObject;

            

            int _num = _gameDirector.GetCardNum();
            var attribute = _attributeConfig.GetCardAttribute(_num);


            cardButtons[i].tag = attribute.name.ToString();
            _edgeObj.GetComponent<Image>().color = attribute.colorRGB;
            _numberObj.GetComponent<Text>().text = (_num%10).ToString();

            
            //配置を終えたらオブジェクトを表示する
            cardButtons[i].SetActive(true);

            //カードを指定位置に移動
            rect.DOAnchorPos(new Vector2(_posX, 0), 1f)
            .SetEase(Ease.OutQuad)
            .SetDelay(delayTime);

            _posX += _spaceWide;
            delayTime += 0.4f*delaTimeSpeed;

            


        }

        StartCoroutine(TurnStartCoroutine());
    }

    private IEnumerator TurnStartCoroutine()
    {
        yield return new WaitForSeconds(2.0f);

        _gameDirector.TurnStart()
;    }



    //カードのステータスを変更
    public void ChangeCardStatus()
    {
        cardsStatus = !cardsStatus;

        

        if (!cardsStatus)
        {
            rect.DOKill();
            rect.DOLocalMoveY(10f, 0.3f)
            .SetRelative(true)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(()=>
            {
                rect.DOLocalMoveY(0f, 0.3f)
                .SetRelative(true)
                .SetEase(Ease.OutCubic);
            });
            
        }
        else
        {
            rect.DOLocalMoveY(10f, 0.3f)
                .SetRelative(true)
            .SetLoops(-1, LoopType.Yoyo);
        }
    }

    


    //カードのステータスを返す
    public bool GetCardStatus()
    {
        return cardsStatus;
    }

    public void CardStateAnime()
    {
        
    }
}
