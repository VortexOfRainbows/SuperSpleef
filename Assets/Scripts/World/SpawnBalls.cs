using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour
{
    public GameObject ball;
    public GameObject player;
    [SerializeField] private float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 3) 
        { 
            for (int i = 0; i < Random.Range(5f, 20f); i++)
            {
                Instantiate(ball, new Vector3(player.transform.position.x + Random.Range(-2f, 2f), 70f, player.transform.position.z + Random.Range(-2f, 2f)), Quaternion.identity);
            }

            timer = 0;
        }

    }
}
