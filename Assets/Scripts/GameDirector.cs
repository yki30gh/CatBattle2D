using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEngine.UIElements;

public class GameDirector : MonoBehaviour
{
    private int[] cards = new int[32];//３２枚のカードを格納
    private int turnCount = 0;
    private int selectCount = 0;


    [SerializeField] private PlayerStatus.Player startP;
    private PlayerStatus[] _playerStatus= new PlayerStatus[2];
    private PlayerTurnStatus[] _playersTurnStatus = new PlayerTurnStatus[2];


    private CardDirector _cardDirector;
    private AttributeConfig _attributeConfig;

    private void Awake()
    {
        CardsReset();
    }


    // Start is called before the first frame update
    void Start()
    {

        _cardDirector = GameObject.Find("Cards").GetComponent<CardDirector>();
        _attributeConfig= GameObject.Find("AttributeMap").GetComponent<AttributeConfig>();

        _playerStatus[0] = GameObject.Find("1P").GetComponent<PlayerStatus>();
        _playerStatus[1] = GameObject.Find("2P").GetComponent<PlayerStatus>();

        

        SetTurn();

    }

    


    //カードを初期化する
    private void CardsReset()
    {
        int n = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                cards[n] = (j + 1) * 10 + i + 1;
                n++;
            }
        }
    }


    //カードを一枚取得
    public int GetCardNum()
    {
        int index;
        int Num=0;
        int count = 0;

        while(Num==0)
        {
            index = UnityEngine.Random.Range(0, 32);
            Num = cards[index];
            cards[index] = 0;
            count++;
            if(count==31)
            {
                CardsReset();
                count = 0;
            }
        }
        return Num;
    }



    private class PlayerTurnStatus
    {

        //メンバ変数取得メソッド

        public int SelectCount
        {
            get { return selectCount; }
        }
        public GameObject[] Members
        {
            get { return members; }
        }
        public GameObject[] Cards
        {
            get { return cards; }
        }
        public int[] CardNumbers
        {
            get { return cardNumbers; }
        }
        

        ///メンバ変数
        private int selectCount=0;

        private int[] cardNumbers = new int[4];

        private GameObject[] members = new GameObject[4];

        private GameObject[] cards = new GameObject[4];


        ///メンバメソッド
        public void AddSelectedMember(int turn, GameObject obj)//メンバーオブジェクトを登録
        {
            this.members[turn] = obj;
        }

        public void AddSelectedCard(int turn, GameObject obj)//カードオブジェクト・カード数字を登録
        {
            this.cards[turn] = obj;

            GameObject _frontObj = obj.transform.GetChild(1).gameObject;
            this.cardNumbers[turn] = int.Parse(_frontObj.transform.GetChild(1).gameObject.GetComponent<Text>().text);
            
        }

        public void RegCardNumber(int turn,int num)
        {
            this.cardNumbers[turn] = num;
        }

        public void AddSelectCount()
        {
            this.selectCount++;
        }


        public float CardNumberSum()
        {
            float attackPower=0;
            if (this.selectCount == 4)
            {
                for (int i = 0; i < selectCount; i++)
                {
                    attackPower += this.cardNumbers[i];
                }
            }
            return attackPower;
        }


    }






    //新しいターンの準備をする
    public void SetTurn()
    {
        _cardDirector.CardsGenerate();//カード生成
        turnCount++;//ターン数カウント
        selectCount = 0;//両プレイヤーカード選択回数（最大７）
        
    }

    //新しいターンを開始する
    public void TurnStart()
    {
        for (int i = 0; i < 2; i++)
        {
            _playerStatus[i].SetStartPlayer(startP);//スタートプレイヤーのステータスをtrueに変更
            _playersTurnStatus[i] = new PlayerTurnStatus();//このターンのプレイヤー管理のインスタンスを生成

        }

        _cardDirector.ChangeCardStatus();//カードを選択可能状態にする

        
    }


    //プレーヤーの行動を開始する
    public void PlayerTurnActive()
    {
        foreach (var player in _playerStatus)
        {
            if(player.GetPlayerStatus())
            {
                player.ChangeMembersStatus();//メンバーを選択可能にする
            }
        }
    }

    //プレイヤーが選択したカードオブジェクトを登録
    public void AddSelectedCardInfo(GameObject cardObj)
    {
        for (int i = 0; i < 2; i++)
        {
            if(_playerStatus[i].GetPlayerStatus())
            {
                int count = _playersTurnStatus[i].SelectCount;
                _playersTurnStatus[i].AddSelectedCard(count, cardObj);

            };
        }

        if(selectCount==6)
        {
            GameObject leftMemberObj;
            for (int i = 0; i < 2; i++)
            {
                if (_playerStatus[i].GetPlayerStatus())
                {
                    
                    leftMemberObj = _playerStatus[i].FindLeftMember(_playersTurnStatus[i].Members);
                    

                    int count = _playersTurnStatus[i].SelectCount;
                    _playersTurnStatus[i].AddSelectedMember(count, leftMemberObj);

                    StartCoroutine(LastCardAutoMove());
                };
            }
        }
        else
        {
            PlayerTurnActive();//メンバーのセレクトへ
        }
        
        selectCount++;
    }



    //プレイヤーが選択したキャラオブジェクトを登録しカードを移動
    public void AddSelectedMember(GameObject memberObj)
    {
        for (int i = 0; i < 2; i++)
        {
            if (_playerStatus[i].GetPlayerStatus())
            {

                //選択したオブジェクトを登録
                int count = _playersTurnStatus[i].SelectCount;
                _playersTurnStatus[i].AddSelectedMember(count, memberObj);

                
            
                CardMoveToMember();
            };

        }


    }

    //カードを対象メンバーへ移動
    public void CardMoveToMember()
    {
        for (int i = 0; i < 2; i++)
        {
            if (_playerStatus[i].GetPlayerStatus())
            {
                int count = _playersTurnStatus[i].SelectCount;
                GameObject cardObj = _playersTurnStatus[i].Cards[count];//選択したカード
                GameObject memberObj= _playersTurnStatus[i].Members[count];//選択したメンバー

                var _attriConf = GameObject.Find("AttributeMap").GetComponent<AttributeConfig>();

                //同属性：数字２倍　不利属性：数字半分
                
                int power = _playersTurnStatus[i].CardNumbers[count];//属性を考慮したカード番号
                int relate=0;//カードとキャラの関係性　0:通常　1:有利　2:不利

                if (cardObj.tag==memberObj.tag)
                {
                    power *= 2;
                    relate = 1;
                }
                else if(_attriConf.FindPositiveAttibute(memberObj.tag)==cardObj.tag)
                {
                    power /= 2;
                    relate = 2;
                }

                
                _playersTurnStatus[i].RegCardNumber(count, power);

                _playersTurnStatus[i].AddSelectCount();

                StartCoroutine(ChangeCardNumberCroutine(power, cardObj,relate));

                //カードを移動
                Vector3 posMember = memberObj.GetComponent<RectTransform>().position;
                posMember.x += 250;
                posMember.y +=70;

                RectTransform cardRect = cardObj.GetComponent<RectTransform>();

                Sequence sequence = DOTween.Sequence()
                     .Append(cardRect.DOMove(posMember, 0.5f))
                     .Join(cardRect.DOScale(new Vector3(0.6f, 0.60f, 0), 0.5f))
                     .OnComplete(()=>
                     { 
                         cardObj.transform.SetParent(memberObj.transform);
                     });
                
                sequence.Play();

            };
        }

        


    }

    //カード番号を更新
    private IEnumerator ChangeCardNumberCroutine(int power,GameObject card,int relate)
    {
        yield return new WaitForSeconds(0.3f);

        var _numberObj=card.transform.Find("CardFront/Number").gameObject;
        
        var rect = _numberObj.GetComponent<RectTransform>();

        if(relate==1)
        {
            var sequence = DOTween.Sequence();

            sequence.Append(rect.DOScale(1.5f, 0.5f))
                    .Append(rect.DORotate(new Vector3(0, 0, -360), 0.4f, RotateMode.FastBeyond360))
                    .Append(rect.DOScale(1.0f, 0.2f));

            sequence.Insert(0.6f, _numberObj.GetComponent<Text>().DOText(power.ToString(), 0.05f));
        }
        else if(relate==2)
        {
            var sequence = DOTween.Sequence();

            sequence.Append(rect.DOScale(0.6f, 0.5f))
                    .Append(rect.DORotate(new Vector3(0, 0, -360), 0.4f, RotateMode.FastBeyond360))
                    .Append(rect.DOScale(1.0f, 0.2f));

            sequence.Insert(0.6f, _numberObj.GetComponent<Text>().DOText(power.ToString(), 0.05f));
        }
        

        if (selectCount != 0) StartCoroutine(ChangeActivePlayerCoroutine());

    }



    //カードが配置されたらアクティブプレイヤーをチェンジする
    private IEnumerator ChangeActivePlayerCoroutine()
    {
        yield return new WaitForSeconds(2f);

        //プレイヤーステータスチェンジ
        for (int i = 0; i < 2; i++)
        {
            _playerStatus[i].ChangePlayerMove();
            _playerStatus[i].ChangePlayerStatus();
            
        }
    }

    public void CardChangeActive()
    {
        if (selectCount == 7)//カードが残り１枚になった時
        {
            LastSelect();
        }
        else
        {
            _cardDirector.ChangeCardStatus();
        }
    }
        
 


    //ターン内ラストのセレクト
    private void LastSelect()
    {
        //残ったのカードオブジェクトを取得
        GameObject leftCardObj = GameObject.Find("Cards").gameObject.transform.GetChild(1).gameObject;
        leftCardObj.GetComponent<cardButton>().CardTurnOver();

        //残ったメンバーオブジェクトを取得しカードを移動
        GameObject leftMemberObj;
        for (int i = 0; i < 2; i++)
        {
            if (_playerStatus[i].GetPlayerStatus())
            {
                leftMemberObj = _playerStatus[i].FindLeftMember(_playersTurnStatus[i].Members);

                int count = _playersTurnStatus[i].SelectCount;
                _playersTurnStatus[i].AddSelectedCard(count, leftCardObj);
                _playersTurnStatus[i].AddSelectedMember(count, leftMemberObj);

                leftCardObj.GetComponent<RectTransform>().DOAnchorPosY(10f, 0.3f)
                   .SetLoops(3, LoopType.Yoyo);

                StartCoroutine(LastCardAutoMove());

            };
        }

        selectCount++;
    }



    //ラストカード自動セレクト
    private IEnumerator LastCardAutoMove()
    {
        yield return new WaitForSeconds(1f);

        CardMoveToMember();

        //両プレイヤーのセレクトが終わったら攻撃する
        if(selectCount==8)
        {
            int[] attackPower=new int[2];
            for (var i=0;i<2;i++)
            {
                attackPower[i] = (int)_playersTurnStatus[i].CardNumberSum();//攻撃値を取得
                
            }

            StartCoroutine(AttackEnemy(attackPower));

            selectCount = 0;

        }
    }


    //相手に攻撃
    private IEnumerator AttackEnemy(int[] powers)
    {
        yield return new WaitForSeconds(1f);

        for (var i = 0; i < 2; i++)
        {
            int j=(i == 0) ? j = 1 : j = 0;
            float rate = _playerStatus[j].AfterHPWidth(powers[i]);
            _playerStatus[j].OnDamage(rate);

        }


    }





}
