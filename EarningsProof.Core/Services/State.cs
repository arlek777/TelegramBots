namespace EarningsProof.Core.Services
{
	public enum StateStep
	{
		FirstSentence,
		SecondSentence,
		Sum,
		ScreenShot
	}

	public class CurrentState
	{
		public StateStep Step { get; set; }

		public string FirstText { get; set; }

		public string SecondText { get; set; }

		public string Sum { get; set; }

		public string ScreenShotImg { get; set; }

		public List<int> MessageIds { get; set; } = new List<int>();
	}


	public static class State
	{
		private static readonly Dictionary<long, CurrentState> UserStates = new Dictionary<long, CurrentState>();

		public static void AddNewMessage(long userId, int id)
		{
			if (id == 0)
				return;

			UserStates.TryAdd(userId, new CurrentState());
			UserStates[userId].MessageIds.Add(id);
		}

		public static StateStep UpdateState(long userId, string text)
		{
			if (UserStates.TryAdd(userId, new CurrentState() { Step = StateStep.FirstSentence }))
			{
				return UserStates[userId].Step;
			}

			var currentState = UserStates[userId];

			switch (currentState.Step)
			{
				case StateStep.FirstSentence:
					currentState.Step = StateStep.SecondSentence;
					currentState.FirstText = text;
					break;
				case StateStep.SecondSentence:
					currentState.Step = StateStep.Sum;
					currentState.SecondText = text;
					break;
				case StateStep.Sum:
					currentState.Step = StateStep.ScreenShot;
					currentState.Sum = text;
					break;
				case StateStep.ScreenShot:
					currentState.Step = StateStep.FirstSentence;
					currentState.MessageIds.Clear();
					break;
			}

			return currentState.Step;
		}

		public static CurrentState CurrentStep(long userId) => UserStates.ContainsKey(userId) ? UserStates[userId] : null;
	}
}
