using System;

using Godot;

namespace NagaisoraFramework.STGSystem
{
	using ECSComponent;
	using Godot.Collections;

	[Tool, GlobalClass]
	public partial class SpriteRenderComponent : STGComponent
	{
		public Sprite3D SpriteRenderer;

		public Texture2D Texture
		{
			get => SpriteRenderer.Texture;
			set => SpriteRenderer.Texture = value;
		}

		public Color Color
		{
			get => SpriteRenderer.Modulate;
			set => SpriteRenderer.Modulate = value;
		}

		public bool RegionEnabled
		{
			get => SpriteRenderer.RegionEnabled;
			set => SpriteRenderer.RegionEnabled = value;
		}

		public Rect2 RegionRect
		{
			get => SpriteRenderer.RegionRect;
			set => SpriteRenderer.RegionRect = value;
		}

		public bool FlipX
		{
			get => SpriteRenderer.FlipH;
			set => SpriteRenderer.FlipH = value;
		}

		public bool FlipY
		{
			get => SpriteRenderer.FlipV;
			set => SpriteRenderer.FlipV = value;
		}

		public string SpriteRendererName = "SpriteRenderer";

		public override void Initialize()
		{
			if (SpriteRenderer is null)
			{
				SpriteRenderer = BaseSTGEntity.GetNodeOrNull<Sprite3D>(SpriteRendererName);

				if (SpriteRenderer is null)
				{
					SpriteRenderer = new()
					{
						Name = SpriteRendererName,
						Layers = 2
					};

					BaseSTGEntity.AddChild(SpriteRenderer);
				}
			}
		}

		public override Variant _Get(StringName property)
		{
			if (property == nameof(SpriteRenderer))
			{
				return SpriteRenderer.GetPath();
			}

			return default;
		}

		public override bool _Set(StringName property, Variant value)
		{
			if (property == nameof(SpriteRenderer))
			{
				//SpriteRenderer = BaseSTGEntity.GetNodeOrNull<Sprite3D>(value.AsNodePath());
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
					{ "name", nameof(SpriteRenderer) },
					{ "type" , (int)Variant.Type.NodePath},
				}
			];
		}
	}
}
