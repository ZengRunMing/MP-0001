using Godot;

using Melanchall.DryWetMidi.Interaction;

namespace NagaisoraFramework.MediaSystem
{
	[GlobalClass]
	public partial class MidiPianoChannel : Control
	{
		public Note[] Notes;

		[Export]
		public int ChannelID;

		[Export]
		public Label ChannelNameLabel;

		[Export]
		public Label MessageLabel;

		[Export]
		public MidiPianoContainer MidiPianoContainer;

		public override void _Ready()
		{
			ChannelNameLabel.Text = (ChannelID + 1).ToString("CH00");
		}

		public override void _Process(double delta)
		{
			if (MessageLabel is not null)
			{
				MessageLabel.Text = "";
			}

			if (MidiPianoContainer is not null)
			{
				MidiPianoContainer.Notes = Notes;
			}

			if (Notes is null || Notes.Length == 0)
			{
				return;
			}

			foreach (Note note in Notes)
			{
				if (MessageLabel != null && MessageLabel.Text != "")
				{
					MessageLabel.Text += " | ";
				}

				if (MessageLabel != null)
				{
					MessageLabel.Text += note + " " + note.Velocity;
				}
			}
		}
	}
}