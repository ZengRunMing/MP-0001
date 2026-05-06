using Godot;

using System.Linq;
using System.Collections.Generic;

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;



namespace NagaisoraFramework.MediaSystem
{
	[GlobalClass]
	public partial class MidiPianoFormViewer : Control
	{
		[Export]
		public MidiControler MidiControler;

		[Export]
		public MidiPianoChannel[] MidiPianoChannels;

		public IEnumerable<Note> OriginalNotes;

		public IEnumerable<TrackChunk> MidiChannels;

		public IDictionary<int, List<Note>> Notes;

		public override void _Ready()
		{
			Notes = new Dictionary<int, List<Note>>();
		}

		public override void _Process(double delta)
		{
			Notes.Clear();

			foreach (MidiPianoChannel channel in MidiPianoChannels)
			{
				if (!Notes.ContainsKey(channel.ChannelID))
				{
					Notes.Add(channel.ChannelID, []);
				}
			}

			if (MidiControler is null || MidiControler.MidiNotes is null || MidiControler.Playback is null)
			{
				goto noteUpdate;
			}

			OriginalNotes = MidiControler.MidiNotes.AtTime((MetricTimeSpan)MidiControler.CurrentTime(TimeSpanType.Metric), MidiControler.Playback.TempoMap, LengthedObjectPart.Entire);

			if (OriginalNotes == null || !OriginalNotes.Any())
			{
				goto noteUpdate;
			}

			foreach (var note in OriginalNotes)
			{
				if (!Notes.ContainsKey(note.Channel))
				{
					Notes.Add(note.Channel, []);
				}

				Notes[note.Channel].Add(note);
			}

			noteUpdate:
			NoteUpdate();
		}

		public void NoteUpdate()
		{
			foreach (MidiPianoChannel channel in MidiPianoChannels)
			{
				if (!Notes.ContainsKey(channel.ChannelID))
				{
					continue;
				}

				channel.Notes = [.. Notes[channel.ChannelID]];
			}
		}
	}
}
