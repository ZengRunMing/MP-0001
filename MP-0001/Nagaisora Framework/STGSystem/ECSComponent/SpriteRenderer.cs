using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class SpriteRenderer : STGComponent
	{
		public Sprite3D Sprite3D;

		public Texture2D Texture
		{
			get => Sprite3D.Texture;
			set => Sprite3D.Texture = value;
		}

		public Color Color
		{
			get => Sprite3D.Modulate;
			set => Sprite3D.Modulate = value;
		}

		public bool RegionEnabled
		{
			get => Sprite3D.RegionEnabled;
			set => Sprite3D.RegionEnabled = value;
		}

		public Rect2 RegionRect
		{
			get => Sprite3D.RegionRect;
			set => Sprite3D.RegionRect = value;
		}

		public bool FlipX
		{
			get => Sprite3D.FlipH;
			set => Sprite3D.FlipH = value;
		}

		public bool FlipY
		{
			get => Sprite3D.FlipV;
			set => Sprite3D.FlipV = value;
		}

		public string SpriteRendererName = "SpriteRenderer";

		public override void Initialize()
		{
			if (Sprite3D is null)
			{
				Sprite3D = BaseSTGEntity.GetNodeOrNull<Sprite3D>(SpriteRendererName);

				if (Sprite3D is null)
				{
					Sprite3D = new()
					{
						Name = SpriteRendererName,
						Layers = 2
					};

					BaseSTGEntity.AddChild(Sprite3D);
				}
			}
		}

		public override void Dispose()
		{
			BaseSTGEntity.RemoveChild(Sprite3D);
		}

		public override Variant _Get(StringName property)
		{
			if (property == nameof(Sprite3D))
			{
				return Sprite3D.GetPath();
			}

			return default;
		}

		public override bool _Set(StringName property, Variant value)
		{
			if (property == nameof(Sprite3D))
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
					{ "name", nameof(Sprite3D) },
					{ "type" , (int)Variant.Type.NodePath},
				}
			];
		}
	}
}
