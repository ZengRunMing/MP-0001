namespace NagaisoraFramework.EntityComponentSystem
{
	public interface ISystem<T> where T : Component
	{
		void Execute(T component);
	}
}
