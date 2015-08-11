namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public interface ILanguageCommandWithResult<T> : ILanguageCommand
	{
		T Result { get; set; }
	}
}