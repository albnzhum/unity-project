/***********************************************************/
/**  © 2018 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru/
/**  Подписка на Рatreon: https://www.patreon.com/null_code
/***********************************************************/

using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    [SerializeField] private Grid grid;
    [SerializeField] private int mapWidth = 100;
    [SerializeField] private int mapHeight = 100;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Transform cursor;

    private static Pathfinding inst;
    public static Node[,] map;
    private Camera cam;
    public static Vector2Int Cursor { get; private set; }
    private Vector2Int target;
    private List<Node> nodes = new List<Node>();

    public static Vector3 cellSize { get { return inst.grid.cellSize; } }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(mapWidth * grid.cellSize.x, mapHeight * grid.cellSize.y, 1));
    }

    public static Vector2 CalculatePosition(Vector3 position) // выровнять позицию объекта по центру ячейки Grid
    {
        Vector3Int pos = inst.grid.WorldToCell(position);
        return new Vector2(pos.x * inst.grid.cellSize.x + inst.grid.cellSize.x / 2f, pos.y * inst.grid.cellSize.y + inst.grid.cellSize.y / 2f);
    }

    void Initialize()
    {
        cam = Camera.main;
        map = new Node[mapWidth, mapHeight];
        GlobalMapRefresh();
    }
    
    void GlobalMapRefresh()
    {
        float posX = -grid.cellSize.x * mapWidth / 2f - grid.cellSize.x / 2f;
        float posY = grid.cellSize.y * mapHeight / 2f - grid.cellSize.y / 2f;
        float Xreset = posX;
        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, y] = new Node();
                posX += grid.cellSize.x;
                map[x, y].cost = -1;
                map[x, y].worldPosition = new Vector3(posX, posY, 0);
                
                if (Physics2D.OverlapPoint(new Vector2(posX, posY), wallMask))
                {
                    map[x, y].cost = -2;
                    map[x, y].isStatic = true;
                }

                map[x, y].arrayPosition = new Vector2Int(x, y);
                map[x, y].x = x;
                map[x, y].y = y;
            }

            posY -= grid.cellSize.y;
            posX = Xreset;
        }
    }

    void OnValidate()
    {
        grid = GetComponent<Grid>();
        mapWidth = Mathf.Abs(mapWidth);
        mapHeight = Mathf.Abs(mapHeight);
        mapWidth = GetEvenInteger(mapWidth);
        mapHeight = GetEvenInteger(mapHeight);
        transform.position = Vector3.zero;
    }

    int GetEvenInteger(int value) // преобразовать в четное число
    {
        if (value % 2 == 0) return value; else return value - 1;
    }

    void Awake()
    {
        inst = this;
        Initialize();
    }

    public static Vector2Int ConvertToMap(Vector3Int cellPos)
    {
        return inst.ConvertToMap_inst(cellPos);
    }

    public static Vector2Int ConvertToWorld(Node cell)
    {
        return inst.ConvertToWorld_inst(cell);
    }

    public static Vector2Int WorldToArray(Vector3 current)
    {
        return ConvertToMap(inst.grid.WorldToCell(current));
    }

    Vector2Int ConvertToMap_inst(Vector3Int cellPos) // конвертировать позицию клетки в соответствие позиции элементу 2D массива
    {
        return new Vector2Int(cellPos.x + mapWidth / 2, (-cellPos.y + mapHeight / 2) - 1);
    }

    Vector2Int ConvertToWorld_inst(Node cell) // конвертировать позицию элемента 2D массива в соответствие позиции клетки
    {
        return new Vector2Int(cell.x - mapWidth / 2, -(cell.y - mapHeight / 2) - 1);
    }

    Vector3 CellPosition(Vector3 position) // получаем позицию клетки решетки Grid
    {
        position.z = 0;
        Vector3Int pos = grid.WorldToCell(position);
        target = ConvertToMap(pos);
        return new Vector3(pos.x * grid.cellSize.x + grid.cellSize.x / 2f, pos.y * grid.cellSize.y + grid.cellSize.y / 2f, 0);
    }

    void LateUpdate()
    {
        Vector3 cursorPosition = CellPosition(cam.ScreenToWorldPoint(Input.mousePosition));

        if (target.x >= 0 && target.x < mapWidth && target.y >= 0 && target.y < mapHeight)
        {
            cursor.position = cursorPosition;
            Cursor = target;
        }
    }

    public static List<Node> FindPath(Vector2Int start, Vector2Int end, bool diagonally)
    {
        return inst.FindPath_inst(start, end, diagonally);
    }

    List<Node> FindPath_inst(Vector2Int start, Vector2Int end, bool diagonally)
    {
        int x, y, eX = end.x, eY = end.y, cost = 0, step = 0;
        
        map[start.x, start.y].cost = -1;

        if (start.x - 1 >= 0)
        {
            if (map[start.x - 1, start.y].cost == -2) step++;
        }
        else step++;

        if (start.y - 1 >= 0)
        {
            if (map[start.x, start.y - 1].cost == -2) step++;
        }
        else step++;

        if (start.x + 1 < mapWidth)
        {
            if (map[start.x + 1, start.y].cost == -2) step++;
        }
        else step++;

        if (start.y + 1 < mapHeight)
        {
            if (map[start.x, start.y + 1].cost == -2) step++;
        }
        else step++;

        if (step == 4) return null; // проверка на доступность (например, юнит окружен)

        step = 0;

        if (map[end.x, end.y].isStatic)
        {
            for (int i = 1; i < i + 1; i++)
            {
                if (end.x - i >= 0)
                    if (map[end.x - i, end.y].cost == -1)
                    {
                        eX = map[end.x - i, end.y].x;
                        eY = map[end.x - i, end.y].y;
                        break;
                    }

                if (end.y - i >= 0)
                    if (map[end.x, end.y - i].cost == -1)
                    {
                        eX = map[end.x, end.y - i].x;
                        eY = map[end.x, end.y - i].y;
                        break;
                    }

                if (end.x + i < mapWidth)
                    if (map[end.x + i, end.y].cost == -1)
                    {
                        eX = map[end.x + i, end.y].x;
                        eY = map[end.x + i, end.y].y;
                        break;
                    }

                if (end.y + i < mapHeight)
                    if (map[end.x, end.y + i].cost == -1)
                    {
                        eX = map[end.x, end.y + i].x;
                        eY = map[end.x, end.y + i].y;
                        break;
                    }
            }
        }

        List<Node> result = new List<Node>();
        map[eX, eY].cost = 0; // начало поиска с точки назначения
        result.Clear();
        result.Add(map[eX, eY]);
        nodes.Add(map[eX, eY]);

        while (true)
        {
            for (int i = 0; i < result.Count; i++)
            {
                if (map[result[i].x, result[i].y].cost == step)
                {
                    if (result[i].x - 1 >= 0)
                        if (map[result[i].x - 1, result[i].y].cost == -1)
                        {
                            cost = step + 1;
                            map[result[i].x - 1, result[i].y].cost = cost;
                            result.Add(map[result[i].x - 1, result[i].y]);
                            nodes.Add(map[result[i].x - 1, result[i].y]);
                        }

                    if (result[i].y - 1 >= 0)
                        if (map[result[i].x, result[i].y - 1].cost == -1)
                        {
                            cost = step + 1;
                            map[result[i].x, result[i].y - 1].cost = cost;
                            result.Add(map[result[i].x, result[i].y - 1]);
                            nodes.Add(map[result[i].x, result[i].y - 1]);
                        }

                    if (result[i].x + 1 < mapWidth)
                        if (map[result[i].x + 1, result[i].y].cost == -1)
                        {
                            cost = step + 1;
                            map[result[i].x + 1, result[i].y].cost = cost;
                            result.Add(map[result[i].x + 1, result[i].y]);
                            nodes.Add(map[result[i].x + 1, result[i].y]);
                        }

                    if (result[i].y + 1 < mapHeight)
                        if (map[result[i].x, result[i].y + 1].cost == -1)
                        {
                            cost = step + 1;
                            map[result[i].x, result[i].y + 1].cost = cost;
                            result.Add(map[result[i].x, result[i].y + 1]);
                            nodes.Add(map[result[i].x, result[i].y + 1]);
                        }
                }
            }

            step++;

            if (map[start.x, start.y].cost != -1) break;
            if (step != cost || step > mapWidth * mapHeight) return null;
        }

        result.Clear();

        // начало поиска со старта
        x = start.x;
        y = start.y;

        step = map[x, y].cost;
        result.Add(map[x, y]);
        
        while (x != eX || y != eY)
        {
            if (diagonally)
            {
                if (x - 1 >= 0 && y - 1 >= 0)
                    if (map[x - 1, y - 1].cost >= 0)
                        if (map[x - 1, y - 1].cost < step)
                        {
                            step = map[x - 1, y - 1].cost;
                            result.Add(map[x - 1, y - 1]);
                            x--;
                            y--;
                            continue;
                        }

                if (y - 1 >= 0 && x + 1 < mapWidth)
                    if (map[x + 1, y - 1].cost >= 0)
                        if (map[x + 1, y - 1].cost < step)
                        {
                            step = map[x + 1, y - 1].cost;
                            result.Add(map[x + 1, y - 1]);
                            x++;
                            y--;
                            continue;
                        }

                if (y + 1 < mapHeight && x + 1 < mapWidth)
                    if (map[x + 1, y + 1].cost >= 0)
                        if (map[x + 1, y + 1].cost < step)
                        {
                            step = map[x + 1, y + 1].cost;
                            result.Add(map[x + 1, y + 1]);
                            x++;
                            y++;
                            continue;
                        }

                if (y + 1 < mapHeight && x - 1 >= 0)
                    if (map[x - 1, y + 1].cost >= 0)
                        if (map[x - 1, y + 1].cost < step)
                        {
                            step = map[x - 1, y + 1].cost;
                            result.Add(map[x - 1, y + 1]);
                            x--;
                            y++;
                            continue;
                        }
            }

            if (x - 1 >= 0)
                if (map[x - 1, y].cost >= 0)
                    if (map[x - 1, y].cost < step)
                    {
                        step = map[x - 1, y].cost;
                        result.Add(map[x - 1, y]);
                        x--;
                        continue;
                    }

            if (y - 1 >= 0)
                if (map[x, y - 1].cost >= 0)
                    if (map[x, y - 1].cost < step)
                    {
                        step = map[x, y - 1].cost;
                        result.Add(map[x, y - 1]);
                        y--;
                        continue;
                    }

            if (x + 1 < mapWidth)
                if (map[x + 1, y].cost >= 0)
                    if (map[x + 1, y].cost < step)
                    {
                        step = map[x + 1, y].cost;
                        result.Add(map[x + 1, y]);
                        x++;
                        continue;
                    }

            if (y + 1 < mapHeight)
                if (map[x, y + 1].cost >= 0)
                    if (map[x, y + 1].cost < step)
                    {
                        step = map[x, y + 1].cost;
                        result.Add(map[x, y + 1]);
                        y++;
                        continue;
                    }

            return null;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (map[nodes[i].x, nodes[i].y].cost >= 0) map[nodes[i].x, nodes[i].y].cost = -1;
        }
        
        nodes.Clear();

        return result;
    }
}
