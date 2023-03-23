using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;
using DG.Tweening;


public class PlayerStatus : MonoBehaviour
{

    [SerializeField] private CharacterConfig.CharacterName[] members = new CharacterConfig.CharacterName[4];//セレクトされたメンバーのキャラ名リスト
    [SerializeField] private GameObject[] memberObjects = new GameObject[4];//メンバーのカードオブジェクト
    [SerializeField] private GameObject HPObject;//メンバーのHPオブジェクト
    [SerializeField] private GameObject membersObj;


    private bool playerStatus=false;//true：プレイヤーのターン false：相手プレイヤーのターン
    private bool membersStatus = false;//true：メンバ選択中 false：カード選択中

    public enum Player
    {
        _1P=0,
        _2P=1
    }


    private CharacterConfig _characterConfig;
    private AttributeConfig _attributeConfig;
    private GameDirector _gameDirector;

    private List<CharacterConfig.CharacterMap> memberMaps = new List<CharacterConfig.CharacterMap>();//セレクトされたメンバー情報

    private int partyMaxHP=0;
    private int partyHP = 0;
    Text hpParaText;

    RectTransform rect;





    // Start is called before the first frame update
    void Awake()
    {
        //インスタンス生成
        rect = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        _characterConfig = GameObject.Find("CharacterMap").GetComponent<CharacterConfig>();
        _attributeConfig = GameObject.Find("AttributeMap").GetComponent<AttributeConfig>();
        _gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        //プレイメンバーのキャラクタークラスを取得
        for (int i = 0; i < members.Length; i++)
        {
            if (i<4)
            {
                //メンバーカードの各オブジェクトを取得
                GameObject _memberObject = memberObjects[i];
                GameObject _backGroundObj = _memberObject.transform.Find("MemberBackGround").gameObject;
                GameObject _characterObj = _backGroundObj.transform.Find("MemberCharacter").gameObject;
                GameObject _markObj = _backGroundObj.transform.Find("MemberMark").gameObject;


                memberMaps.Add(_characterConfig.GetPlayerMap(members[i]));//メンバー情報を取得しメンバーリストに追加

                //メンバーの属性を取得
                var memberAttribute = memberMaps[i].getAttribute();
                memberObjects[i].tag = memberAttribute.ToString();//タグを設定

                //セレクトに合わせてメンバーの画像をセット
                _backGroundObj.GetComponent<Image>().sprite = _attributeConfig.GetBackGroundSprite(memberAttribute);
                _characterObj.GetComponent<Image>().sprite = memberMaps[i].getSprite();
                _markObj.GetComponent<Image>().sprite = _attributeConfig.GetMarkSprite(memberAttribute);


                //チームのHPを計算
                partyMaxHP += memberMaps[i].getHP();
               


            }
            
        }

        //チームHPの各オブジェクトを取得
        hpParaText = HPObject.transform.Find("HPPara").gameObject.GetComponent<Text>();
        hpParaText.text = partyMaxHP + "/" + partyMaxHP;

        partyHP = partyMaxHP;




    }

    private void Start()
    {
      

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /*****************ステータス管理********************/
    public void SetStartPlayer(Player player)
    {
        if(this.tag==player.ToString())
        {
            playerStatus = true;
        }
        else
        {
            playerStatus = false;
        }
    }


    public void ChangePlayerStatus()
    {
        playerStatus = !playerStatus;
    }

    public bool GetPlayerStatus()
    {
        return playerStatus;
    }

    public void ChangeMembersStatus()
    {
        membersStatus = !membersStatus;


        for (int i = 0; i < 4; i++)
        {
            var member = membersObj.transform.GetChild(i).gameObject;
            bool status = member.GetComponent<MemberButton>().MemberStatus;
            if (status)
            {
                if (membersStatus)
                {
                    member.GetComponent<RectTransform>().DOScale(1.08f, 0.5f)
                        .OnComplete(() =>
                        {
                            member.GetComponent<RectTransform>().DOScale(1.06f, 0.5f)
                            .SetLoops(-1, LoopType.Yoyo);
                        });
                }
                else
                {
                    member.GetComponent<RectTransform>().DOKill();
                    member.GetComponent<RectTransform>().DOScale(1.0f, 0.5f);
                }

            }
        }

    }

    public bool GetMembersStatus()
    {
        return membersStatus;
    }

    /*************************************/


    //不利属性を返す
    public GameObject FindLeftMember(GameObject[] members)
    {
        GameObject targetObj=null;
        foreach (var member in memberObjects)
        {
            if(!members.Contains(member))
            {
                targetObj = member;
                break;
            }
        }
        return targetObj;
    }

    public void ChangePlayerMove()
    {
        

        var panel = transform.GetChild(0).gameObject;
        if (playerStatus)
        {
            panel.transform.DOMoveX(5000, 1.5f)
                .SetRelative(true)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    rect.localScale = new Vector2(0.55f, 0.55f);
                    rect.anchoredPosition = new Vector2(-3500,1100);
                    rect.DOAnchorPos(new Vector2(-850, 1100), 1f)
                    .OnComplete(() =>
                    {
                        _gameDirector.CardChangeActive();
                    });
                    
                });
            
        }
        else
        {
            panel.transform.DOMoveX(-3500, 1.5f)
                .SetRelative(true)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    rect.localScale = new Vector2(1f, 1f);
                    rect.anchoredPosition = new Vector2(5000,100);
                    rect.DOAnchorPos(new Vector2(0,100), 1f);
                });
        }

        
    }
   


    //HPの更新
    public float AfterHPWidth(int power)
    {
        if(partyHP<=power)
        {
            partyHP = 0;

        }
        else
        {
            partyHP -= power;
        }

        
        hpParaText.text = partyHP + "/" + partyMaxHP;

        float HPrate = (float)partyHP / (float)partyMaxHP;
        Debug.Log(power);
        return HPrate;

        
    }

    //ダメージアニメーション
    public void OnDamage(float rate)
    {
        var HP=HPObject.transform.GetChild(0).gameObject;
        
        RectTransform rect = HP.GetComponent<RectTransform>();
        int afterWidth = (int)(rect.sizeDelta.x*rate);
        
        rect.DOSizeDelta(new Vector2(afterWidth, 90), 1f)
            .SetEase(Ease.OutBounce);
    }
}
