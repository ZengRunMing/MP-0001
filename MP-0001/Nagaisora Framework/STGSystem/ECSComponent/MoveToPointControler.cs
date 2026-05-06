using Godot;
using Godot.Collections;

using System;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public unsafe partial class MoveToPointControler : STGComponent
	{
		public Transform Transform;
		public MoveControler MoveControler;
		public MoveVectorCalculater MoveVectorCalculater;

		public Vector2 DestPoint;

		public float Accelerate;

		public bool OnMoeToPoint;

		public Action Action;

		public override Variant _Get(StringName property)
		{
			throw new NotImplementedException();
		}

		public override bool _Set(StringName property, Variant value)
		{
			throw new NotImplementedException();
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 执行移动到指定点的控制
		/// </summary>
		/// <param name="destPoint">指定坐标</param>
		/// <param name="maxVelocity">最大速度</param>
		/// <param name="defaultVelovity">默认速度</param>
		/// <param name="action">完成时执行的外部动作</param>
		public void ExecuteMoveToPoint(Vector2 destPoint, float maxVelocity, float defaultVelovity, Action action = null)
		{
			DestPoint = destPoint;

			MoveControler.MaxVelocity = maxVelocity;
			MoveControler.MinVelocity = 0f;
			MoveControler.Velocity = defaultVelovity;

			Action = new Action(action);

			OnMoeToPoint = true;
		}
	}
}
