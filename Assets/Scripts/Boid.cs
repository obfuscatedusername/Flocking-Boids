using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid
{
    private Vector2 pos, vel, accel, apLoc;
    private float maxForce, maxSpeed, neighbourDist, desiredSeparation;
    static float MAX_SPEED = 0.0125f;
    GameObject mesh;
    private float w, h, sepMod, aliMod, cohMod;
    private Sprite[] boids;
    private bool Fleeing;
    // Start is called before the first frame update

    public Boid(float x, float y, string name)
    {
        accel = new Vector2(0f, 0f);
        float angle = Random.Range(0, 2*Mathf.PI);
        vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        pos = new Vector2(x, y);
        
        maxSpeed = 0.125f;
        maxForce = 0.02f;
        w = 8.8f;
        h = 5f;
        neighbourDist = 1f;
        desiredSeparation = .75f;
        mesh = new GameObject();
        mesh.name = name;
        mesh.AddComponent<SpriteRenderer>();
        boids = Resources.LoadAll<Sprite>("chev");
        mesh.GetComponent<SpriteRenderer>().sprite = boids[0];//[(int)Random.Range(0.0f, 2.0f)];
        sepMod = 1.5f;
        aliMod = .5f;
        cohMod = .5f;
        Fleeing = false;
    }

    public void SepMod(float val)
    {
        sepMod = val;
    }

    public void AliMod(float val)
    {
        aliMod = val;
    }

    public void CohMod(float val)
    {
        cohMod = val;
    }

    public void Run(List<Boid> boids, Vector3 ap)
    {
        apLoc = new Vector2(ap.x, ap.y);
        Flock(boids);
        BoidUpdate();
        Bounds();
    }

    public void ApplyForce(Vector2 f)
    {
        accel += f;
    }

    public Vector3 GetForces()
    {
        return new Vector3(sepMod, cohMod, aliMod);
    }

    public void Flock(List<Boid> b)
    {
        float fleeRad = 1.5f;
        //Detect if boid should flee, 
        if( Vector2.Distance(apLoc, pos) < fleeRad)
        {
            Fleeing = true;
        }
        else
        {
            Fleeing = false;
        }

        if (Fleeing)
        {
            Vector2 dir = pos - apLoc;
            ApplyForce(Seek(dir.normalized * 3f));
        }else
        //new
        {
            Vector2[] tings = ComputeForces(b);
            tings[0] *= sepMod;
            tings[1] *= aliMod;
            tings[2] *= cohMod;
            ApplyForce(tings[0]);
            ApplyForce(tings[2]);
            ApplyForce(tings[1]);
        }

        //old
        {
            //Vector2 sep = Separate(b);
            //Vector2 coh = Cohesion(b);
            //Vector2 ali = Align(b);
            //sep *= sepMod;
            //ali *= aliMod;
            //coh *= cohMod;
            //ApplyForce(sep);
            //ApplyForce(coh);
            //ApplyForce(ali);
        }
        
    }

    void BoidUpdate()
    {
        vel += accel;
        vel = Vector2.ClampMagnitude(vel, maxSpeed);
        pos += vel;
        accel = new Vector2(0f, 0f);

        mesh.GetComponent<Transform>().position = pos;
        float theta = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        mesh.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, theta));
    }

    //return new Vector2[]{ sep, coh, ali };
    Vector2[] ComputeForces(List<Boid> b)
    {
        Vector2 sep = new Vector2(0f, 0f);
        Vector2 coh = new Vector2(0f, 0f);
        Vector2 ali = new Vector2(0f, 0f);
        int sepCount = 0;
        int aliCount = 0;
        int cohCount = 0;

        foreach (Boid boi in b)
        {
            float d = Vector2.Distance(pos, boi.pos);
            //Separation
            if (d > 0 && d < desiredSeparation)
            {
                Vector2 diff = pos - boi.pos;
                diff.Normalize();
                diff /= d;
                sep += diff;
                sepCount++;
            }
            
            //Alignment
            if (d > 0 && d < neighbourDist)
            {
                ali += boi.vel;
                aliCount++;
            }

            //Cohesion
            if (d > 0 && d < neighbourDist)
            {
                coh += boi.vel;
                cohCount++;
            }
        }

        //separation stuff
        {
            if (sepCount > 0)
                sep /= (float)sepCount;

            if (sep.magnitude > 0)
            {
                sep.Normalize();
                sep *= maxSpeed;
                sep -= vel;
                sep = Vector2.ClampMagnitude(sep, maxForce);
            }
        }

        //cohesion stuff
        {
            if (cohCount > 0)
            {
                coh /= (float)cohCount;
                mesh.GetComponent<SpriteRenderer>().sprite = boids[0];
                coh = Seek(coh);
            }
            else
            {
                mesh.GetComponent<SpriteRenderer>().sprite = boids[1];
                coh = new Vector2(0f, 0f);
            }
        }

        //Alignment stuff
        {
            if (aliCount > 0)
            {
                ali /= (float)aliCount;
                ali.Normalize();
                ali *= maxSpeed;
                Vector2 steer = ali - vel;
                steer = Vector2.ClampMagnitude(steer, maxForce);
            }
            else
            {
                ali = new Vector2(0f, 0f);
            }
        }

        return new Vector2[]{ sep, coh, ali };

    }

    Vector2 Seek(Vector2 target)
    {
        Vector2 des = target - pos;
        des.Normalize();
        des *= maxSpeed;
        Vector2 steer = des - vel;
        steer = Vector2.ClampMagnitude(steer, maxForce);
        return steer;
    }

    //This method wraps the boids if they meet any of the boundaries
    private void Bounds()
    {
        if (pos.x < -w) pos.x = w;
        if (pos.y < -h) pos.y = h;
        if (pos.x > w) pos.x = -w;
        if (pos.y > h) pos.y = -h;
    }

    //Vector2 Separate(List<Boid> b)
    //{
    //    Vector2 steer = new Vector2(0f, 0f);
    //    int count = 0;
    //    foreach(Boid _b in b)
    //    {
    //        float d = Vector2.Distance(pos, _b.pos);
    //        if(d > 0 && d < desiredSeparation)
    //        {
    //            Vector2 diff = pos - _b.pos;
    //            diff.Normalize();
    //            diff /= d;
    //            steer += diff;
    //            count++;
    //        }
    //    }
    //    if (count > 0)
    //        steer /= (float)count;

    //    if(steer.magnitude > 0)
    //    {
    //        steer.Normalize();
    //        steer *= maxSpeed;
    //        steer -= vel;
    //        steer = Vector2.ClampMagnitude(steer, maxForce);
    //    }
    //    return steer;
    //}

    //Vector2 Align(List<Boid> b)
    //{
    //    Vector2 sum = new Vector2(0f, 0f);
    //    int count = 0;
    //    foreach (Boid _b in b)
    //    {
    //        float d = Vector2.Distance(pos, _b.pos);
    //        if(d > 0 && d < neighbourDist)
    //        {
    //            sum += _b.vel;
    //            count++;
    //        }
    //    }
    //    if(count > 0)
    //    {
    //        sum /= (float)count;
    //        sum.Normalize();
    //        sum *= maxSpeed;
    //        Vector2 steer = sum - vel;
    //        steer = Vector2.ClampMagnitude(steer, maxForce);
    //        return steer;
    //    }
    //    else
    //    {
    //        return new Vector2(0f, 0f);
    //    }
    //}

    //Vector2 Cohesion(List<Boid> b)
    //{
    //    Vector2 sum = new Vector2(0f, 0f);
    //    int count = 0;
    //    foreach (Boid _b in b)
    //    {
    //        float d = Vector2.Distance(pos, _b.pos);
    //        if (d > 0 && d < neighbourDist)
    //        {
    //            sum += _b.vel;
    //            count++;
    //        }
    //    }
    //    if (count > 0)
    //    {
    //        sum /= count;
    //        mesh.GetComponent<SpriteRenderer>().sprite = boids[0];
    //        return Seek(sum);
    //    }
    //    else
    //    {
    //        mesh.GetComponent<SpriteRenderer>().sprite = boids[1];
    //        return new Vector2(0f, 0f);
    //    }
    //}
}
