using Battle.Ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatTarget : MonoBehaviour {
    private BattleAI ai;

    public RetreatTarget(BattleAI Ai)
    {
        this.ai = Ai;
    }

    public void SetRetreatTarget ()
    {
        ai.tempTarget = ai.CurrentTarget;
        ai.Retreater.transform.position = ai.retreatDistance * (ai.CurrentTarget.transform.position - ai.transform.position).normalized;
        ai.CurrentTarget = ai.Retreater.transform;
    }
}
