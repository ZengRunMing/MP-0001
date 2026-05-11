using Godot;
using Godot.Collections;

using System;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	public partial class GuidanceControler : STGComponent
	{
		public new STGEntity BaseEntity
		{
			get
			{
				return base.BaseEntity;
			}
			set
			{
				base.BaseEntity = value;

				BaseTransform = base.BaseEntity.GetComponent<Transform>();
				MoveVectorCalculater = base.BaseEntity.GetComponent<MoveVectorCalculater>();
			}
		}

		public STGEntity TargetEntity
		{
			get
			{
				return m_TargetEntity;
			}
			set
			{
				m_TargetEntity = value;

				TargetTransform = m_TargetEntity.GetComponent<Transform>();
			}
		}

		private STGEntity m_TargetEntity;

		public Transform BaseTransform;
		public Transform TargetTransform;

		public MoveVectorCalculater MoveVectorCalculater;

		public override Variant _Get(StringName property)
		{
			throw new NotImplementedException();
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			throw new NotImplementedException();
		}

		public override bool _Set(StringName property, Variant value)
		{
			throw new NotImplementedException();
		}
	}
}
