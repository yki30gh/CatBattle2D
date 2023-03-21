using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Progress;
using UnityEngine.UIElements;

public class GameDirector : MonoBehaviour
{
    private int[] cards = new int[32];//３２枚のカードを格納
    private int turnCount = 0;
    private int selectCount = 0;


    [SerializeField] private PlayerStatus.Player startP;

    private PlayerStatus[] _playerStatus= new PlayerStatus[2];
    private CardDirector _cardDirector;

    private PlayerTurnStatus[] _playersTurnStatus = new PlayerTurnStatus[2];


    private void Awake()
    {
        CardsReset();
    }


    // Start is called before the first frame update
    void Start()
    {

        _cardDirector = GameObject.Find("Cards").GetComponent<CardDirector>();
        _playerStatus[0] = GameObject.Find("1P").GetComponent<PlayerStatus>();
        _playerStatus[1] = GameObject.Find("2P").GetComponent<PlayerStatus>();

        

        SetTurn();

    }

    // Update is called once per frame
    void Update()
    {

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
            index = Random.Range(0, 32);
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

        ///メンバ変数
        private int selectCount=0;


        private GameObject[] members = new GameObject[4];

        private GameObject[] cards = new GameObject[4];


        ///メンバメソッド
        
        public void AddSelectedMember(int turn, GameObject obj)
        {
            this.members[turn] = obj;
        }

        public void AddSelectedCard(int turn, GameObject obj)
        {
            this.cards[turn] = obj;
        }

        public void AddSelectCount()
        {
            this.selectCount++;
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
    public void PlayerTurnStart()
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
        PlayerTurnStart();//メンバーのセレクトへ
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


    public void CardMoveToMember()
    {
        for (int i = 0; i < 2; i++)
        {
            if (_playerStatus[i].GetPlayerStatus())
            {
                int count = _playersTurnStatus[i].SelectCount;
                
                GameObject cardObj = _playersTurnStatus[i].Cards[count];//選択したカード
                GameObject memberObj= _playersTurnStatus[i].Members[count];//選択したメンバー
                

                _playersTurnStatus[i].AddSelectCount();


                Vector3 posMember = memberObj.GetComponent<RectTransform>().position;
                posMember.x += 260;
                posMember.y -= 220;

                RectTransform cardRect = cardObj.GetComponent<RectTransform>();

                //カードを移動
                Sequence sequence = DOTween.Sequence()
                     .Append(cardRect.DOMove(posMember, 0.5f))
                     .Join(cardRect.DOScale(new Vector3(0.6f, 0.60f, 0), 0.5f))
                     .OnComplete(()=>
                     {
                         cardObj.transform.parent = memberObj.transform;
                     });

                sequence.Play();

            };
        }

        if (selectCount != 0) StartCoroutine(ChangeActivePlayerCoroutine());


    }



    //カードが配置されたらアクティブプレイヤーをチェンジする
    private IEnumerator ChangeActivePlayerCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        //プレイヤーステータスチェンジ
        for (int i = 0; i < 2; i++)
        {
            _playerStatus[i].ChangePlayerStatus();
        }

        if (selectCount == 7)//カードが残り１枚になった時
        {
            LastSelect();
        }
        else
        {
            _cardDirector.ChangeCardStatus();
        }
;
    }


    //ターン内ラストのセレクト
    private void LastSelect()
    {
        selectCount = 0;
        GameObject leftCardObj = GameObject.Find("Cards").gameObject.transform.GetChild(1).gameObject;
        leftCardObj.GetComponent<cardButton>().CardTurnOver();
        GameObject leftMemberObj;
        for (int i = 0; i < 2; i++)
        {
            if (_playerStatus[i].GetPlayerStatus())
            {
                leftMemberObj = _playerStatus[i].FindLeftMember(_playersTurnStatus[i].Members);

                int count = _playersTurnStatus[i].SelectCount;
                _playersTurnStatus[i].AddSelectedCard(count, leftCardObj);
                _playersTurnStatus[i].AddSelectedMember(count, leftMemberObj);

                StartCoroutine(LastCardAutoMove());

            };
        }
    }

    //ラストカード自動セレクト
    private IEnumerator LastCardAutoMove()
    {
        yield return new WaitForSeconds(1f);

        CardMoveToMember();

 
    }





}
