using Godot;
using System;
using System.IO;
using System.Text;

namespace NagaisoraFramework
{
	public class InputEventHelper
	{
		public static ulong ToBinary(InputEvent @event)
		{
			MemoryStream memory = new(8);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			memory.Position = 0;

			if (@event is InputEventKey key)
			{
				writer.Write((byte)0);
				writer.Write((byte)key.Location);
				memory.Position = 4;
				writer.Write((uint)key.PhysicalKeycode);
			}
			else if (@event is InputEventJoypadButton button)
			{
				writer.Write((byte)1);
				memory.Position = 4;
				writer.Write((uint)button.ButtonIndex);
			}
			else if (@event is InputEventJoypadMotion motion)
			{
				writer.Write((byte)2);
				memory.Position = 4;
				writer.Write((uint)motion.Axis);
			}
			else
			{
				throw new ArgumentException("Temporarily unsupported type");
			}

			writer.Close();

			byte[] bytes = memory.ToArray();
			return BitConverter.ToUInt64(bytes, 0);
		}

		public static InputEvent FromBinary(ulong data)
		{
			MemoryStream memory = new(BitConverter.GetBytes(data));
			BinaryReader reader = new(memory, Encoding.UTF8);

			byte type = reader.ReadByte();
			InputEvent @event;

			switch (type)
			{
				case 0:
					{
						KeyLocation location = (KeyLocation)reader.ReadByte();

						memory.Position = 4;

						Key key = (Key)reader.ReadUInt32();

						@event = new InputEventKey
						{
							Location = location,
							PhysicalKeycode = key,
						};
					}
					break;
				case 1:
					{
						memory.Position = 4;
						
						JoyButton button = (JoyButton)reader.ReadUInt32();

						@event = new InputEventJoypadButton
						{
							ButtonIndex = button,
						};
					}
					break;
				case 2:
					{
						memory.Position = 4;

						JoyAxis axis = (JoyAxis)reader.ReadUInt32();

						@event = new InputEventJoypadMotion
						{
							Axis = axis,
						};
					}
					break;
				default:
					throw new ArgumentException("Temporarily unsupported type");
			}

			reader.Close();
			memory.Close();

			return @event;
		}
	}
}
