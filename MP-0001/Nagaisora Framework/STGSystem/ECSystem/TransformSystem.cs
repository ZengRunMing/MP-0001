using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;

	public partial class TransformSystem : ISystem<Transform>
	{
		public void Execute(Transform component)
		{
			component.BaseSTGEntity.Position = new(component.Position.X, component.Position.Y, 0);
			component.BaseSTGEntity.Rotation = component.Rotation;
			component.BaseSTGEntity.Scale = Vector3.One * component.Scale;
		}

		public void SubThreadExecute(Transform component)
		{
			
		}
	}
}
