using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GridGenerator : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
    private int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateGridShape();
        CreateGridMesh();
    }

    void CreateGridShape()
    {
        int thoughtSectionGridSize = 9;
        int cellXCount = 0;
        int cellZCount = 0;

        vertices = new List<Vector3>();

        for (int z=0; z <= zSize; z++)
        {
            for (int x=0; x <= xSize; x++)
            {
                if (cellXCount == 0 && cellZCount == 0)
                {
                    vertices.Add(new Vector3(x, 0, z));
                    var gridDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    gridDot.transform.localPosition = new Vector3(x, 0, z);
                    gridDot.transform.localScale = new Vector3(2, 2, 2);
                    
                }

                if (cellXCount == thoughtSectionGridSize)
                {
                    cellXCount = 0;
                }
                else
                {
                    cellXCount++;
                }
            }

            if (cellZCount == thoughtSectionGridSize)
            {
                cellZCount = 0;
            }
            else
            {
                cellZCount++;
            }
        }
    }

    void CreateGridMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i=0; i<vertices.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(vertices[i], 2f);
        }
    }
}
