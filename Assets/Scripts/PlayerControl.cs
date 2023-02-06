/***********************************************************/
/**  © 2018 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru/
/**  Подписка на Рatreon: https://www.patreon.com/null_code
/***********************************************************/

using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float speed = 1; // скорость движения персонажа
    public float speedAnimator = 1; // множитель скорости проигрывания клипов анимации
    //public Animator animator;
    public bool diagonallyMove = true; // использовать движение по диагонали
    private Move lastMoved;
    private Vector3 playerPos;
    public bool isMove { get; private set; }
    private List<Node> path = new List<Node>();
    private float delta, magnitude;
    private int moveIndex, lastX, lastY;
    private bool getStop;
    private Vector2Int current, last;

    void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.color = new Color(0, 255, 0, .25f);
                Gizmos.DrawCube(path[i].worldPosition, new Vector3(Pathfinding.cellSize.x, Pathfinding.cellSize.y, 1));
            }
        }
    }

    void Start()
    {
        //animator.SetFloat("speed", speedAnimator);
        playerPos = Pathfinding.CalculatePosition(transform.position);
        transform.position = playerPos;
        lastMoved = Move.Down;

    }
	
	void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isMove)
            {
                playerPos = transform.position;
                BuildPath(Pathfinding.WorldToArray(playerPos), Pathfinding.Cursor, true);

                if (path != null && path.Count > 1)
                {
                    isMove = true;
                    getStop = false;
                }

                current = Pathfinding.Cursor;
            }

            last = Pathfinding.Cursor;
        }

        PlayerMove();
    }

    Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta, out float magnitude)
    {
        Vector3 a = target - current;
        magnitude = a.magnitude;
        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            magnitude = 0;
            return target;
        }
        return current + a / magnitude * maxDistanceDelta;
    }

    void BuildPath(Vector2Int start, Vector2Int end, bool isDelta)
    {
        moveIndex = 1;
        if (isDelta) if (!isMove) delta = .1f; else delta = 0;
        lastX = start.x;
        lastY = start.y;
        path = Pathfinding.FindPath(start, end, diagonallyMove);
    }

    void PlayerMove()
    {
        if (!isMove) return;
        
        transform.position = MoveTowards(transform.position, playerPos, speed * Time.deltaTime, out magnitude);

        if (getStop && magnitude == 0)
        {
            playerPos = transform.position;
            isMove = false;
            return;
        }

        if (magnitude <= delta)
        {
            if (path != null && moveIndex + 1 >= path.Count)
            {
                delta = 0;
            }
            else
            {
                delta = .1f;
            }

            if (path != null && path.Count > 1 && moveIndex < path.Count)
            {
                playerPos = path[moveIndex].worldPosition;

                if (moveIndex + 1 < path.Count)
                {
                    if (lastX != path[moveIndex + 1].x && lastY != path[moveIndex + 1].y)
                    {
                        delta = 0;
                    }
                }

                lastX = path[moveIndex].x;
                lastY = path[moveIndex].y;

                moveIndex++;
            }
            else
            {
                getStop = true;
            }

            if (current != last)
            {
                BuildPath(Pathfinding.WorldToArray(playerPos), last, false);

                if (path == null || path.Count <= 1)
                {
                    getStop = true;
                }

                last = current;
            }
        }
    }

    

}
