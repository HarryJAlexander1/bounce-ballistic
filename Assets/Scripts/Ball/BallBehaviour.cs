using System;
using Assets.Scripts.Level;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private BallManager _ballManager;

    // Start is called before the first frame update
    void Start()
    {
        var levelGenerator = FindObjectOfType<LevelGenerator>();

        if (levelGenerator != null)
            _ballManager = levelGenerator.GetComponent<BallManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            InitiateDeath();
        }
    }

    private void InitiateDeath()
    {
        if (_ballManager != null)
        {
            _ballManager.RemoveBall(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
