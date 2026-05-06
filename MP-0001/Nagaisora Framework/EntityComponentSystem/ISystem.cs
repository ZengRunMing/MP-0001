namespace NagaisoraFramework.EntityComponentSystem
{
	public interface ISystem<T> where T : Component
	{
		/// <summary>
		/// 更新在主线程的方法
		/// </summary>
		/// <remarks>
		/// 注意: 此处因尽可能不执行过于耗时的程序
		/// </remarks>
		/// <param name="component"></param>
		void Execute(T component);

		/// <summary>
		/// 更新在子线程的方法
		/// </summary>
		/// <remarks>
		/// 警告: 此处不得执行有关游戏对象更新的程序 (理论上来讲轻则报错，重则程序崩溃)
		/// </remarks>
		/// <param name="component"></param>
		void SubThreadExecute(T component);
	}
}
