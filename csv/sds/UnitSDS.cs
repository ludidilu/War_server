﻿public class UnitSDS : CsvBase, IUnitSDS
{
    public int unitType;
    public double moveSpeed;
    public double radius;
    public int weight;
    public double queuePos;
    public int attackDamage;
    public double attackRange;
    public double attackStep;
    public int hp;
    public double visionRange;
    public int[] targetType;
    public int attackType;
    public double attackTypeData;
    public bool isHero;
    public int prize;
    public int skill;
    public int spawnSkill;

    private UnitType[] targetTypeFix;

    public UnitType GetUnitType()
    {
        return (UnitType)unitType;
    }

    public double GetMoveSpeed()
    {
        return moveSpeed;
    }

    public double GetRadius()
    {
        return radius;
    }

    public int GetWeight()
    {
        return weight;
    }

    public double GetQueuePos()
    {
        return queuePos;
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }

    public double GetAttackRange()
    {
        return attackRange;
    }

    public double GetAttackStep()
    {
        return attackStep;
    }

    public int GetHp()
    {
        return hp;
    }

    public double GetVisionRange()
    {
        return visionRange;
    }

    public UnitType[] GetTargetType()
    {
        return targetTypeFix;
    }

    public UnitAttackType GetAttackType()
    {
        return (UnitAttackType)attackType;
    }

    public double GetAttackTypeData()
    {
        return attackTypeData;
    }

    public bool GetIsHero()
    {
        return isHero;
    }

    public int GetPrize()
    {
        return prize;
    }

    public int GetSkill()
    {
        return skill;
    }

    public int GetSpawnSkill()
    {
        return spawnSkill;
    }

    public override void Fix()
    {
        targetTypeFix = new UnitType[targetType.Length];

        for (int i = 0; i < targetType.Length; i++)
        {
            targetTypeFix[i] = (UnitType)targetType[i];
        }
    }
}
