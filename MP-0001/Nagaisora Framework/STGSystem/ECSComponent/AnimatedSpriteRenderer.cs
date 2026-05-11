using Godot;
using Godot.Collections;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[GlobalClass, Tool]
	public partial class AnimatedSpriteRenderer : STGComponent
	{
		public AnimatedSprite3D AnimatedSprite3D;

		public SpriteFrames SpriteFrames
		{
			get => AnimatedSprite3D.SpriteFrames;
			set => AnimatedSprite3D.SpriteFrames = value;
		}

		public Color Color
		{
			get => AnimatedSprite3D.Modulate;
			set => AnimatedSprite3D.Modulate = value;
		}

		public bool FlipX
		{
			get => AnimatedSprite3D.FlipH;
			set => AnimatedSprite3D.FlipH = value;
		}

		public bool FlipY
		{
			get => AnimatedSprite3D.FlipV;
			set => AnimatedSprite3D.FlipV = value;
		}

		public string AnimationName
		{
			get => AnimatedSprite3D.Animation;
			set => AnimatedSprite3D.Animation = value;
		}

		public string AnimatedSprite3DName = "AnimatedSprite3D";

		public AnimatedSpriteRenderer()
		{
			if (AnimatedSprite3D is null)
			{
				AnimatedSprite3D = BaseEntity.GetNodeOrNull<AnimatedSprite3D>(AnimatedSprite3DName);

				if (AnimatedSprite3D is null)
				{
					AnimatedSprite3D = new()
					{
						Name = AnimatedSprite3DName,
						Layers = 2
					};

					BaseEntity.AddChild(AnimatedSprite3D);
				}
			}
		}

		public override void Dispose()
		{
			BaseEntity.RemoveChild(AnimatedSprite3D);
			base.Dispose();
		}

		public override Variant _Get(StringName property)
		{
			throw new System.NotImplementedException();
		}

		public override bool _Set(StringName property, Variant value)
		{
			throw new System.NotImplementedException();
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			return
			[
				new()
				{
					{ "name", nameof(AnimatedSprite3D) },
					{ "type", (int)Variant.Type.Object },
				},
				new()
				{
					{ "name", nameof(SpriteFrames) },
					{ "type", (int)Variant.Type.Object },
				}
			];
		}
	}
}
