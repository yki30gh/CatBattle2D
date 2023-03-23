using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;


public class cardButton : MonoBehaviour
{
    GameObject _backObj;
    GameObject _frontObj;
    private CardDirector _cardDirector;
    private GameDirector _gameDirector;

    private bool cardStatus;//true：未選択 false：選択済み
    RectTransform rect;



    

    // Start is called before the first frame update
    void Start()
    {
        cardStatus = true;
        rect = GetComponent<RectTransform>();

        _cardDirector = GameObject.Find("Cards").GetComponent<CardDirector>();
        _gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();


        _backObj = gameObject.transform.GetChild(0).gameObject;
        _frontObj = gameObject.transform.GetChild(1).gameObject;
        _frontObj.transform.Rotate(new Vector3(0, 90, 0));

        

    }

    // Update is called once per frame
    void Update()
    {
            
            
        
        
    }

    //カードを裏返す
    public void CardOnClick()
    {
        if (_cardDirector.GetCardStatus() && cardStatus)
        {

            CardTurnOver();

            

            _cardDirector.ChangeCardStatus();//false

            _gameDirector.AddSelectedCardInfo(this.gameObject);//選択したカードオブジェクトを登録
        }
        
    }


    public void MouseEnterCard()
    {
        if (_cardDirector.GetCardStatus() && cardStatus)
        {
            _backObj.transform.DOScale(1.1f, 0.2f);
            _frontObj.transform.DOScale(1.1f, 0.2f);
        }
    }

    public void MouseExitCard()
    {
        if (_cardDirector.GetCardStatus() && cardStatus)
        {
            _backObj.transform.DOScale(1f, 0.2f);
            _frontObj.transform.DOScale(1f, 0.2f);
        }
    }

    //カード裏返し
    public void CardTurnOver()
    {
        _backObj.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.1f)
            .OnComplete(() =>
            {
                _backObj.SetActive(false);
                _frontObj.SetActive(true);
                _frontObj.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.1f);
            });

        cardStatus = false;//選択済みにする
    }

}
