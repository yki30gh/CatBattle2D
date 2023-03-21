using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;


public class PlayerStatus : MonoBehaviour
{

    [SerializeField] private CharacterConfig.CharacterName[] members = new CharacterConfig.CharacterName[4];//セレクトされたメンバーのキャラ名リスト
    [SerializeField] private GameObject[] memberObjects = new GameObject[4];//メンバーのカードオブジェクト
    [SerializeField] private GameObject HPObject;//メンバーのHPオブジェクト


    private bool playerStatus=false;//true：プレイヤーのターン false：相手プレイヤーのターン
    private bool membersStatus = false;//true：メンバ選択中 false：カード選択中

    public enum Player
    {
        _1P=0,
        _2P=1
    }


    private CharacterConfig _characterConfig;
    private AttributeConfig _attributeConfig;

    private List<CharacterConfig.CharacterMap> memberMaps = new List<CharacterConfig.CharacterMap>();//セレクトされたメンバー情報

    private int PartyMaxHP=0;


    


    // Start is called before the first frame update
    void Awake()
    {
        _characterConfig = GameObject.Find("CharacterMap").GetComponent<CharacterConfig>();
        _attributeConfig = GameObject.Find("AttributeMap").GetComponent<AttributeConfig>();

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
                PartyMaxHP += memberMaps[i].getHP();


            }
            
        }

        //チームHPの各オブジェクトを取得
        GameObject _hpParaObj = HPObject.transform.Find("HPPara").gameObject;
        _hpParaObj.GetComponent<Text>().text = PartyMaxHP + "/" + PartyMaxHP;




    }

    private void Start()
    {
      

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    ///ステータス管理メソッド
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
    }

    public bool GetMembersStatus()
    {
        return membersStatus;
    }


    //
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
}
