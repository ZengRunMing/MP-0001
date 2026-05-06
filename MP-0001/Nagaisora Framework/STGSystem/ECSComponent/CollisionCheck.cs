using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class CollisionCheck : STGComponent
	{
		public new STGEntity BaseSTGEntity
		{
			get => base.BaseSTGEntity;
			set
			{
				base.BaseSTGEntity = value;
				BaseEntityDetermineRadius = base.BaseSTGEntity.GetComponent<DetermineData>();
			}
		}

		public STGEntity TargetSTGEntity
		{
			get => m_TargetEntity;
			set
			{
				m_TargetEntity = value;
				TargetEntityDetermineRadius = m_TargetEntity.GetComponent<DetermineData>();
			}
		}

		public bool IsCollision { get; set; }

		public STGEntity m_TargetEntity;
		

		public DetermineData BaseEntityDetermineRadius;
		public DetermineData TargetEntityDetermineRadius;

		public override Variant _Get(StringName property)
		{
			return (string)property switch
			{
				nameof(TargetSTGEntity) => (Variant)(TargetSTGEntity ?? default),
				nameof(IsCollision) => (Variant)IsCollision,
				_ => default,
			};
		}

		public override bool _Set(StringName property, Variant value)
		{
			switch (property)
			{
				case nameof(TargetSTGEntity):
					TargetSTGEntity = (STGEntity)Window.GetFocusedWindow().GetNode(value.AsNodePath());
					break;
				case nameof(IsCollision):
					IsCollision = value.AsBool();
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
					{ "name", nameof(TargetSTGEntity) },
					{ "type", (int)Variant.Type.NodePath },
				},
				new()
				{
					{ "name", nameof(IsCollision) },
					{ "type", (int)Variant.Type.Bool },
				}
			];
		}
	}
}
