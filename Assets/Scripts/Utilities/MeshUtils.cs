using UnityEngine;

namespace MemoryFishing.Utilities
{
    public static class MeshUtils
    {
        private static Quaternion AngleAxis(float rotationFloat, Vector3 normal)
        {
            int rotation = Mathf.RoundToInt(rotationFloat);
            rotation %= 360;
            if (rotation < 0) rotation += 360;

            return Quaternion.AngleAxis(rotation, normal);
        }

        public static Mesh CreateQuad(float width, float height)
        {
            Mesh quad = new();

            Vector3[] vertices = new Vector3[4]; // 4 Vertices are created, one for each corner of the Quad.
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6]; // 2 Tris are created. Each Tri makes up 3 indexes on an array, meaning the array is of size 6.

            vertices[0] = new Vector3(0, 0);            // Bottom Left
            vertices[1] = new Vector3(0, height);       // Top Left
            vertices[2] = new Vector3(width, height);   // Top Right
            vertices[3] = new Vector3(width, 0);        // Bottom Right

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);

            // Tris will be front-facing, so vertex index moves clockwise
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            quad.vertices = vertices;
            quad.uv = uv;
            quad.triangles = triangles;

            return quad;
        }

        // Creates an Array of Vertices, UVs and Tris. These will always match the required size to make a quad, or a whole number of quads.
        public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
        {
            vertices = new Vector3[4 * quadCount];
            uvs = new Vector2[4 * quadCount];
            triangles = new int[6 * quadCount];
        }

        // Takes Vertices, UVs and Tris of a full mesh, and relocates some of them in order to add a quad to the mesh.
        public static void AddQuadFromMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 worldPosition, Vector3 normal, Vector3 quadSize, Vector2 uvMin, Vector2 uvMax)
        {
            // Relocate Vertices
            int startingVertIndex = index * 4;

            quadSize *= .5f;

            vertices[startingVertIndex + 0] = worldPosition + (AngleAxis(90, normal) * quadSize);
            vertices[startingVertIndex + 1] = worldPosition + (AngleAxis(180, normal) * quadSize);
            vertices[startingVertIndex + 2] = worldPosition + (AngleAxis(270, normal) * quadSize);
            vertices[startingVertIndex + 3] = worldPosition + (AngleAxis(0, normal) * quadSize);

            // Relocate UVs
            uvs[startingVertIndex + 0] = new Vector2(uvMin.x, uvMax.y);
            uvs[startingVertIndex + 1] = new Vector2(uvMin.x, uvMin.y);
            uvs[startingVertIndex + 2] = new Vector2(uvMax.x, uvMin.y);
            uvs[startingVertIndex + 3] = new Vector2(uvMax.x, uvMax.y);

            // Set Tris to Match Vertices
            int startingTriIndex = index * 6;

            triangles[startingTriIndex + 0] = startingVertIndex + 0;
            triangles[startingTriIndex + 1] = startingVertIndex + 1;
            triangles[startingTriIndex + 2] = startingVertIndex + 2;

            triangles[startingTriIndex + 3] = startingVertIndex + 0;
            triangles[startingTriIndex + 4] = startingVertIndex + 2;
            triangles[startingTriIndex + 5] = startingVertIndex + 3;
        }
    }
}