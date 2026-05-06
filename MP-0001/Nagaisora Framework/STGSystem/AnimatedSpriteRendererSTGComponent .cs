using System;

using Godot;

namespace NagaisoraFramework.STGSystem
{
	using AnimationSystem;

	[GlobalClass]
	[Serializable]
	public partial class AnimatedSpriteRendererSTGComponent : STGEntity
	{
		[Export]
		public AnimatedSprite3D AnimatedSpriteRenderer;

		[Export]
		public SpriteFrames SpriteFrames
		{
			get => AnimatedSpriteRenderer.SpriteFrames;
			set => AnimatedSpriteRenderer.SpriteFrames = value;
		}

		[Export]
		public Color Color
		{
			get => AnimatedSpriteRenderer.Modulate;
			set => AnimatedSpriteRenderer.Modulate = value;
		}

		[Export]
		public bool FlipX
		{
			get => AnimatedSpriteRenderer.FlipH;
			set => AnimatedSpriteRenderer.FlipH = value;
		}

		[Export]
		public bool FlipY
		{
			get => AnimatedSpriteRenderer.FlipV;
			set => AnimatedSpriteRenderer.FlipV = value;
		}

		[Export]
		public string AnimationName
		{
			get => AnimatedSpriteRenderer.Animation;
			set => AnimatedSpriteRenderer.Animation = value;
		}

		[Export]
		public string AnimatedSpriteRendererName = "AnimatedSpriteRenderer";

		public override void _Ready()
		{
			base._Ready();

			if (AnimatedSpriteRenderer is null)
			{
				AnimatedSpriteRenderer = GetNodeOrNull<AnimatedSprite3D>(AnimatedSpriteRendererName);

				if (AnimatedSpriteRenderer is null)
				{
					AnimatedSpriteRenderer = new()
					{
						Name = AnimatedSpriteRendererName,
					};

					AddChild(AnimatedSpriteRenderer);
				}
			}
		}
	}
}
