namespace EarningsProof.Core
{
    public static class Constants
    {
        
    }

    public static class MessageTexts
    {
	    public const string Start = "Введите текст для первого сообщение. Если вам не нужно первое сообщение, введите 0.";
	    public const string SecondMessage = "Введите текст для второго сообщение. Если вам не нужно второе сообщение, введите 0.";
	    public const string SumMessage = "Введите сумму для скриншота с деньгами. Если вам не нужен скриншот со счетом, введите 0.";
	}

    public static class CallbackCommands
    {
        
    }

    public static class Commands
    {
        public const string Help = "/help";
        public const string Start = "/start";
        public const string Prepare = "/prepare";
        public const string Generate = "/generate";
		public const string Reset = "/reset";
	}
}
