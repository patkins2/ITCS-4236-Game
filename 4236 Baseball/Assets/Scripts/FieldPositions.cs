using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPositions : MonoBehaviour {

    public bool positionOccupied = false;
    public bool isBatter;
    public bool isPitcher;
    public bool isCatcher;

    private void Awake() {
        positionOccupied = false;
    }

}
