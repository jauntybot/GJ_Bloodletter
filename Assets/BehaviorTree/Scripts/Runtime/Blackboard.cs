using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KiwiBT {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard {

        public enum LookDir { Forward, Scanning, AtPlayer };
        public LookDir lookDir;
        public Vector3 moveToPosition;
 
        public float waitDur;

        public bool enemyCheck;
    }
}