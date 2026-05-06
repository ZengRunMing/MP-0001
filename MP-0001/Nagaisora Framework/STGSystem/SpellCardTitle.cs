//using UnityEngine;

//namespace NagaisoraFramework.STGSystem
//{
//	public class SpellCardTitle : MonoBehaviour
//	{

//		protected string m_Title;

//		protected uint m_Score;

//		protected ushort m_HistoryCount;

//		protected ushort m_GetCount;

//		protected bool m_Failed;

//		public string Title
//		{
//			get => m_Title;
//			set => m_Title = value;
//		}

//		public uint Score
//		{
//			get => m_Score;
//			set => m_Score = value;
//		}

//		public ushort HistoryCount
//		{
//			get => m_HistoryCount;
//			set => m_HistoryCount = value;
//		}

//		public ushort GetCount
//		{
//			get => m_GetCount;
//			set => m_GetCount = value;
//		}

//		public bool Failed
//		{
//			get => m_Failed;
//			set => m_Failed = value;
//		}

//		public DoubleLayeredText TitleText;
//		public DoubleLayeredText ScoreText;
//		public DoubleLayeredText FailedText;
//		public DoubleLayeredText HistoryText;

//		public void Update()
//		{
//			TitleText.text = Title;
//			ScoreText.text = Score.ToString();
//			HistoryText.text = $"{GetCount:d2} / {HistoryCount:d2}";

//			if (!Failed)
//			{
//				ScoreText.gameObject.SetActive(true);
//				FailedText.gameObject.SetActive(false);
//			}
//			else
//			{
//				ScoreText.gameObject.SetActive(false);
//				FailedText.gameObject.SetActive(true);
//			}
//		}
//	}
//}
