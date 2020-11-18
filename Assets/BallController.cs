using System.Collections.Generic;
using NSTools;
using UnityEngine;
using zigzag;

public class BallController : MonoBehaviour
{
    public GameplayController gameplayController;
    
    private CircleCollider2D ballCollider;
    private Camera camera;

    private int ballDirection = -1;

    private float ballSpeed = 1;
    void Start()
    {
        ballCollider = GetComponent<CircleCollider2D>();
        camera = Camera.main;
        var cameraFrom = camera.transform.position;
        EZ.Spawn().Add(0.3f, t => {
            camera.transform.position = Vector3.Lerp(cameraFrom, Vector3.zero, t);
        });
    }
    
    ContactFilter2D contactFilter = new ContactFilter2D().NoFilter();
    void Update()
    {
        if (gameplayController.state == GameState.Play)
        {
            var dx = Vector2.right * 2 * Time.deltaTime * ballDirection;
            var dy = Vector2.up * Time.deltaTime;
            camera.transform.Translate(dy*ballSpeed);
            transform.Translate((dx+dy)*ballSpeed);
            var colliders = new List<Collider2D>();
            if (ballCollider.OverlapCollider(contactFilter, colliders) == 0)
                gameplayController.StateLose();
            foreach (var coll in colliders)
            {
                switch (coll.name)
                {
                    case "finish":
                        gameplayController.StateWin();
                        break;

                    case "path":
                    case "gem":
                        coll.SendMessage("Touch");
                        break;
                    
                }
            }
        }
    }

    public void Turn()
    {
        ballDirection *= -1;
    }

    public void Lose()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -1;
        EZ.Spawn().Add(0.3f, t =>
        {
            transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, t);
            transform.Translate(Vector3.down * t);
        });
    }
}
