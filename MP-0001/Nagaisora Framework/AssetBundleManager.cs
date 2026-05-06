//
//using System;
//using System.Collections.Generic;
//using System.IO;

//using UnityEngine;

//namespace NagaisoraFramework
//{
//	using DataSystem;

//	public static class AssetBundleManager
//	{
//		public static Dictionary<Guid, AssetBundleData> AssetBundleDatas;

//		static AssetBundleManager()
//		{
//			AssetBundleDatas = new Dictionary<Guid, AssetBundleData>();
//		}

//		public static AssetBundleData FindAssetBundleData(Guid guid)
//		{
//			if (AssetBundleDatas.TryGetValue(guid, out AssetBundleData assetBundleData))
//			{
//				return assetBundleData;
//			}

//			GD.PrintException(new FileNotFoundException($"[Asset Bundle Manager] Asset bundle data with GUID -> [{guid}] not found"));
//			return null;
//		}
//	}
//}
//
