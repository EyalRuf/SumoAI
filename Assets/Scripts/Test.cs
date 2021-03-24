using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] bool isRight;
    bool isMoving;
    [SerializeField] Vector3 desiredLocation;
    [SerializeField] Vector3 leftPosition = new Vector3(-1, 0, 0);
    [SerializeField] Vector3 rightPosition = new Vector3(1, 0, 0);

    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, desiredLocation, speed * Time.deltaTime);

            if (transform.position == desiredLocation)
            {
                isMoving = false;
                isRight = !isRight;
                StartCoroutine(Move());
            }
        }
        
        
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(0.1f);

        if (!isRight)
        {
            desiredLocation = leftPosition;
            isMoving = true;
        }
        else
        {
            desiredLocation = rightPosition;
            isMoving = true;
        }
    }
}