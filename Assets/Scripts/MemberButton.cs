using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;

public class MemberButton : MonoBehaviour
{
    private bool memberStatus;//true：未選択　false：選択済み

    private GameObject _playerObj;
    private GameObject _ContentObj;

    //クラスのインスタンス
    private PlayerStatus _playerStatus;
    private GameDirector _gameDirector;

    // Start is called before the first frame update
    void Start()
    {
        memberStatus = true;//生成時は未選択状態にしておく


        _playerObj = transform.root.gameObject;


        _playerStatus = _playerObj.GetComponent<PlayerStatus>();
        _gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //選択時
    public void MemberOnClick()
    {
        if(_playerStatus.GetMembersStatus() && memberStatus)
        {
            _playerStatus.ChangeMembersStatus();
            memberStatus = false;

            transform.DOScale(1f, 0.2f);

            _gameDirector.AddSelectedMember(gameObject);

        };

    }


    
    public void MouseEnterMember()
    {
        
        if (_playerStatus.GetMembersStatus() && memberStatus)
        {
            
        }
    }

    public void MouseExitMember()
    {
        if (_playerStatus.GetMembersStatus() && memberStatus)
        {
            
        }
    }

    public bool MemberStatus
    {
        get { return memberStatus; }
    }

}
