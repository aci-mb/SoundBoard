using SoundBoard.Model;

namespace SoundBoard.Services.SoundImplementation
{
	public interface ISoundFactory
	{
		/// <summary>
		/// Creates a new <see cref="ISound"/> instance for playback
		/// </summary>
		/// <returns></returns>
		ISound Create();

		/// <summary>
		/// All supported extensions in the format of "*.extension".
		/// E. g. "*.mp3"
		/// </summary>
		string[] SupportedExtensions { get; }
	}
}