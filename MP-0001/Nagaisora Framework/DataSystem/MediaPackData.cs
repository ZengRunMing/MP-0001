using System;
using System.Collections.Generic;

using Godot;

namespace NagaisoraFramework.DataSystem
{
	using System.Collections;

	public class SoundEffectDataPack
    {
        public const string DataHead = "NSED";


    }

    public struct SoundEffectData
    {
        public AudioStreamWav AudioStreamWav;

        public string DataPath;

        public string Name;
    }

    public abstract class IBGMData
    {
        public abstract string Name { get; set; }

        public abstract string DataPath { get; set; }

        public abstract TimeSpan StartTime { get; set; }
        public abstract TimeSpan LoopStartTime { get; set; }
        public abstract TimeSpan LoopEndTime { get; set; }

        public abstract IBGMData Copy();

		public byte[] ToEditorHeadBinary()
		{
			MemoryStream memory = new(new byte[512]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			writer.Write(StartTime.Ticks);
			writer.Write(LoopStartTime.Ticks);
			writer.Write(LoopEndTime.Ticks);
			writer.Write(Name.ToCharArray());

			memory.Position = 112;

			writer.Write(DataPath.ToCharArray());

			writer.Close();
			return memory.ToArray();
		}

		public static IBGMData FromEditorHeadBinary(byte[] binary, bool wave)
		{
			if (binary.Length != 512)
			{
				throw new Exception("WaveBGMData Editor Head Binary Length Error");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			TimeSpan startTime = TimeSpan.FromTicks(reader.ReadInt64());
			TimeSpan loopStartTime = TimeSpan.FromTicks(reader.ReadInt64());
			TimeSpan loopEndTime = TimeSpan.FromTicks(reader.ReadInt64());

			byte[] stringbuffer = reader.ReadBytes(88);
			string name = StringHelper.GetStringFromBinary(stringbuffer, Encoding.UTF8);

			memory.Position = 112;

			byte[] dataPathBuffer = reader.ReadBytes(400);
			string dataPath = StringHelper.GetStringFromBinary(dataPathBuffer, Encoding.UTF8);

			reader.Close();
			memory.Close();

			IBGMData data;

			if (wave)
			{
				data = new WaveBGMData();
			}
			else
			{
				data = new MidiBGMData();
			}

			data.Name = name;
			data.StartTime = startTime;
			data.LoopStartTime = loopStartTime;
			data.LoopEndTime = loopEndTime;
			data.DataPath = dataPath;

			return data;
		}
	}


    public class WaveBGMData : IBGMData
    {
        public override string Name { get; set; } = "";

		public AudioStreamWav AudioStreamWav { get; set; }

        public override string DataPath { get; set; } = "";

        public override TimeSpan StartTime { get; set; }
        public override TimeSpan LoopStartTime { get; set; }
        public override TimeSpan LoopEndTime { get; set; }

        public WaveBGMData()
        {

        }

		public WaveBGMData(AudioStreamWav audioClip, TimeSpan startime, TimeSpan loopstartime, TimeSpan loopendtime)
		{
            AudioStreamWav = audioClip;
			StartTime = startime;
			LoopStartTime = loopstartime;
			LoopEndTime = loopendtime;
		}

		public WaveBGMData(AudioStreamWav audioClip, string name, TimeSpan startime, TimeSpan loopstartime, TimeSpan loopendtime)
		{
			AudioStreamWav = audioClip;
			Name = name;
			StartTime = startime;
			LoopStartTime = loopstartime;
			LoopEndTime = loopendtime;
		}
        public WaveBGMData(string dataPath, string name, TimeSpan startime, TimeSpan loopstartime, TimeSpan loopendtime)
        {
            DataPath = dataPath;
            Name = name;
            StartTime = startime;
            LoopStartTime = loopstartime;
            LoopEndTime = loopendtime;
        }

        public override IBGMData Copy()
        {
            return new WaveBGMData(DataPath, Name, StartTime, LoopStartTime, LoopEndTime);
        }

		public static WaveBGMData FromEditorHeadBinary(byte[] binary)
		{
			return FromEditorHeadBinary(binary, true) as WaveBGMData;
		}

		public byte[] ToHeadBinary(long position, int length, int frequency)
        {
            MemoryStream memory = new(new byte[128]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

            writer.Write(position);
            writer.Write(length);
            writer.Write(frequency);

			writer.Write(StartTime.Ticks);
            writer.Write(LoopStartTime.Ticks);
            writer.Write(LoopEndTime.Ticks);

            writer.Write(Name.ToCharArray());

            writer.Close();

            return memory.ToArray();
		}

        public static (long position, int length, int frequency, WaveBGMData data) FromHeadBinary(byte[] binary)
        {
            if (binary.Length != 128)
            {
                throw new Exception("WaveBGMData Head Binary Length Error");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);
            
            long position = reader.ReadInt64();
            int length = reader.ReadInt32();
            int frequency = reader.ReadInt32();

            TimeSpan startTime = TimeSpan.FromTicks(reader.ReadInt64());
            TimeSpan loopStartTime = TimeSpan.FromTicks(reader.ReadInt64());
            TimeSpan loopEndTime = TimeSpan.FromTicks(reader.ReadInt64());

            byte[] stringbuffer = reader.ReadBytes(88);
            string name = StringHelper.GetStringFromBinary(stringbuffer, Encoding.UTF8);

            reader.Close();
            memory.Close();

			WaveBGMData data = new()
			{
                Name = name,
                StartTime = startTime,
                LoopStartTime = loopStartTime,
                LoopEndTime = loopEndTime
            };

            return (position, length, frequency, data);
		}
	}


    public class MidiBGMData : IBGMData
    {
        public override string Name { get; set; } = "";

        public MemoryStream MidiData;

        public override string DataPath { get; set; } = "";

        public override TimeSpan StartTime { get; set; }
        public override TimeSpan LoopStartTime { get; set; }
        public override TimeSpan LoopEndTime { get; set; }

        public MidiBGMData()
        {

        }

        public MidiBGMData(MemoryStream midiData, TimeSpan startime, TimeSpan loopstartime, TimeSpan loopendtime)
        {
            MidiData = midiData;
            StartTime = startime;
            LoopStartTime = loopstartime;
            LoopEndTime = loopendtime;
        }

        public MidiBGMData(MemoryStream midiData, string name, TimeSpan startime, TimeSpan loopstartime, TimeSpan loopendtime)
        {
			MidiData = midiData;
			Name = name;
            StartTime = startime;
            LoopStartTime = loopstartime;
            LoopEndTime = loopendtime;
        }

        public MidiBGMData(string filepath, MemoryStream midiData, string name, TimeSpan startime, TimeSpan loopstartime, TimeSpan loopendtime)
        {
            DataPath = filepath;
            MidiData = midiData;
            Name = name;
            StartTime = startime;
            LoopStartTime = loopstartime;
            LoopEndTime = loopendtime;
        }

        public override IBGMData Copy()
        {
            return new MidiBGMData(DataPath, MidiData, Name, StartTime, LoopStartTime, LoopEndTime);
        }

		public static MidiBGMData FromEditorHeadBinary(byte[] binary)
		{
			return FromEditorHeadBinary(binary, false) as MidiBGMData;
		}

		public byte[] ToHeadBinary(long position, int length)
		{
			MemoryStream memory = new(new byte[128]);
			BinaryWriter writer = new(memory, Encoding.UTF8, true);

			writer.Write(position);
			writer.Write(length);

            memory.Position = 16; // skip 4 bytes

			writer.Write(StartTime.Ticks);
			writer.Write(LoopStartTime.Ticks);
			writer.Write(LoopEndTime.Ticks);

			writer.Write(Name.ToCharArray());

			writer.Close();

			return memory.ToArray();
		}

		public static (long position, int length, MidiBGMData data) FromHeadBinary(byte[] binary)
		{
			if (binary.Length != 128)
			{
				throw new Exception("MidiBGMData Head Binary Length Error");
			}

			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			long position = reader.ReadInt64();
			int length = reader.ReadInt32();

            memory.Position = 16; // skip 4 bytes

			TimeSpan startTime = TimeSpan.FromTicks(reader.ReadInt64());
			TimeSpan loopStartTime = TimeSpan.FromTicks(reader.ReadInt64());
			TimeSpan loopEndTime = TimeSpan.FromTicks(reader.ReadInt64());

			byte[] stringbuffer = reader.ReadBytes(88);
			string name = StringHelper.GetStringFromBinary(stringbuffer, Encoding.UTF8);

			reader.Close();
            memory.Close();

			MidiBGMData data = new()
			{
				Name = name,
				StartTime = startTime,
				LoopStartTime = loopStartTime,
				LoopEndTime = loopEndTime
			};

			return (position, length, data);
		}
	}

	public partial class BGMPackData : NagaisoraFrameworkData
	{
		public const string DataHead = "NBGM";

		public const uint Version = 0X00000001;

		public List<WaveBGMData> WaveBGMDatas;
		public List<MidiBGMData> MidiBGMDatas;

		public bool MidiPlayable;

		public byte[] ToBinary()
        {
			MemoryStream TotalMemory = new();

			MemoryStream HeadMemory = new();
			MemoryStream DataMemory = new();

			BinaryWriter HeadWriter = new(HeadMemory, Encoding.UTF8, true);
			BinaryWriter DataWriter = new(DataMemory, Encoding.UTF8, true);

            HeadMemory.Position = 0;
            DataMemory.Position = 0;

            HeadWriter.Write(DataHead.ToCharArray());
            HeadWriter.Write(Version);

			byte[] bytes = new byte[8];

			BitArray bitArray = new(bytes);

			bitArray[0] = MidiPlayable;

			HeadWriter.Write(bytes);

			HeadWriter.Write(WaveBGMDatas != null ? WaveBGMDatas.Count : 0);
			HeadWriter.Write(MidiBGMDatas != null ? MidiBGMDatas.Count : 0);

			HeadMemory.Position = 256;

			Guid[] guids = LinkDatasGUID;

			foreach (Guid guid in guids)
			{
				HeadWriter.Write(guid.ToByteArray());
			}

			if (HeadMemory.Position < 512)
			{
				HeadWriter.Write([255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255]);
			}

			HeadMemory.Position = 512;

            foreach (WaveBGMData data in WaveBGMDatas)
            {
				byte[] AudioData = data.AudioStreamWav.Data;

                HeadWriter.Write(data.ToHeadBinary(DataMemory.Position, AudioData.Length, data.AudioStreamWav.MixRate));

				DataWriter.Write(AudioData);
            }

			foreach (MidiBGMData data in MidiBGMDatas)
			{
				HeadWriter.Write(data.ToHeadBinary(DataMemory.Position, (int)data.MidiData.Length));

				DataWriter.Write(data.MidiData.ToArray());
			}

			long remain = HeadMemory.Length % 512;
			HeadWriter.Write(new byte[remain > 0 ? 512 - remain : 0]);

			HeadWriter.Close();
            DataWriter.Close();

            byte[] HeadBinary = HeadMemory.ToArray();
            byte[] DataBinary = DataMemory.ToArray();

            TotalMemory.Position = 0;

			TotalMemory.Write(HeadBinary, 0, HeadBinary.Length);
            TotalMemory.Write(DataBinary, 0, DataBinary.Length);

            return TotalMemory.ToArray();
		}

        public static BGMPackData FromBinary(byte[] binary)
        {
			MemoryStream memory = new(binary);
			BinaryReader reader = new(memory, Encoding.UTF8);

			memory.Position = 0;

			string ReadHead = new(reader.ReadChars(DataHead.Length));
			if (ReadHead != DataHead)
			{
				throw new InvalidDataException($"数据标识头不匹配");
			}

			uint ReadVersion = reader.ReadUInt32();
			if (ReadVersion != Version)
			{
				throw new InvalidDataException($"数据发行版本不匹配");
			}

			byte[] bytes = reader.ReadBytes(8);

			BitArray bitArray = new(bytes);



            int WaveLength = reader.ReadInt32();
			int MidiLength = reader.ReadInt32();

			int TotalLength = WaveLength + MidiLength;

			int ILength = TotalLength / 4;
			ILength += TotalLength % 4 != 0 ? 1 : 0;

			memory.Position = 512 + (ILength * 512);

			List<byte> dataBinary = [];
            while (memory.Position < memory.Length)
            {
				dataBinary.Add(reader.ReadByte());
			}

			MemoryStream DataMemory = new([.. dataBinary]);
			BinaryReader DataReader = new(DataMemory, Encoding.UTF8);

			memory.Position = 512;

			List<WaveBGMData> waveBGMDatas = [];
			List<MidiBGMData> midiBGMDatas = [];

            for (int i = 0; i < WaveLength; i++)
            {
                (long position, int length, int frequency, WaveBGMData data) = WaveBGMData.FromHeadBinary(reader.ReadBytes(128));

				DataMemory.Position = position;
				byte[] databuffer = DataReader.ReadBytes(length);
				data.AudioStreamWav = new()
				{
					Format = AudioStreamWav.FormatEnum.Qoa,
					MixRate = frequency,
					Stereo = true,
					Data = databuffer,
				};

				waveBGMDatas.Add(data);
			}

			for (int i = 0; i < MidiLength; i++)
			{
				(long position, int length, MidiBGMData data) = MidiBGMData.FromHeadBinary(reader.ReadBytes(128));

				DataMemory.Position = position;
				byte[] databuffer = DataReader.ReadBytes(length);
				data.MidiData = new(databuffer);

				midiBGMDatas.Add(data);
			}

			BGMPackData BGMPackData = new()
			{
                WaveBGMDatas = waveBGMDatas,
				MidiBGMDatas = midiBGMDatas,
            };

            return BGMPackData;
        }

        public static FileInfo[] ListDirectoryAllFile(string Path)
        {
            DirectoryInfo direction = new(Path);
            return direction.GetFiles("*.wav", SearchOption.AllDirectories);
        }
    }
}
