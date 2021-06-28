using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CellScriptServer : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isOcupied = false;
    [SerializeField] private StonesColor ocupiedColor = StonesColor.Empty;
    private bool isSelected = false;
    private void Awake()
    {
        ocupiedColor = StonesColor.Empty;
    }
    public void SetOcupied(bool b, StonesColor color)
    {
        isOcupied = b;
        ocupiedColor = color;
    }
    public bool IsOcupied()
    {
        return isOcupied;
    }
    public bool IsSelected()
    {
        return isSelected;
    }
    public StonesColor WhatIsColor()
    {
        return ocupiedColor;
    }
    public void SetSelection(bool b)
    {
        isSelected = b;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isOcupied);
            stream.SendNext(ocupiedColor);
        }
        else
        {
            isOcupied = (bool)stream.ReceiveNext();
            ocupiedColor = (StonesColor)stream.ReceiveNext();
        }
    }
}
