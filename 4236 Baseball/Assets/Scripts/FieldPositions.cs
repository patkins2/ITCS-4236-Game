using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPositions : MonoBehaviour {

    public bool positionOccupied = false;
    public bool isBatter;
    public bool isPitcher;

    private void Awake() {
        positionOccupied = false;
    }

}
