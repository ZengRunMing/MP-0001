using System;
using System.Collections.Generic;

using Godot;

namespace NagaisoraFramework.MediaSystem
{
	[GlobalClass]
	public partial class SoundEffectControler : Node
	{
		[Export]
		public float Volume;

		public SoundEffectData[] SoundEffectDatas;

		public IDictionary<string, SoundEffectData> SoundEffectDataDictionary = new Dictionary<string, SoundEffectData>();

		public List<AudioStreamPlayer2D> AudioStreamPlayers;

		public override void _Ready()
		{
			Initialization();

			if (SoundEffectDatas is null)
			{
				GD.PushWarning("参数 SoundEffectData 为空，将无法播放音效");
				return;
			}

			foreach (SoundEffectData data in SoundEffectDatas)
			{
				if (data.SEType == SEType.AsName)
				{
					SoundEffectDataDictionary.Add(data.Name, data);
					continue;
				}

				SoundEffectDataDictionary.Add(Enum.GetName(typeof(SEType), data.SEType), data);
			}
		}

		public void PlaySE(string name)
		{
			if (!SoundEffectDataDictionary.ContainsKey(name)) // 检查音效名称是否存在于字典中
			{
				return;
			}

			AudioStreamWav clip = SoundEffectDataDictionary[name].AudioStreamWav; // 获取音频剪辑

			foreach (AudioStreamPlayer2D player in AudioStreamPlayers)
			{
				if (player.Stream is null) // 检查音频源的音频剪辑是否为 null
				{
					if (player.Stream != clip) // 如果当前音频源的音频剪辑不是要播放的剪辑
					{
						continue; // 跳过当前音频源
					}
				}

				if (player.Playing) // 如果音频源正在播放
				{
					player.Stop(); // 停止当前音频源的播放
				}

				player.Stream = clip; // 设置音频源的音频剪辑为要播放的剪辑
				player.PitchScale = 1f; // 设置音频源的时间缩放为 1（正常速度）
				if (SoundEffectDataDictionary[name].TimeScale) // 如果音频剪辑需要时间缩放
				{
					player.PitchScale = (float)Engine.TimeScale; // 设置音频源的时间缩放为当前时间缩放
				}

				player.VolumeLinear = Volume; // 设置音频源的音量为 SoundEffectControler 的音量
				player.Play(); // 播放音频源
				return; // 成功播放音效后直接返回
			}

			AddAudioScource(AudioStreamPlayers.Count - 1); // 如果没有可用的音频源，则添加一个新的音频源
		}

		public void AddAudioScource(int a)
		{
			AudioStreamPlayer2D obj = new()
			{
				Name = "SE-" + a.ToString("00")
			};

			AddChild(obj);

			AudioStreamPlayers.Add(obj);
		}

		public void Initialization()
		{
			AudioStreamPlayers = [];

			for (int a = 0; a < 16; a++)
			{
				AddAudioScource(a);
			}
		}

		public void SEReset()
		{
			foreach (AudioStreamPlayer2D player in AudioStreamPlayers)
			{
				player.Dispose();
			}

			AudioStreamPlayers.Clear();
			Initialization();
		}
	}

	[Serializable]
	public struct SoundEffectData
	{
		public SEType SEType;
		public string Name;
		public bool TimeScale;
		public AudioStreamWav AudioStreamWav;

		public SoundEffectData(AudioStreamWav audioClip, string name = "", bool timescale = false, SEType type = SEType.AsName)
		{
			if (type == SEType.AsName)
			{
				Name = name;
				SEType = SEType.AsName;
			}
			else
			{
				Name = Enum.GetName(typeof(SEType), type);
				SEType = type;
			}
			TimeScale = timescale;
			AudioStreamWav = audioClip;
		}
	}
}

