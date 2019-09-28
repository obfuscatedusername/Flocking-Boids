using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sim : MonoBehaviour
{
    Flock _f;
    int numBoids = 500;
    private Vector3 forces;
    public Text t_NumBoids, t_Sep, t_Coh, t_Ali;
    [SerializeField]private Vector3 ApexPredator;
    void Start()
    {
        _f = new Flock();
        for (int i = 0; i < numBoids; i++)
        {
            _f.AddBoid(new Boid(0f, 0f, "Boid "+i));
        }
        
    }

    private void ForcesUpdate()
    {
        //Update the Separation
        if (Input.GetKeyDown(KeyCode.S))
            _f.UpdateSeparation(forces.x += 0.1f);
        if (Input.GetKeyDown(KeyCode.X))
            _f.UpdateSeparation(forces.x -= 0.1f);

        //Update the Cohesion
        if (Input.GetKeyDown(KeyCode.D))
            _f.UpdateCohesion(forces.y += 0.1f);
        if (Input.GetKeyDown(KeyCode.C))
            _f.UpdateCohesion(forces.y -= 0.1f);


        //Update the Alignment
        if (Input.GetKeyDown(KeyCode.A))
            _f.UpdateAlignment(forces.z += 0.1f);
        if (Input.GetKeyDown(KeyCode.Z))
            _f.UpdateAlignment(forces.z -= 0.1f);
    }

    void Update()
    {
        ApexPredator = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ForcesUpdate();
        forces = _f.GetForces(0);
        t_NumBoids.text = "Boids: " + numBoids;
        t_Sep.text = "Separation: "+forces.x + "f";
        t_Coh.text = "Cohesion: "+forces.y + "f";
        t_Ali.text = "Alignment: "+forces.z + "f";
        _f.Run(ApexPredator);
    }
}
