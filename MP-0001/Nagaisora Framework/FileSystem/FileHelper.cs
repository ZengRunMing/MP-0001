using System;
using System.Collections.Generic;
using System.IO;

namespace NagaisoraFramework
{
	public static class FileHelper
	{
		/// <summary>
		/// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
		/// </summary>
		/// <param name="path">文件夹路径</param>
		/// <param name="filextension">扩展名可以多个 例如 .mp3.wma.rm</param>
		/// <returns>List<FileInfo></returns>
		public static void GetFile(string path, string filextension, ref List<FileInfo> list)
		{
			if (list == null)
			{
				list = new List<FileInfo>();
			}

			string[] dir = Directory.GetDirectories(path); //文件夹列表   
			DirectoryInfo fdir = new DirectoryInfo(path);
			FileInfo[] file = fdir.GetFiles();
			//FileInfo[] file = Directory.GetFiles(path); //文件列表   
			if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空
			{
				foreach (FileInfo f in file) //显示当前目录所有文件   
				{
					if (filextension.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
					{
						list.Add(f);
					}
				}

				foreach (string d in dir)
				{
					GetFile(d, filextension, ref list);//递归
				}
			}
			return;
		}

		public static (string filename, string extension) GetFileName(string path)
		{
			string filename = Path.GetFileName(path);		//文件名
			string filextension = Path.GetExtension(path);	//拓展名

			return (filename, filextension);
		}
	}
}
