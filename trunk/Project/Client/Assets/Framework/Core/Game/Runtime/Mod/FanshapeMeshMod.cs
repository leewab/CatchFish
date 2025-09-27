// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using UnityEngine;
// using Utils;
// namespace MUGame
// {
//     public class FanshapeMeshItem
//     {
//         public GameObject gameObjce;
//         public MeshFilter meshFilter;
//         public MeshRenderer meshRender;
//         public Mesh mesh;
//
//         public FanshapeMeshItem()
//         {
//             gameObjce = new GameObject();
//             meshFilter = gameObjce.AddComponent<MeshFilter>();      
//             meshRender = gameObjce.AddComponent<MeshRenderer>();
//             mesh = meshFilter.mesh;
//         }
//     };
//     class FanshapeMeshMod
//     {
//         static FanshapeMeshMod _instance;
//         public static FanshapeMeshMod Instance {
//             get{
//                 if (_instance == null)
//                     _instance = new FanshapeMeshMod();
//                 return _instance;
//              }
//         }
//         private MemoryPool<FanshapeMeshItem> cacheMesh = new MemoryPool<FanshapeMeshItem>(10);
//
//         /// <summary>
//         /// 创建mesh
//         /// </summary>
//         /// <param name="radius">半径</param>
//         /// <param name="angleDegree">角度</param>
//         /// <param name="segments">分段数</param>
//         /// <param name="cradius">内部圆心半径</param>
//         /// <returns></returns>
//         // public Mesh CreateMesh(float radius, float angleDegree, int segments = 1, float cradius = 0.5f)  
//         //{    
//         //    Mesh newMesh = Create(radius, angleDegree, segments, cradius);
//         //    return newMesh;
//         //}  
//         public void DisposeMesh(FanshapeMeshItem item)
//          {
//              cacheMesh.Free(item);
//          }
//         private Mesh Create(Mesh mesh, float radius, float angleDegree, int segments, float cradius = 0.5f)  
//         {
//             if (segments == 0)
//             {
//                 segments = 1;
//             }
//
//             if (mesh == null)
//             {
//                 mesh = new Mesh();
//             }
//             Vector3[] vertices = new Vector3[3 + segments - 1 + 10];
//             vertices[0] = new Vector3(0, 0, 0);
//
//             float angle = Mathf.Deg2Rad * angleDegree;
//             float currAngle = angle / 2;
//             float deltaAngle = angle / segments;
//             float deltaAngle2 = Mathf.Deg2Rad * (360f - angleDegree) / 9;
//             float currAngle2 = 0f;
//             for (int i = 1; i < vertices.Length; i++)
//             {
//                 if (i < vertices.Length - 10)
//                 {
//                     currAngle2 = currAngle;
//                     vertices[i] = new Vector3(Mathf.Cos(currAngle) * radius, 0, Mathf.Sin(currAngle) * radius);
//                     currAngle -= deltaAngle;
//                 }else
//                 {
//                     vertices[i] = new Vector3(Mathf.Cos(currAngle2) * cradius, 0, Mathf.Sin(currAngle2) * cradius);
//                     currAngle2 -= deltaAngle2;
//                 }
//             }
//
//             int[] triangles = new int[(segments + 10) * 3];
//             for (int i = 0, vi = 1; i < triangles.Length; i += 3, vi++)
//             {
//                 triangles[i] = 0;
//                 triangles[i + 1] = vi;
//                 triangles[i + 2] = vi + 1;
//             }
//
//             mesh.vertices = vertices;
//             mesh.triangles = triangles;
//
//             Vector2[] uvs = new Vector2[vertices.Length];
//             for (int i = 0; i < uvs.Length; i++)
//             {
//                 uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
//             }
//             mesh.uv = uvs;
//
//             return mesh;  
//         }  
//   
//        //private bool checkDiff(float radius, float angleDegree, int segments, int angleDegreePrecision, int radiusPrecision)  
//        //{  
//        //    return segments != this.segments || (int)((angleDegree - this.angleDegree) * angleDegreePrecision) != 0 ||  
//        //            (int)((radius - this.radius) * radiusPrecision) != 0;  
//        // }  
//
//
//         /// <summary>
//         /// 创建FanshapeMeshItem
//         /// </summary>
//         /// <param name="radius">半径</param>
//         /// <param name="angleDegree">角度</param>
//         /// <param name="segments">分段数</param>
//         /// <param name="cradius">内部圆心半径</param>
//         /// <returns></returns>
//         public FanshapeMeshItem CreateFanshapeMeshItem(float radius, float angleDegree, int segments = 1, float cradius = 0.5f)
//         {
//             FanshapeMeshItem item = cacheMesh.Alloc();
//             item.gameObjce.layer = 23;
//             item.meshRender.material.shader = Shader.Find("GOE/MainColor");
//             item.meshRender.material.color = new Color(0.52f, 0.44f, 0.34f, 1f);
//
//             Create(item.mesh, radius, angleDegree, segments, cradius);
//
//
//             return item;
//         }
//
//
//     }
// }
