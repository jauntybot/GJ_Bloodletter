using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KiwiBT {
    public class Wait : ActionNode {
        public enum WaitType { Fixed, BlackBoard };
        public WaitType waitType;

        public float duration = 1;
        float startTime;

        protected override void OnStart() {
            startTime = Time.time;
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            float dur = 0f;
            switch (waitType) {
                case WaitType.Fixed:
                    dur = duration;
                break;
                case WaitType.BlackBoard:
                    dur = blackboard.waitDur;
                break;
            }

            if (Time.time - startTime > dur) {
                return State.Success;
            }

            if (Time.time - startTime > context.enemy.energyRegenDelay && context.enemy.energyLevel < 100f)
                context.enemy.energyLevel += context.enemy.energyRegenRate;
            if (context.enemy.energyLevel > 100) context.enemy.energyLevel = 100;

            return State.Running;
        }
    }
}
