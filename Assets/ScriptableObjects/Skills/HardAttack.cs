using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HardAttack : Skill
{
    public override void TryGetCost(Player player)
    {
        Dice[] dices = player.Dices;

    }
   
    public override void Start( )
    {
        
    }


    public override void OnActiveted()
    {
        throw new System.NotImplementedException();
    }
}
