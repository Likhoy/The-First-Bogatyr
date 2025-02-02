using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor; // Нужно для работы со сценами

public class TilemapDrawer : MonoBehaviour
{
    public Tilemap tilemap; // Ссылка на Tilemap
    public TileBase tile;       // Тайл, который вы хотите использовать
    public Vector2Int areaSize; // Размер области в тайлах

    void Start()
    {
        DrawRectangle(new Vector3Int(0, 0, 0), areaSize);
        SaveCurrentScene();
    }

    void DrawRectangle(Vector3Int start, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y > size.y; y--)
            {
                tilemap.SetTile(new Vector3Int(start.x + x, start.y + y, start.z), tile);
            }
        }
    }

    void SaveCurrentScene()
    {
        // Получаем путь к активной сцене
        string scenePath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;

        // Сохраняем сцену
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("Сцена сохранена: " + scenePath);
    }
}
