using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class KeyEvent : STGComponent
	{
		public KeyDownEvent Event;

		public KeyEvent(KeyDownEvent @event)
		{
			Event = @event;

			STGControler.RegisterKeyEvent(Event);
		}

		public override void Dispose()
		{
			STGControler.UnRegisterKeyEvent(Event);
			base.Dispose();
		}

		public override Variant _Get(StringName property) => default;

		public override bool _Set(StringName property, Variant value) => false;

		public override Array<Dictionary> _GetPropertyList() => [];
	}
}
