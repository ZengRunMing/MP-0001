//using NagaisoraFramework.DataSystem;
//using UnityEngine;
//using UnityEngine.UI;

//namespace NagaisoraFramework.StorySystem
//{
//	public class StoryIllustrationBox : MonoBehaviour
//	{
//		public StoryIllustrationData IllustrationData;

//		public bool Disabled = true;
//		public bool IsLeft = false;

//		public Animation Animation;
//		public Image MainImageBox;
//		public Image FaceImageBox;

//		public void Init(uint main, uint face)
//		{
//			SetMainImage(main);
//			SetFaceImage(face);
//			Enable();
//		}

//		public void SetMainImage(uint i)
//		{
//			IllustrationSpriteData data = IllustrationData.Images[i];

//			MainImageBox.sprite = data.Sprite;
//			MainImageBox.rectTransform.localPosition = data.Position;
//			MainImageBox.rectTransform.sizeDelta = data.Size;
//		}

//		public void SetFaceImage(uint i)
//		{
//			IllustrationSpriteData data = IllustrationData.Images[i];

//			FaceImageBox.sprite = data.Sprite;
//			FaceImageBox.transform.localPosition = data.Position;
//			FaceImageBox.rectTransform.sizeDelta = data.Size;
//		}

//		public void SetOrderTop()
//		{
//			transform.SetAsLastSibling();
//		}

//		public void SetImageColor(Color color)
//		{
//			MainImageBox.color = color;
//			FaceImageBox.color = color;
//		}

//		public void Enable()
//		{
//			SetOrderTop();
//			if (IsLeft)
//			{
//				Animation.Play("LeftFaceEnable");
//			}
//			else
//			{
//				Animation.Play("RightFaceEnable");
//			}

//			Disabled = false;
//		}

//		public void Disable()
//		{
//			if (IsLeft)
//			{
//				Animation.Play("LeftFaceDisable");
//			}
//			else
//			{
//				Animation.Play("RightFaceDisable");
//			}

//			Disabled = true;
//		}
//	}
//}