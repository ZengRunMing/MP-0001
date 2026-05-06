using Godot;
using Godot.Collections;

using System;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class CollisionCheck(Action<STGEntity> action) : STGComponent
	{
		public new STGEntity BaseSTGEntity
		{
			get => base.BaseSTGEntity;
			set
			{
				base.BaseSTGEntity = value;
				BaseEntityDetermineData = base.BaseSTGEntity.GetComponent<DetermineData>();
				BaseEntityTransform = base.BaseSTGEntity.GetComponent<Transform>();
			}
		}

		public Array<STGEntity> TargetSTGEntitys
		{
			get => m_TargetSTGEntitys;
			set
			{
				m_TargetSTGEntitys = value;

				TargetEntityDetermineDatas.Clear();
				TargetEntityTransforms.Clear();

				foreach (STGEntity entity in m_TargetSTGEntitys)
				{
					TargetEntityDetermineDatas.Add(entity.GetComponent<DetermineData>());
					TargetEntityTransforms.Add(entity.GetComponent<Transform>());
				}
			}
		}

		public bool Scale;

		public Array<bool> IsCollision = [];

		public Array<STGEntity> m_TargetSTGEntitys = [];
		
		public DetermineData BaseEntityDetermineData;
		public Transform BaseEntityTransform;

		public Array<DetermineData> TargetEntityDetermineDatas = [];
		public Array<Transform> TargetEntityTransforms = [];

		public Action<STGEntity> Action = action;

		public override Variant _Get(StringName property)
		{
			return (string)property switch
			{
				nameof(TargetSTGEntitys) => (Variant)(TargetSTGEntitys ?? default),
				nameof(Scale) => (Variant)Scale,
				_ => default,
			};
		}

		public override bool _Set(StringName property, Variant value)
		{
			switch (property)
			{
				case nameof(TargetSTGEntitys):
					TargetSTGEntitys = value.AsGodotArray<STGEntity>();
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
					{ "name", nameof(TargetSTGEntitys) },
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
