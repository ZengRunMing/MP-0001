using Godot;
using Godot.Collections;

using System;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class CollisionCheck(Action<STGEntity> action) : STGComponent
	{
		public new STGEntity BaseEntity
		{
			get => base.BaseEntity;
			set
			{
				base.BaseEntity = value;
				BaseEntityDetermineData = base.BaseEntity.GetComponent<DetermineData>();
				BaseEntityTransform = base.BaseEntity.GetComponent<Transform>();
			}
		}

		public Array<STGEntity> TargetEntitys
		{
			get => m_TargetEntitys;
			set
			{
				m_TargetEntitys = value;

				TargetEntityDetermineDatas.Clear();
				TargetEntityTransforms.Clear();

				foreach (STGEntity entity in m_TargetEntitys)
				{
					TargetEntityDetermineDatas.Add(entity.GetComponent<DetermineData>());
					TargetEntityTransforms.Add(entity.GetComponent<Transform>());
				}
			}
		}

		public bool Scale;

		public Array<bool> IsCollision = [];

		public Array<STGEntity> m_TargetEntitys = [];
		
		public DetermineData BaseEntityDetermineData;
		public Transform BaseEntityTransform;

		public Array<DetermineData> TargetEntityDetermineDatas = [];
		public Array<Transform> TargetEntityTransforms = [];

		public Action<STGEntity> Action = action;

		public override Variant _Get(StringName property)
		{
			return (string)property switch
			{
				nameof(TargetEntitys) => (Variant)(TargetEntitys ?? default),
				nameof(Scale) => (Variant)Scale,
				_ => default,
			};
		}

		public override bool _Set(StringName property, Variant value)
		{
			switch (property)
			{
				case nameof(TargetEntitys):
					TargetEntitys = value.AsGodotArray<STGEntity>();
					break;
				case nameof(Scale):
					Scale = value.AsBool();
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
					{ "name", nameof(TargetEntitys) },
					{ "type", (int)Variant.Type.Array },
				},
				new()
				{
					{ "name", nameof(Scale) },
					{ "type", (int)Variant.Type.Bool },
				}
			];
		}
	}
}
