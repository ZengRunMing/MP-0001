using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class Transform : STGComponent
	{
		public Vector2 Position;
		public Vector3 Rotation;
		public float Scale;

		public Vector3 OriginalPosition;
		public Vector3 OriginalScale;

		public override Variant _Get(StringName property)
		{
			return (string)property switch
			{
				nameof(Position) => (Variant)Position,
				nameof(Rotation) => (Variant)Rotation,
				nameof(Scale) => (Variant)Scale,

				nameof(OriginalPosition) => (Variant)OriginalPosition,
				nameof(OriginalScale) => (Variant)OriginalScale,
				_ => default
			};
		}

		public override bool _Set(StringName property, Variant value)
		{
			switch (property)
			{
				case nameof(Position):
					Position = value.AsVector2();
					break;
				case nameof(Rotation):
					Rotation = value.AsVector3();
					break;
				case nameof(Scale):
					Scale = value.AsSingle();
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
					{ "name", nameof(Position) },
					{ "type", (int)Variant.Type.Vector2 },
				},
				new()
				{
					{ "name", nameof(Rotation) },
					{ "type", (int)Variant.Type.Vector3 },
				},
				new()
				{
					{ "name", nameof(Scale) },
					{ "type", (int)Variant.Type.Float },
				},
				new()
				{
					{ "name", nameof(OriginalPosition) },
					{ "type", (int)Variant.Type.Vector3 },
				},
				new()
				{
					{ "name", nameof(OriginalScale) },
					{ "type", (int)Variant.Type.Vector3 },
				}
			];
		}
	}
}
