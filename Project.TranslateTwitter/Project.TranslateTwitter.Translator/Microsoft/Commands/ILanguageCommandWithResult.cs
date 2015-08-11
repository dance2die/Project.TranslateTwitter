namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public interface ILanguageCommandWithResult<T>
	{
		T Result { get; set; }
	}
}