using System;
using System.Collections.Generic;

using Godot;
using Godot.Collections;

namespace NagaisoraFramework.DataSystem
{
	[Tool, GlobalClass]
	public abstract partial class NagaisoraFrameworkData : Resource
	{
		public Array<string> LinkDatasGUIDData
		{
			get
			{
				Array<string> strings = [];
				foreach (var guid in m_LinkDatasGUID)
				{
					strings.Add(BitConverter.ToString(guid.ToByteArray()).Replace('-', ' '));
				}

				return strings;
			}
			private set => _ = value;
		}

		public Guid[] LinkDatasGUID => [.. m_LinkDatasGUID];

		private List<Guid> m_LinkDatasGUID;

		public NagaisoraFrameworkData()
		{
			m_LinkDatasGUID = [];
		}

		public NagaisoraFrameworkData(Guid[] guids)
		{
			if (guids.Length > 16)
			{
				throw new ArgumentException("超出数据文件最大链接数");
			}

			m_LinkDatasGUID = new List<Guid>(guids);
		}

		public void Add(Guid guid)
		{
			if (m_LinkDatasGUID.Count >= 16)
			{
				throw new ArgumentException("超出数据文件最大链接数");
			}

			m_LinkDatasGUID.Add(guid);
		}

		public void Remove(Guid guid)
		{
			m_LinkDatasGUID.Remove(guid);
		}

		public void Clear()
		{
			m_LinkDatasGUID.Clear();
		}

		public override Variant _Get(StringName property)
		{
			if (property == nameof(LinkDatasGUIDData))
			{
				return LinkDatasGUIDData;
			}
			return default;
		}

		public override bool _Set(StringName property, Variant value)
		{
			if (property == nameof(LinkDatasGUIDData))
			{
				string[] valuestrings = value.AsStringArray();
				List<Guid> guids = [];
				foreach (var valuestring in valuestrings)
				{
					string guidstring = valuestring.Replace(' ', '-');
					if (guidstring.Split('-').Length < 16)
					{
						return false;
					}
					guids.Add(Guid.Parse(guidstring));
				}
				m_LinkDatasGUID = guids;
				return true;
			}
			return false;
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			return
			[
				new()
				{
					{ "name", nameof(LinkDatasGUIDData) },
					{ "type", (int)Variant.Type.PackedStringArray },
				}
			];
		}
	}
}
