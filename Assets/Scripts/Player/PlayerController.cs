using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Level;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject ProjectilePrefab;
    [SerializeField]
    LevelGenerator LevelGenerator;
    // Start is called before the first frame update
    void Start()
    {
        if (LevelGenerator == null)
            LevelGenerator = FindObjectOfType<LevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile();
            Debug.Log("Mouse Clicked at: " + GetMousePosition());
        }
    }

    private void FireProjectile() 
    {
        var projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        Vector2[] perimeterVertices = LevelGenerator != null ? LevelGenerator.GetPerimeterVertices() : new Vector2[0];
        projectile.GetComponent<ProjectileBehaviour>().Activate(GetMousePosition(), perimeterVertices);
    }

    private Vector2 GetMousePosition() 
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
