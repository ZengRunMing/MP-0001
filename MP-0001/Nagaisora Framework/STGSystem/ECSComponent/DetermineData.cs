using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class DetermineData : STGComponent
	{
		public float DetermineRadius { get; set; }
		public Vector2 DetermineOffset { get; set; }

		public override Variant _Get(StringName property)
		{
			return (string)property switch
			{
				nameof(DetermineRadius) => (Variant)DetermineRadius,
				nameof(DetermineOffset) => (Variant)DetermineOffset,
				_ => default,
			};
		}

		public override bool _Set(StringName property, Variant value)
		{
			switch (property)
			{
				case nameof(DetermineRadius):
					DetermineRadius = value.AsSingle();
					break;
				case nameof(DetermineOffset):
					DetermineOffset = value.AsVector2();
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
					{ "name", nameof(DetermineRadius) },
					{ "type", (int)Variant.Type.Float },
				},
				new()
				{
					{ "name", nameof(DetermineOffset) },
					{ "type", (int)Variant.Type.Vector2 },
				}
			];
		}
	}
}
