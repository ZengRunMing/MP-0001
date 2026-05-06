using Godot;

namespace NagaisoraFramework
{
	public class KeyConfig
	{
		public InputEvent Up;
		public InputEvent Down;
		public InputEvent Left;
		public InputEvent Right;
		public InputEvent Shoot;
		public InputEvent Bomb;
		public InputEvent Slow;
		public InputEvent Skip;

		public InputEvent J_Shoot;
		public InputEvent J_Bomb;
		public InputEvent J_Slow;
		public InputEvent J_Skip;
		public InputEvent J_Escape;

		public static readonly KeyConfig Default = new()
		{
			Up = new InputEventKey { PhysicalKeycode = Key.Up },
			Down = new InputEventKey { PhysicalKeycode = Key.Down },
			Left = new InputEventKey { PhysicalKeycode = Key.Left },
			Right = new InputEventKey { PhysicalKeycode = Key.Right },

			Shoot = new InputEventKey { PhysicalKeycode = Key.Z },
			Bomb = new InputEventKey { PhysicalKeycode = Key.X },
			Slow = new InputEventKey { PhysicalKeycode = Key.Shift, Location = KeyLocation.Left },
			Skip = new InputEventKey { PhysicalKeycode = Key.Ctrl, Location = KeyLocation.Left },

			J_Shoot = new InputEventJoypadButton { ButtonIndex = JoyButton.X },
			J_Bomb = new InputEventJoypadMotion { Axis = JoyAxis.TriggerLeft },
			J_Slow = new InputEventJoypadMotion { Axis = JoyAxis.TriggerRight },
			J_Skip = new InputEventJoypadButton { ButtonIndex = JoyButton.B },

			J_Escape = new InputEventJoypadButton { ButtonIndex = JoyButton.A },
		};

		public byte[] ToBinary()
		{
			MemoryStream stream = new();
			BinaryWriter writer = new(stream, Encoding.UTF8, true);

			writer.Write(InputEventHelper.ToBinary(Up));
			writer.Write(InputEventHelper.ToBinary(Down));
			writer.Write(InputEventHelper.ToBinary(Left));
			writer.Write(InputEventHelper.ToBinary(Right));

			writer.Write(InputEventHelper.ToBinary(Shoot));
			writer.Write(InputEventHelper.ToBinary(Bomb));
			writer.Write(InputEventHelper.ToBinary(Slow));

			writer.Write(InputEventHelper.ToBinary(J_Shoot));
			writer.Write(InputEventHelper.ToBinary(J_Bomb));
			writer.Write(InputEventHelper.ToBinary(J_Slow));

			writer.Write(InputEventHelper.ToBinary(J_Escape));

			writer.Close();

			return stream.ToArray();
		}

		public static KeyConfig FromBinary(byte[] binary)
		{
			MemoryStream stream = new(binary);
			BinaryReader reader = new(stream, Encoding.UTF8, false);

			KeyConfig keyconfig = new()
			{
				Up = InputEventHelper.FromBinary(reader.ReadUInt64()),
				Down = InputEventHelper.FromBinary(reader.ReadUInt64()),
				Left = InputEventHelper.FromBinary(reader.ReadUInt64()),
				Right = InputEventHelper.FromBinary(reader.ReadUInt64()),

				Shoot = InputEventHelper.FromBinary(reader.ReadUInt64()),
				Bomb = InputEventHelper.FromBinary(reader.ReadUInt64()),
				Slow = InputEventHelper.FromBinary(reader.ReadUInt64()),
				Skip = InputEventHelper.FromBinary(reader.ReadUInt64()),

				J_Shoot = InputEventHelper.FromBinary(reader.ReadUInt64()),
				J_Bomb = InputEventHelper.FromBinary(reader.ReadUInt64()),
				J_Slow = InputEventHelper.FromBinary(reader.ReadUInt64()),
				J_Skip = InputEventHelper.FromBinary(reader.ReadUInt64()),

				J_Escape = InputEventHelper.FromBinary(reader.ReadUInt64()),
			};

			reader.Close();
			stream.Close();

			return keyconfig;
		}
	}

}
