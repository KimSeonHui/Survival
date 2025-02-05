﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drone : MonoBehaviour
{
   // private Light flash;
    private Camera droneCam;
    public bool isDroneMoving = false;

    public AudioClip droneMovingSound;
    public AudioClip droneTeleportSound;
    void Start()
    {       
       // flash = gameObject.GetComponent<Light>();
        droneCam = this.GetComponentInChildren<Camera>();

       // flash.type = LightType.Point;
       // flash.range = 0f;
        droneCam.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDroneMoving)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ClientSend.DroneStop(this.gameObject);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (this.transform.parent != null)
                {
                    isDroneMoving = false;
                    PlayerController _player = this.transform.parent.GetComponent<PlayerController>();
                    transform.SetParent(null, true);
                    ClientSend.SkillTeleportation(this.transform.position);
                    this.GetComponent<AudioSource>().PlayOneShot(droneTeleportSound);

                    droneCam.gameObject.SetActive(false);
                    _player.enabled = true;
                    _player.camTransform.gameObject.SetActive(true);
                    _player.GetComponent<PlayerManager>().playerItem.item_number1 = null;
                    _player.GetComponent<PlayerManager>().playerItem.GrabItem = null;
                    Destroy(this.gameObject, 2f);

                    UIManager.instance.itemImageUI[0].sprite = UIManager.instance.itemGrayImage
                            [(int)GameManager.players[Client.instance.myId].playerType * UIManager.instance.itemImageUI.Length];
                    UIManager.instance.itemCountText[0].text = "0";
                    UIManager.instance.itemCountText[0].color = UIManager.instance.textColor[(int)UIManager.TextColor.GRAY];
                }
            }
        }
    }
    public void Moving()
    {
        if (this.transform.parent != null)
        {
            this.transform.parent.GetComponent<PlayerController>().enabled = false;
            this.transform.parent.GetComponent<PlayerController>().camTransform.gameObject.SetActive(false);
            this.transform.position += new Vector3(0f, 5f, 0f);
            droneCam.gameObject.SetActive(true);
            isDroneMoving = true;            
        }
        else
        {
            //Debug.Log($"Error - parent null");
        }
        /*player.transform.GetChild(0).GetComponent<Camera>().gameObject.SetActive(false);
        player.GetComponent<PlayerController>().enabled = false;
        gameObject.transform.position += new Vector3 (0f, 5f, 0f);
        droneCam.gameObject.SetActive(true);       
        isDroneMoving = true;*/
    }

    public void Stop()
    {
        isDroneMoving = false;
        droneCam.gameObject.SetActive(false);
        this.transform.parent.GetChild(0).GetComponent<Camera>().gameObject.SetActive(true);
        this.transform.parent.GetComponent<PlayerController>().enabled = true;

        transform.SetParent(null, true);
    }
  
    private void FixedUpdate()
    {
        if (isDroneMoving)
        {
            bool[] _inputs = new bool[]
            {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
            };            
            ClientSend.DroneMovement(_inputs, this.gameObject);
        }        
    }

    public void OnFlash()
    {
       // flash.range = 8f;
    }
}
