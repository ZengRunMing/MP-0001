using Godot;
using Godot.Collections;
using NagaisoraFramework.EntityComponentSystem;
using System;

namespace NagaisoraFramework.STGSystem.ECSComponent
{
	[Tool]
	[GlobalClass]

	public partial class ParticleSystem : STGComponent
	{
		public Array<GpuParticles3D> ParticleSystems = [];

		[ExportToolButton("Shoot")]
		public Callable Button => Callable.From(Shoot);

		[ExportToolButton("Save")]
		public Callable SaveButton => Callable.From(Save);

		public ParticleSystem(STGControler controler, STGEntity entity, int particleSystemCount) : base(controler, entity)
		{
			for (int i = 0; i < particleSystemCount; i++)
			{
				GpuParticles3D gpuParticles3D = new()
				{
					Name = i.ToString("00")
				};

				ParticleSystems.Add(gpuParticles3D);
				BaseSTGEntity.AddChild(gpuParticles3D);
			}
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

		public override Variant _Get(StringName property)
		{
			throw new NotImplementedException();
		}

		public override bool _Set(StringName property, Variant value)
		{
			throw new NotImplementedException();
		}

		public override Array<Dictionary> _GetPropertyList()
		{
			throw new NotImplementedException();
		}
	}
}
