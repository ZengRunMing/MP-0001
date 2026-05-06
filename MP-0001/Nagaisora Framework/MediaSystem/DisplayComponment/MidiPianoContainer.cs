using Godot;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;

namespace NagaisoraFramework.MediaSystem
{
	[GlobalClass]
	public partial class MidiPianoContainer : HBoxContainer
	{
		public Note[] Notes;

		[Export]
		public ColorRect[] NoteKeys;

		[Export]
		public Color KeyDownColor;

		public List<Color> KeyNormalColors;

		public override void _Ready()
		{
			KeyNormalColors = [];

			for (int i = 0; i < NoteKeys.Length; i++)
			{
				KeyNormalColors.Add(NoteKeys[i].Color);
			}
		}

		public override void _Process(double delta)
		{
			for (int i = 0; i < NoteKeys.Length; i++)
			{
				NoteKeys[i].Color = KeyNormalColors[i];
			}

			if (Notes is null || Notes.Length == 0)
			{
				return;
			}

			foreach (Note note in Notes)
			{
				NoteKeys[note.NoteNumber].Color = new Color(KeyDownColor.R, KeyDownColor.G, KeyDownColor.B, KeyDownColor.A);
			}
		}
	}
}
