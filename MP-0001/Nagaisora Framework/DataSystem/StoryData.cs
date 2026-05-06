//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace NagaisoraFramework.DataSystem
//{
//	public enum StoryType
//	{
//		Scense = 1,
//	}

//
//	public class StoryData : NagaisoraFrameworkData
//	{
//		public const string DataHead = "NSTD";

//		public StoryProgram StoryProgram;

//		public byte[] ToBinary()
//		{
//			return null;
//		}

//		public StoryData FromBinary(byte[] binary)
//		{
//			return null;
//		}
//	}

//
//	public struct FaceImageInfo
//	{
//		public string Name;
//		public Vector2 Size;
//		public Sprite Image;
//	}

//
//	public struct SceneInfo
//	{
//		public string SceneName;
//		public Image SceneImage;
//	}

//
//	public class StoryProgram
//	{
//		public string ProgramName;
//		public List<StoryProgramStep> ProgramSteps;
//	}

//
//	public class StoryProgramStep
//	{
//		public List<StoryProgramAction> Actions;
//		public StoryProgramAction Transition;

//		public Queue<StoryProgramAction> ActionsQueue;

//		public void Init()
//		{
//			if (Actions == null)
//			{
//				return;
//			}

//			ActionsQueue = new Queue<StoryProgramAction>();

//			foreach (StoryProgramAction action in Actions)
//			{
//				ActionsQueue.Enqueue(action);
//			}
//		}
//	}

//
//	public class StoryProgramAction
//	{
//		public uint Command;
//		public List<string> CommandValue;
//	}
//}
