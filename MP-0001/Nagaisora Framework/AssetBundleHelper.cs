//using UnityEngine;

//namespace NagaisoraFramework
//{
//	public static class AssetBundleHelper
//	{
//		public static AssetBundle LoadAssetBundleFromFile(string filePath)
//		{
//			if (string.IsNullOrEmpty(filePath))
//			{
//				GD.PrintError("File path is null or empty.");
//				return null;
//			}
//			if (!System.IO.File.Exists(filePath))
//			{
//				GD.PrintError($"AssetBundle file not found at path: \"{filePath}\"");
//				return null;
//			}

//			AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
			
//			if (assetBundle == null)
//			{
//				GD.PrintError($"Failed to load AssetBundle from file: \"{filePath}\"");
//			}
			
//			return assetBundle;
//		}

//		public static AssetBundle LoadAssetBundle(byte[] binary)
//		{
//			if (binary == null || binary.Length == 0)
//			{
//				GD.PrintError("Binary data is null or empty.");
//				return null;
//			}

//			AssetBundle assetBundle = AssetBundle.LoadFromMemory(binary);

//			if (assetBundle == null)
//			{
//				GD.PrintError("Failed to load AssetBundle from binary data.");
//				return null;
//			}

//			return assetBundle;
//		}

//		public static void UnloadAssetBundle(AssetBundle assetBundle, bool unloadAllLoadedObjects = false)
//		{
//			if (assetBundle == null)
//			{
//				GD.PrintWarning("AssetBundle is null, nothing to unload.");
//				return;
//			}
//			assetBundle.Unload(unloadAllLoadedObjects);
//			GD.Print($"AssetBundle \"{assetBundle.name}\" unloaded successfully.");
//		}

//		public static T LoadAsset<T>(AssetBundle assetBundle, string assetName) where T : Object
//		{
//			if (assetBundle == null)
//			{
//				GD.PrintError("AssetBundle is null.");
//				return null;
//			}
			
//			T asset = assetBundle.LoadAsset<T>(assetName);

//			if (asset == null)
//			{
//				GD.PrintError($"Asset \"{assetName}\" not found in AssetBundle \"{assetBundle.name}\".");
//			}
//			return asset;
//		}
//	}
//}
