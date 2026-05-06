//using System.Collections.Generic;

//using UnityEngine;

//namespace NagaisoraFramework.StorySystem
//{
//	using DataSystem;

//	public class StoryFaceControl : MonoBehaviour
//	{
//		public List<StoryIllustrationData> StoryIllustrationDatas;

//		public Dictionary<uint, StoryIllustrationBox> StoryIllustrationBoxs;

//		public uint EnabledStoryIllustrationBox;

//		public GameObject Prefabricate;

//		public GameObject LeftFaceMain;
//		public GameObject RightFaceMain;

//		public void Init()
//		{
//			StoryIllustrationBoxs = new Dictionary<uint, StoryIllustrationBox>();
//		}

//		public StoryIllustrationBox FindIllustrationBox(uint illustrationBoxIndex)
//		{
//			foreach (KeyValuePair<uint, StoryIllustrationBox> i in StoryIllustrationBoxs)
//			{
//				if (i.Key == illustrationBoxIndex)
//				{
//					return i.Value;
//				}
//			}

//			return null;
//		}

//		public StoryIllustrationBox SetEnableIllustrationBox(uint illustrationBoxIndex)
//		{
//			StoryIllustrationBox box = null;

//			foreach (KeyValuePair<uint, StoryIllustrationBox> i in StoryIllustrationBoxs)
//			{
//				if (i.Key == illustrationBoxIndex)
//				{
//					i.Value.Enable();
//					box = i.Value;

//					continue;
//				}

//				i.Value.Disable();
//			}

//			EnabledStoryIllustrationBox = illustrationBoxIndex;

//			return box;
//		}

//		public void SetExpression(uint illustrationBoxIndex, uint illustrationExpressionIndex)
//		{
//			StoryIllustrationBox box = FindIllustrationBox(illustrationBoxIndex);

//			box.SetFaceImage(illustrationExpressionIndex);
//		}

//		public void SetMainImage(uint illustrationBoxIndex, uint illustrationMainIndex)
//		{
//			StoryIllustrationBox box = FindIllustrationBox(illustrationBoxIndex);
//			box.SetMainImage(illustrationMainIndex);
//		}

//		public void Clear()
//		{
//			foreach (var box in StoryIllustrationBoxs.Values)
//			{
//				Destroy(box.gameObject);
//			}

//			StoryIllustrationBoxs.Clear();
//		}
//	}
//}