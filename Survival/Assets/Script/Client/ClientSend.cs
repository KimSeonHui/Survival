﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>클라이언트에서 서버로 TCP형태로 패킷전송</summary>
    /// <param name="_packet"></param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>클라이언트에서 서버로 UDP형태로 패킷전송</summary>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }
    
    #region Packets
    /// <summary>welcome메세지를 받고난 후 동작하는 함수 (잘 받았다고 서버로 전송)</summary>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(Client.playerType.ToString());

            SendTCPData(_packet);
        }
    }

    /// <summary>welcome메세지를 받고난 후 동작하는 함수 (잘 받았다고 서버로 전송)</summary>
    public static void GameStart()
    {
        using (Packet _packet = new Packet((int)ClientPackets.gameStart))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(Client.playerType.ToString());

            SendTCPData(_packet);
        }
    }

    /// <summary>player 움직임에 대한 packet UDP전송(주기적으로 전송하기때문에 패킷의 끝을 확인해야함)</summary>
    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach(bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            //주기적으로 보내기때문에 데이터충돌발생확률이 높음 > UDP
            SendUDPData(_packet);
        }
    }
    /// <summary>player 공격에 대한 packet TCP전송(공격할 때 한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    public static void PlayerShootBullet(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShootBullet))
        {
            _packet.Write(_facing);
            _packet.Write(GameManager.EMPInstallFinished);

            SendTCPData(_packet);
        }
    }

    /*
    /// <summary>괴물 공격에 대한 packet TCP전송(공격할 때 한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    public static void CreatureAttack (Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.creatureAttack))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }
    */
    /// <summary>괴물 공격에 대한 packet TCP전송(공격할 때 한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    public static void CreatureAttack(bool _isCreatureAttack)
    {
        using (Packet _packet = new Packet((int)ClientPackets.creatureAttack))
        {
            _packet.Write(_isCreatureAttack);

            SendTCPData(_packet);
        }
    }

    /// <summary>아이템 버리기에 대한 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_facing">버릴 위치</param>
    public static void PlayerShootBomb(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShootBomb))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    /// <summary>아이템 획득에 대한 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_item">획득한 아이템종류</param>
    public static void PlayerGetItem(GameObject _item)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerGetItem))
        {
            _packet.Write(_item.GetComponent<ItemSpawner>().spawnerId);

            SendTCPData(_packet);
        }
    }

    /// <summary>아이템 버리기에 대한 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_item">버릴 아이템종류</param>
    /// <param name="_position">버릴위치</param>
    public static void PlayerThrowItem(ItemSpawner _item, Vector3 _position)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(_item.spawnerId);

            _packet.Write(_position.x);
            _packet.Write(0.5f);
            _packet.Write(_position.z);

            SendTCPData(_packet);
        }
    }

    /// <summary>아이템 들기에 대한 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_key">누른 키</param>
    public static void PlayerGrabItem(int _grabSpawnerId, int _spawnerId, int _key)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerGrabItem))
        {
            _packet.Write(_grabSpawnerId);
            _packet.Write(_spawnerId);
            _packet.Write(_key);

            SendTCPData(_packet);
        }
    }

    /// <summary>아이템 들기에 대한 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_key">누른 키</param>
    public static void PlayerUseItem(int _spawnerId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerUseItem))
        {
            _packet.Write(_spawnerId);

            SendTCPData(_packet);
        }
    }

    /// <summary>EMPZONE에 설치한 EMP packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <summary>설치한 EMP packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_position">플레이어 위치</param>
    /// <param name="_emp">설치하는 emp</param>
    public static void InstallEMP(Vector3 _position, ItemSpawner _emp)
    {
        using (Packet _packet = new Packet((int)ClientPackets.installEMP))
        {
            _packet.Write(_position.x);
            if (_position.y >= 8f)
            {
                _packet.Write(12f);
            }
            else
            {
                _packet.Write(0.5f);
            }
            _packet.Write(_position.z);

            _packet.Write(_emp.spawnerId);
            

            SendTCPData(_packet);
        }
    }

    /// <summary>LightTrap설치 관련정보 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_position">플레이어 위치</param>
    /// <param name="_spawnerId">설치하는 LightTrap ID</param>
    public static void Install(Vector3 _position, int _spawnerId, int _floor)
    {
        using (Packet _packet = new Packet((int)ClientPackets.install))
        {
            _packet.Write(_position.x);
            if (_position.y >= 8f)
            {
                _packet.Write(2.2f);
            }
            else
            {
                _packet.Write(0.2f);
            }
            _packet.Write(_position.z);
            _packet.Write(_spawnerId);
            _packet.Write(_floor);


            SendTCPData(_packet);
        }
    }

    /// <summary>플레이어 체력 회복 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_hp">플레이어 체력</param>
    public static void Cure(float _hp)
    {
        using (Packet _packet = new Packet((int)ClientPackets.cure))
        {
            _packet.Write(_hp);

            SendTCPData(_packet);
        }
    }

    /// <summary>플레이어 은폐 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_hidePlace">은폐하는 오브젝트</param>
    public static void Hide(GameObject _hidePlace)
    {
        using (Packet _packet = new Packet((int)ClientPackets.hide))
        {
            _packet.Write(_hidePlace.transform.position);

            SendTCPData(_packet);
        }
    }

    /// <summary>순간이동 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_target">이동할 위치</param>
    public static void SkillCure(bool _cure)
    {
        using (Packet _packet = new Packet((int)ClientPackets.skillCure))
        {
            _packet.Write(_cure);

            SendTCPData(_packet);
        }
    }

    /// <summary>순간이동 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_target">이동할 위치</param>
    public static void SkillTeleportation(Vector3 _target)
    {
        using (Packet _packet = new Packet((int)ClientPackets.skillTeleportation))
        {
            _packet.Write(_target);

            SendTCPData(_packet);
        }
    }

    /// <summary>드론 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_spawnerId">드론ID</param>
    public static void SkillDrone(int _spawnerId)
    {
        using (Packet _packet = new Packet((int)ClientPackets.skillDrone))
        {
            _packet.Write(_spawnerId);

            SendTCPData(_packet);
        }
    }

    /// <summary>drone 움직임에 대한 packet UDP전송(주기적으로 전송하기때문에 패킷의 끝을 확인해야함)</summary>
    public static void DroneMovement(bool[] _inputs, GameObject _drone)
    {
        using (Packet _packet = new Packet((int)ClientPackets.droneMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(_drone.transform.rotation);

            _packet.Write(_drone.GetComponent<ItemSpawner>().spawnerId);

            //주기적으로 보내기때문에 데이터충돌발생확률이 높음 > UDP
            SendUDPData(_packet);
        }
    }

    /// <summary>드론 조작 멈춤 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_drone">은폐하는 오브젝트</param>
    public static void DroneStop(GameObject _drone)
    {
        using (Packet _packet = new Packet((int)ClientPackets.droneStop))
        {
            _packet.Write(_drone.GetComponent<ItemSpawner>().spawnerId);

            SendTCPData(_packet);
        }
    }

    /// <summary>괴물 이속 증가 packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_player">플레이어</param>
    public static void SpeedUp(PlayerManager _player)
    {
        using (Packet _packet = new Packet((int)ClientPackets.skillSpeedUp))
        {
            _packet.Write(_player.id);
            SendTCPData(_packet);
        }
    }

    /// <summary>keyChange packet TCP전송(한번만 전송하므로 누락이 될지언정 오류가 발생하지는 않음)</summary>
    /// <param name="_player">플레이어</param>
    public static void KeyChange(PlayerManager _player)
    {
        using (Packet _packet = new Packet((int)ClientPackets.keyChange))
        {
            _packet.Write(_player.id);
            SendTCPData(_packet);
        }
    }

    /*
    public static void UDPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.udpTestReceived))
        {
            _packet.Write("Received a UDP packet.");
            SendUDPData(_packet);
        }
    }*/
    #endregion
}

