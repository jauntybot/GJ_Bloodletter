using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{

    public enum DoorType { Free, Toll, Blood };
    public DoorType doorType;


    public float cost;


    protected override void Start() {
        base.Start();
    }

    public override void Interact() {



    }


    public void OpenCloseDoor(bool state) {



    }

}
