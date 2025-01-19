#if UNITY_EDITOR
using UnityEditor.Tilemaps;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "BrushPrefab", menuName = "BrushPrefab")]
#if UNITY_EDITOR
[CustomGridBrush(false, true, false, "BrushPrefab")]
public class NewMonoBehaviourScript : GameObjectBrush
#else
public class NewMonoBehaviourScript : MonoBehaviour
#endif
{

#if UNITY_EDITOR
    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        base.Erase(gridLayout, brushTarget, position);
    }
#endif

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
