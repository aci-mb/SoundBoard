using AcillatemSoundBoard.Model;

namespace AcillatemSoundBoard.Services.SoundImplementation
{
	public interface ISoundFactory
	{
		/// <summary>
		/// Creates a new <see cref="ISound"/> instance for playback
		/// </summary>
		/// <returns></returns>
		ISound Create();

		/// <summary>
		/// Gets all supported extensions in the format of "*.extension", e. g. "*.mp3"
		/// </summary>
		string[] SupportedExtensions { get; }
	}
}