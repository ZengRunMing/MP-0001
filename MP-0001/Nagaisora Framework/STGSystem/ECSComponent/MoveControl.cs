using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class MoveControl : STGComponent
	{
		public Vector2 MoveVector;

		/// <summary>
		/// 组件在移动时的速度, 以像素/帧为单位
		/// </summary>
		/// <remarks>
		/// <para>组件的实际移动向量由Direction和Velocity共同决定, 当Velocity为0时组件不移动</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改, 在设置时会自动限制在MinVelocity和MaxVelocity之间</para>
		/// </remarks>
		[Export]
		public float Velocity
		{
			get
			{
				return m_Velocity;
			}
			set
			{
				if (value > MaxVelocity)
				{
					m_Velocity = MaxVelocity;
				}
				else if (value < MinVelocity)
				{
					m_Velocity = MinVelocity;
				}
				else
				{
					m_Velocity = value;
				}
			}
		}

		/// <summary>
		/// 界定组件移动速度的上限, 以像素/帧为单位
		/// </summary>
		/// <remarks>
		/// <para>当Velocity被设置时会自动限制在MaxVelocity和MinVelocity之间</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改, 在设置时会自动限制Velocity的值在MinVelocity和MaxVelocity之间</para>
		/// </remarks>
		[Export]
		public float MaxVelocity
		{
			get
			{
				return m_MaxVelocity;
			}
			set
			{
				m_MaxVelocity = value;
				if (Velocity > m_MaxVelocity)
				{
					m_Velocity = m_MaxVelocity;
				}
			}
		}

		/// <summary>
		/// 界定组件移动速度的下限, 以像素/帧为单位
		/// </summary>
		/// <remarks>
		/// <para>当Velocity被设置时会自动限制在MaxVelocity和MinVelocity之间</para>
		/// <para>使用Export特性以便在编辑器中调试时查看和修改, 在设置时会自动限制Velocity的值在MinVelocity和MaxVelocity之间</para>
		/// </remarks>
		[Export]
		public float MinVelocity
		{
			get
			{
				return m_MinVelocity;
			}
			set
			{
				m_MinVelocity = value;
				if (Velocity < m_MinVelocity)
				{
					m_Velocity = m_MinVelocity;
				}
			}
		}

		private float m_Velocity;
		private float m_MaxVelocity;
		private float m_MinVelocity;

		public override Variant _Get(StringName property)
		{
			throw new System.NotImplementedException();
		}

		public override bool _Set(StringName property, Variant value)
		{
			throw new System.NotImplementedException();
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			throw new System.NotImplementedException();
		}
	}
}
