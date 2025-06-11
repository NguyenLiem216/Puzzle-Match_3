using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintData
{
    public Gem sourceGem;
    public Gem targetGem;
    public int matchCount;

    public HintData(Gem source, Gem target, int count)
    {
        sourceGem = source;
        targetGem = target;
        matchCount = count;
    }
}
