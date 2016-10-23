using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoundBoard.Model;
using SoundBoard.Services.SoundImplementation.CsCore;

namespace SoundBoard.Tests.Services.SoundImplementation.CsCore
{
	[TestClass]
	public class CsCoreSoundFactoryTests
	{
		private CsCoreSoundFactory Target { get; set; }

		[TestInitialize]
		public void InititalizeBeforeEachTest()
		{
			Target = new CsCoreSoundFactory();
		}

		[TestMethod]
		public void Create__ReturnsISoundOfTypeCsCoreSound()
		{
			//Act
			ISound actual = Target.Create();

			//Assert
			actual.Should().BeOfType<CsCoreSound>();
		}

		[TestMethod]
		public void SupportedExtensions__ReturnsSupportedExtensionsOfCsCore()
		{
			//Arrange
			string[] expected =
			{
				"*.3g2", "*.3gp", "*.3gp2", "*.3gpp", "*.aac", "*.adt", "*.adts", "*.aif", "*.aifc", "*.aiff", "*.asf", "*.fla",
				"*.flac", "*.m2ts", "*.m4a", "*.m4v", "*.mov", "*.mp2", "*.mp3", "*.mp4", "*.mp4v", "*.mpeg3", "*.ogg",
				"*.wav", "*.wave", "*.wm", "*.wma", "*.wmv",
			};

			//Act
			string[] actual = Target.SupportedExtensions;

			//Assert
			actual.ShouldBeEquivalentTo(expected);
		}
	}
}
