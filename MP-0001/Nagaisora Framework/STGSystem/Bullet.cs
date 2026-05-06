using System;

using Godot;

namespace NagaisoraFramework.STGSystem
{
	[GlobalClass]

	public partial class Bullet : STGEntity
	{
		public STGEntity DetermingTarget;

		public bool Determing = true;
		public bool Delete_Effect = false;

		/// <summary>
		/// <para>子弹组件的全判定方法</para>
		/// <para>可以通过重写添加需要执行的判定程序，不需要回调父类的函数</para>
		/// </summary>
		/// <param name="Target">指定的STGComponment</param>
		public virtual void Check(STGEntity Target)
		{

		}
	}
}

