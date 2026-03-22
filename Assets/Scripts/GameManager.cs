using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject LevelPrefab;
    // Start is called before the first frame update
    void Start()
    {
        var level = Instantiate(LevelPrefab, new Vector3(0,0,0), quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
