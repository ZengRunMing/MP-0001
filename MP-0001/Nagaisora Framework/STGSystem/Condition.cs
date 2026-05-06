using System;

namespace NagaisoraFramework.STGSystem
{
	public class Condition
	{
		public bool Flag = false;
		public bool LoopExecution = false;

		public Func<bool> ConditionFunc;
		public Action ExecuteAction;

		public Condition(Func<bool> conditionFunc, Action executeAction, bool loopExecution)
		{
			ConditionFunc = conditionFunc;
			ExecuteAction = executeAction;
			LoopExecution = loopExecution;
		}

		public void SetFlag()
		{
			if (LoopExecution)
			{
				return;
			}

			Flag = true;
		}

		public void ResetFlag()
		{
			Flag = false;
		}

		public void ConditionExecute()
		{
			if (LoopExecution)
			{
				ResetFlag();
			}

			if (Flag)
			{
				return;
			}

			if (ConditionFunc())
			{
				ExecuteAction();

				if (!LoopExecution)
				{
					SetFlag();
				}
			}
		}
	}
}
