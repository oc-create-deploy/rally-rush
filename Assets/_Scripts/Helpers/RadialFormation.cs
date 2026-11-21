using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RadialFormation : FormationBase {
    public int amount = 1;
    public float radius = 0.5F;
    public float radiusGrowthMultiplier = 0;
    public float rotations = 1;
    public int rings = 1;
    public float ringOffset = 1;
    public float nthOffset = 0;

    public override IEnumerable<Vector3> EvaluatePoints() {
        int ringCount=Mathf.Max(1,rings);
        int safeAmount=Mathf.Max(0,amount);
        float ringOffset = 0f;
        for (int i = 0; i < ringCount; i++) {
            int amountPerRing=safeAmount/ringCount+(i<safeAmount%ringCount ? 1 : 0);
            for (int j = 0; j < amountPerRing; j++) {
                float angle = j * Mathf.PI * (2 * rotations) / amountPerRing + (i % 2 != 0 ? nthOffset : 0);

                float radius = this.radius + ringOffset + j * radiusGrowthMultiplier;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                Vector3 pos = new(x, -1, z);

                pos += GetNoise(pos);

                pos *= Spread;
                
                yield return pos;
            }

            ringOffset += this.ringOffset;
        }
    }
}
