using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;

    //TODO remove later!
    [Range(0,1)][SerializeField] float movementFactor; // 0 not moved, 1 fully moved.

    Vector3 startingPos;

	// Use this for initialization
	void Start () {
        startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        // set movement factor.
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSinwave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinwave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos - offset;
	}
}
