using Godot;

namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;

	public partial class OutBoundaryCheckSystem : ISystem<OutBoundaryCheck>
	{
		public void Execute(OutBoundaryCheck component)
		{
			if (!component.IsOutBoundary)
			{
				return;
			}

			component.OutBoundary?.Invoke();
		}

		public void SubThreadExecute(OutBoundaryCheck component)
		{
			component.IsOutBoundary = OutBoundaryCheck(component);
		}

		public static bool OutBoundaryCheck(OutBoundaryCheck component)
		{
			if (Mathf.Abs(component.Transform.Position.X) > component.STGControler.BoundarySize.X / 2 || Mathf.Abs(component.Transform.Position.Y) > component.STGControler.BoundarySize.Y / 2)
			{
				return true;
			}

			return false;
		}
	}
}
