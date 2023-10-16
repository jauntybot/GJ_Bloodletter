using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class EnemyThreshold : ActionNode
{

    public enum CompareTo { Const, Var };
    public CompareTo compareTo;
    public enum EnemyVar { EnergyLevel, DetectionLevel, DetectionDelta, TerrorLevel, LethalityLevel, Hidden };
    public EnemyVar enemyVar;
    public EnemyVar compareVar;
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
            case EnemyVar.Hidden:
                _var = context.enemy.hidden ? 1 : 0;
            break;
        }
        var _var2 = 0f;
        switch(compareVar) {
            case EnemyVar.EnergyLevel:
                _var2 = context.enemy.energyLevel;
            break;
            case EnemyVar.DetectionLevel:
                _var2 = context.enemy.detectionLevel;
            break;
            case EnemyVar.DetectionDelta:
                _var2 = context.enemy.detectionDelta;
            break;
            case  EnemyVar.TerrorLevel:
                _var2 = context.enemy.director.terrorLevel;
            break;
            case EnemyVar.LethalityLevel:
                _var2 = context.enemy.director.hostilityLevel;
            break;
            case EnemyVar.Hidden:
                _var2 = context.enemy.hidden ? 1 : 0;
            break;
        }
        var _val = (compareTo == CompareTo.Var) ? _var2 : value;
        switch (op) {
            case Operator.LessThan:
                if (_var < _val) return State.Success;
            break;
            case Operator.LessThanEqual:
                if (_var <= _val) return State.Success;
            break;
            case Operator.Equal:
                if (_var == _val) return State.Success;
            break;
            case Operator.GreaterThanEqual:
                if (_var >= _val) return State.Success;
            break;
            case Operator.GreaterThan:
                if (_var > _val) return State.Success;
            break;


        }
        return State.Failure;
    }
}