using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCardController : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform target;
    private float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float step =  speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
         if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform target){
        this.target = target;
    }

    public void SetSpeed(float speed){
        this.speed = speed;
    }
}
