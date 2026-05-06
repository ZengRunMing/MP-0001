namespace NagaisoraFramework.STGSystem
{
	public class SpellCard
	{
		public STGControler STGControler;

		public string Name;
		public uint Score;
		public float TimeLimit;

		public SpellCard(string name, uint score, float timeLimit)
		{
			Name = name;
			Score = score;
			TimeLimit = timeLimit;
		}

		public virtual void Register(STGControler stgControler)
		{
			STGControler = stgControler;
		}

		public virtual void Start()
		{

		}

		public virtual void End()
		{

		}
	}
}
