using Godot;
using Godot.Collections;

using System;

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

		public float m_Value;

		public Array<float> SendActionValues;

		public Array<bool> ActionSendeds;

		public Action<float> Action;

		public HealthValue(float value, Array<float> sendActionValues, Action<float> action)
		{
			Value = value;
			SendActionValues = sendActionValues.Duplicate(true);

			ActionSendeds = [.. new bool[SendActionValues.Count]];

			Action = action;
		}

		public override Variant _Get(StringName property)
		{
			return (string)property switch
			{
				nameof(Value) => Value,
				nameof(SendActionValues) => SendActionValues,
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
				case nameof(SendActionValues):
					SendActionValues = value.AsGodotArray<float>();
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
				new()
				{
					{ "name", nameof(SendActionValues) },
					{ "type", (int)Variant.Type.Array },
				}
			];
		}
	}
}
