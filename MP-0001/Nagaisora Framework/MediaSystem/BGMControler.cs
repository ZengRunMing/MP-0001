using Godot;

namespace NagaisoraFramework.MediaSystem
{
	using DataSystem;
	using System;

	[GlobalClass]
	public partial class BGMControler : Node
    {
		[Export]
		public AudioControler AudioControler;
		[Export]
		public MidiControler MidiControler;
		
		[Export]
		public int WaveIndex;
		[Export]
		public int MidiIndex;

		public BGMPackData BGMPackData;

		public bool WaveIsPlaying => AudioControler.Playing;

		public bool MidiIsPlaying => MidiControler.Playback != null && MidiControler.Playback.IsRunning;

		public WaveBGMData SelectWaveBGMDataOfIndex(int index)
		{
			WaveIndex = index;
			return BGMPackData.WaveBGMDatas[index];
		}

		public MidiBGMData SelectMidiBGMDataOfIndex(int index)
		{
			MidiIndex = index;
			return BGMPackData.MidiBGMDatas[index];
		}

		public void WavePlay()
		{
			AudioControler.SetAudioData(BGMPackData.WaveBGMDatas[WaveIndex]);
			AudioControler.Play();
		}

		public void MidiPlay()
		{
			MidiControler.SetMidiData(BGMPackData.MidiBGMDatas[MidiIndex]);
			MidiControler.Play();
		}

		public void WaveStop()
		{
			AudioControler.Stop();
		}

		public void MidiStop()
		{
			MidiControler.Stop();
		}

		public IBGMData BGMPlay(int i)
		{
			WaveStop();
			MidiStop();

			IBGMData packData = null;

			switch (GlobalData.ConfigData.BGMode)
			{
				case 0:
					if (BGMPackData.WaveBGMDatas == null || BGMPackData.WaveBGMDatas.Count == 0)
					{
						throw new Exception("Set [wave] play mode, but wave data is not found");
					}
					packData = SelectWaveBGMDataOfIndex(i);
					WavePlay();
					break;
				case 1:
					if (BGMPackData.MidiBGMDatas == null || BGMPackData.MidiBGMDatas.Count == 0)
					{
						throw new Exception("Set [midi] play mode, but midi data is not found");
					}
					packData = SelectMidiBGMDataOfIndex(i);
					MidiControler.SetOutputDevice(GlobalData.ConfigData.OutputDeviceName);
					MidiPlay();
					break;
				case 2:
					if (BGMPackData.WaveBGMDatas != null && BGMPackData.WaveBGMDatas.Count != 0)
					{
						packData = SelectWaveBGMDataOfIndex(i);
					}
					else if (BGMPackData.MidiBGMDatas != null && BGMPackData.MidiBGMDatas.Count != 0)
					{
						packData = SelectMidiBGMDataOfIndex(i);
					}
					else
					{
						packData = null;
					}
					break;
			}

			UnLockBGM(i);

			return packData;
		}

		public static void UnLockBGM(int i)
		{
			if (i >= GlobalData.ScoreData.BGMUnlockData.Length)
			{
				return;
			}

			GlobalData.ScoreData.BGMUnlockData[i] = true;
		}

		public new void Dispose()
		{
			MidiControler.Dispose();
			base.Dispose();
		}
	}
}
