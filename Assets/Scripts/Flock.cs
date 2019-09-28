using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock
{
    List<Boid> boidList;
    public Flock()
    {
        boidList = new List<Boid>();
    }



    public void Run(Vector3 ap)
    {
        foreach(Boid b in boidList)
        {
            b.Run(boidList, ap);
        }
    }

    public void AddBoid(Boid b)
    {
        boidList.Add(b);
    }

    public void UpdateSeparation(float val)
    {
        foreach (Boid b in boidList)
            b.SepMod(val);
    }

    public void UpdateAlignment(float val)
    {
        foreach (Boid b in boidList)
            b.AliMod(val);
    }

    public void UpdateCohesion(float val)
    {
        foreach (Boid b in boidList)
            b.CohMod(val);
    }

    

    public Vector3 GetForces(int id)
    {
        return boidList[id].GetForces();
    }
      
}
