using Godot;

namespace NagaisoraFramework
{
	[GlobalClass]
	public partial class InputSystem : Node
	{
		public KeyConfig KeyConfig;

		public bool Enabled = true;

		public Vector2 AxisVector;

		public InputKey InputKey;

		public delegate void KeyDownEvent(InputKey inputKey);
		public event KeyDownEvent KeyDown;

		public InputSystem(KeyConfig keyConfig, string name = "Global")
		{
			Name = name;
			SetKeyConfig(keyConfig);
		}

		public void SetKeyConfig(KeyConfig keyConfig)
		{
			KeyConfig = keyConfig;
			SetKeyConfig();
		}

		public void SetKeyConfig()
		{
			SetAction($"{Name}_Up", KeyConfig.Up, new InputEventJoypadMotion { Axis = JoyAxis.LeftY, AxisValue = 0.5f });
			SetAction($"{Name}_Down", KeyConfig.Down, new InputEventJoypadMotion { Axis = JoyAxis.LeftY, AxisValue = -0.5f });
			SetAction($"{Name}_Left", KeyConfig.Left, new InputEventJoypadMotion { Axis = JoyAxis.LeftX, AxisValue = -0.5f });
			SetAction($"{Name}_Right", KeyConfig.Right, new InputEventJoypadMotion { Axis = JoyAxis.LeftX, AxisValue = 0.5f });

			SetAction($"{Name}_Shoot", KeyConfig.Shoot, KeyConfig.J_Shoot);
			SetAction($"{Name}_Bomb", KeyConfig.Bomb, KeyConfig.J_Bomb);
			SetAction($"{Name}_Slow", KeyConfig.Slow, KeyConfig.J_Slow);
			SetAction($"{Name}_Skip", KeyConfig.Skip, KeyConfig.J_Skip);

			SetAction($"{Name}_Escape", new InputEventKey { PhysicalKeycode = Key.Escape }, KeyConfig.J_Escape);
		}

		public static void SetAction(string actionName, params InputEvent[] events)
		{
			if (!InputMap.HasAction(actionName))
			{
				GD.Print($"The action {actionName} does not exist, and a new action will be added");
				InputMap.AddAction(actionName);
			}

			InputMap.ActionEraseEvents(actionName);

			foreach (InputEvent @event in events)
			{
				InputMap.ActionAddEvent(actionName, @event);
			}
		}

		public static void SetGUIKeyConfig(KeyConfig keyConfig)
		{
			SetAction("ui_up", keyConfig.Up, new InputEventJoypadMotion { Axis = JoyAxis.LeftY, AxisValue = 0.5f });
			SetAction("ui_down", keyConfig.Down, new InputEventJoypadMotion { Axis = JoyAxis.LeftY, AxisValue = -0.5f });
			SetAction("ui_left", keyConfig.Left, new InputEventJoypadMotion { Axis = JoyAxis.LeftX, AxisValue = -0.5f });
			SetAction("ui_right", keyConfig.Right, new InputEventJoypadMotion { Axis = JoyAxis.LeftX, AxisValue = 0.5f });
			SetAction("ui_accept", new InputEventKey { PhysicalKeycode = Key.Enter }, keyConfig.Shoot, keyConfig.J_Shoot);
			SetAction("ui_cancel", new InputEventKey { PhysicalKeycode = Key.Escape }, keyConfig.Bomb, keyConfig.J_Escape, keyConfig.J_Bomb);
		}

		public override void _Input(InputEvent @event)
		{
			if (!Enabled)
			{
				return;
			}

			InputKey = new();

			AxisVector = Input.GetVector($"{Name}_Left", $"{Name}_Right", $"{Name}_Up", $"{Name}_Down");
			if (AxisVector != Vector2.Zero)
			{
				if (AxisVector.Y > 0.5f)
				{
					InputKey.Up = true;
				}
				if (AxisVector.Y < -0.5f)
				{
					InputKey.Down = true;
				}
				if (AxisVector.X < -0.5f)
				{
					InputKey.Left = true;
				}
				if (AxisVector.X > 0.5f)
				{
					InputKey.Right = true;
				}
			}

			if (Input.IsActionPressed($"{Name}_Shoot"))
			{
				InputKey.Shoot = true;
			}
			if (Input.IsActionPressed($"{Name}_Bomb"))
			{
				InputKey.Bomb = true;
			}

			if (Input.IsActionPressed($"{Name}_Slow"))
			{
				InputKey.Slow = true;
			}

			KeyDown?.Invoke(InputKey);
		}
	}
}
