using UnityEditor.Tilemaps;
using UnityEngine;


[CreateAssetMenu(fileName = "BrushPrefab", menuName = "BrushPrefab")]
[CustomGridBrush(false, true, false, "BrushPrefab")]
public class NewMonoBehaviourScript : GameObjectBrush
{

    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        base.Erase(gridLayout, brushTarget, position);
    }

    public static Transform GetObjectInCell(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        Transform[] children = brushTarget.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (gridLayout.WorldToCell(child.position) == position)
            {
                return child;
            }
        }
        return null;
    }
}
