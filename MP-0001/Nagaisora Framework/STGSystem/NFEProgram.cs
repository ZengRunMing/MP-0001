
namespace NagaisoraFramework.STGSystem
{
	public class NFEProgram
	{
		public ExecuteSystem ExecuteSystem;
		public STGControler STGControler;

		public NFEProgram(ExecuteSystem executeSystem, STGControler controler)
		{
			ExecuteSystem = executeSystem;
			STGControler = controler;
		}

		public virtual void Main()
		{

		}
	}
}
