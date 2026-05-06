using System;

using Godot;

namespace NagaisoraFramework.STGSystem
{
	[Tool]
	[GlobalClass]
	[Serializable]
	public partial class ParticleSystemSTGComponent : STGEntity
	{
		[Export]
		public GpuParticles2D[] ParticleSystems;

		[ExportToolButton("Shoot")]
		public Callable Button => Callable.From(Shoot);

		[ExportToolButton("Save")]
		public Callable SaveButton => Callable.From(Save);

		public override void Init()
		{
			base.Init();
		}

		public void Shoot()
		{
			if (ParticleSystems is null)
			{
				return;
			}

			foreach (var particleSystem in ParticleSystems)
			{
				particleSystem.Restart();
			}
		}

		public void Save()
		{
			GD.Print(ParticleSystems[0].ProcessMaterial.ResourceName);

			byte[] bytes = GD.VarToBytes(ParticleSystems[0].ProcessMaterial);
			GD.Print(StringHelper.HexTable(bytes));

			Variant variant = GD.BytesToVar(bytes);
			Material material = variant.As<Material>();

			GD.Print(material.ResourceName);

			ResourceLoader.Load(material.ResourceName);
		}
	}
}
