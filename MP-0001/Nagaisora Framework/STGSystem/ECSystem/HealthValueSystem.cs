namespace NagaisoraFramework.STGSystem.ECSystem
{
	using ECSComponent;
	using EntityComponentSystem;
	
	public partial class HealthValueSystem : ISystem<HealthValue>
	{
		public void Execute(HealthValue component)
		{
			int i = 0;

			foreach (float value in component.SendActionValues)
			{
				if (component.ActionSendeds[i])
				{
					goto skip;
				}

				if (component.Value < value)
				{
					component.Action?.Invoke(value);
					component.ActionSendeds[i] = true;
				}

				skip:
				i++;
			}
		}

		public void SubThreadExecute(HealthValue component)
		{
			
		}
	}
}
