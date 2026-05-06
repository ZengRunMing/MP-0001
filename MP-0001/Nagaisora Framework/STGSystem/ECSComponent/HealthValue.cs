using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class HealthValue : STGComponent
	{
		public float Value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = value;
			}
		}

		public float m_Value = 1f;

		public override Variant _Get(StringName property)
		{
			return (string)property switch
			{
				nameof(Value) => Value,
				_ => default,
			};
		}

		public override bool _Set(StringName property, Variant value)
		{
			switch (property)
			{
				case nameof(Value):
					Value = value.AsSingle();
					break;
				default:
					return false;
			}

			return true;
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			return
			[
				new()
				{
					{ "name", nameof(Value) },
					{ "type", (int)Variant.Type.Float },
				}
			];
		}
	}
}
