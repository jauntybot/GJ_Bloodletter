using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class EnemyThreshold : ActionNode
{

    public enum EnemyVar { EnergyLevel, DetectionLevel, DetectionDelta, TerrorLevel, LethalityLevel };
    public EnemyVar enemyVar;
    public enum Operator { LessThan, LessThanEqual, Equal, GreaterThan, GreaterThanEqual };
    public Operator op;
    public float value;


    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        var _var = 0f;
        switch(enemyVar) {
            case EnemyVar.EnergyLevel:
                _var = context.enemy.energyLevel;
            break;
            case EnemyVar.DetectionLevel:
                _var = context.enemy.detectionLevel;
            break;
            case EnemyVar.DetectionDelta:
                _var = context.enemy.detectionDelta;
            break;
            case  EnemyVar.TerrorLevel:
                _var = context.enemy.director.terrorLevel;
            break;
            case EnemyVar.LethalityLevel:
                _var = context.enemy.director.hostilityLevel;
            break;
        }
        switch (op) {
            case Operator.LessThan:
                if (_var < value) return State.Success;
            break;
            case Operator.LessThanEqual:
                if (_var <= value) return State.Success;
            break;
            case Operator.Equal:
                if (_var == value) return State.Success;
            break;
            case Operator.GreaterThanEqual:
                if (_var >= value) return State.Success;
            break;
            case Operator.GreaterThan:
                if (_var > value) return State.Success;
            break;


        }
        return State.Failure;
    }
}
