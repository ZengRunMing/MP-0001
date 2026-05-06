//using UnityEngine;

//namespace NagaisoraFramework.STGSystem
//{
//	public class BendLaserMesh : MonoBehaviour
//	{
//		public MeshFilter meshFilter;
//		public MeshRenderer meshRenderer;

//		public Material material;

//		private Mesh mesh;

//		private Color color;

//		public int ColorType;

//		public int Length = 5;

//		public float Width = 8f;

//		public int UVWidth = 16;

//		private Vector3[] vertices;

//		private Vector2[] uv;

//		private int[] triangles;

//		public Vector2[] KeyPoints;

//		public bool Inited;

//		public float Transparent
//		{
//			get
//			{
//				return color.a * 255f;
//			}
//			set
//			{
//				color.a = Mathf.Clamp(value, 0f, 255f) / 255f;
//				material.color = color;
//			}
//		}

//		public void FixedUpdate()
//		{
//			if (!Inited)
//			{
//				return;
//			}

//			meshRenderer.material = material;

//			if (KeyPoints is null || KeyPoints.Length < 2)
//			{
//				return;
//			}

//			SetLength();
//			SetVertives();
//			SetUV();
//			SetTriangles();
//			meshFilter.mesh = mesh;
//		}

//		private void CreateMesh()
//		{
//			mesh = new Mesh();

//			if (KeyPoints is null || KeyPoints.Length < 2)
//			{
//				return;
//			}

//			SetLength();
//			SetVertives();
//			SetUV();
//			SetTriangles();
//		}

//		private void SetLength()
//		{
//			if (Length < 2)
//			{
//				Length = 2;
//			}
//			vertices = new Vector3[Length * 2];
//			uv = new Vector2[Length * 2];
//			triangles = new int[(Length - 1) * 6];
//		}

//		private void SetVertives()
//		{
//			if (KeyPoints.Length != Length)
//			{
//				Length = KeyPoints.Length;
//				SetLength();
//			}
//			float f = Mathf.Atan2(KeyPoints[0].y - KeyPoints[1].y, KeyPoints[0].x - KeyPoints[1].x);
//			vertices[0].x = KeyPoints[0].x - Width / 2f * Mathf.Sin(f);
//			vertices[0].y = KeyPoints[0].y + Width / 2f * Mathf.Cos(f);
//			vertices[1].x = KeyPoints[0].x + Width / 2f * Mathf.Sin(f);
//			vertices[1].y = KeyPoints[0].y - Width / 2f * Mathf.Cos(f);
//			for (int i = 1; i < Length; i++)
//			{
//				vertices[2 * i].x = KeyPoints[i].x - Width / 2f * Mathf.Sin(f);
//				vertices[2 * i].y = KeyPoints[i].y + Width / 2f * Mathf.Cos(f);
//				vertices[2 * i + 1].x = KeyPoints[i].x + Width / 2f * Mathf.Sin(f);
//				vertices[2 * i + 1].y = KeyPoints[i].y - Width / 2f * Mathf.Cos(f);
//				f = Mathf.Atan2(KeyPoints[i - 1].y - KeyPoints[i].y, KeyPoints[i - 1].x - KeyPoints[i].x);
//			}
//			mesh.vertices = vertices;
//		}

//		private void SetUV()
//		{
//			int num = UVWidth - ColorType;
//			uv[0].x = 0f;
//			uv[0].y = num / (float)UVWidth;
//			uv[1].x = 0f;
//			uv[1].y = (num + 1) / (float)UVWidth;
//			for (int i = 1; i < Length; i++)
//			{
//				uv[2 * i].x = i / (float)(Length - 1);
//				uv[2 * i].y = num / (float)UVWidth;
//				uv[2 * i + 1].x = i / (float)(Length - 1);
//				uv[2 * i + 1].y = (num + 1) / (float)UVWidth;
//			}
//			mesh.uv = uv;
//		}

//		private void SetTriangles()
//		{
//			int num = 0;
//			for (int i = 0; i < Length - 1; i++)
//			{
//				for (int j = 0; j < 1; j++)
//				{
//					int num2 = 2;
//					int num3 = j + i * num2;
//					triangles[num] = num3;
//					triangles[num + 1] = num3 + num2 + 1;
//					triangles[num + 2] = num3 + 1;
//					triangles[num + 3] = num3;
//					triangles[num + 4] = num3 + num2;
//					triangles[num + 5] = num3 + num2 + 1;
//					num += 6;
//				}
//			}
//			mesh.triangles = triangles;
//		}

//		public void Init(string sortingLayerName, int order)
//		{
//			if (!TryGetComponent(out meshFilter))
//			{
//				meshFilter = gameObject.AddComponent<MeshFilter>();
//			}

//			if (!TryGetComponent(out meshRenderer))
//			{
//				meshRenderer = gameObject.AddComponent<MeshRenderer>();
//			}

//			CreateMesh();
//			transform.localScale = new Vector3(1f, 1f, 1f);
//			meshRenderer.sortingLayerName = sortingLayerName;
//			meshRenderer.sortingOrder = order;

//			Inited = true;
//		}

//		public void Clear()
//		{
//			Destroy(meshFilter);
//			Destroy(meshRenderer);
//		}
//	}
//}