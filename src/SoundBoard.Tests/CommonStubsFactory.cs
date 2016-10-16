using System;
using System.Collections.ObjectModel;
using System.Linq;
using AcillatemSoundBoard.Model;
using AcillatemSoundBoard.Services;
using AcillatemSoundBoard.Services.SoundImplementation;
using AcillatemSoundBoard.ViewModel;
using Rhino.Mocks;

namespace AcillatemSoundBoard.Tests
{
	internal static class CommonStubsFactory
	{
		public static IObservableSoundService StubObservableSoundService(ObservableCollection<ISound> initialCollection = null)
		{
			ObservableCollection<ISound> activeSounds = initialCollection ?? new ObservableCollection<ISound>();

			IObservableSoundService observableSoundService = MockRepository.GenerateStub<IObservableSoundService>();
			observableSoundService.Stub(service => service.ActiveSounds)
				.Return(new ReadOnlyObservableCollection<ISound>(activeSounds));

			observableSoundService.Stub(service => service.Add(Arg<ISound>.Is.NotNull))
			   .WhenCalled(invocation => activeSounds.Add((ISound)invocation.Arguments.First()));

			observableSoundService.Stub(service => service.Remove(Arg<ISound>.Is.NotNull))
			   .WhenCalled(invocation => activeSounds.Remove((ISound)invocation.Arguments.First()));

			return observableSoundService;
		}

		public static ISound StubClonableSoundWithRandomName()
		{
			ISound theSound = MockRepository.GenerateStub<ISound>();
			theSound.Name = Guid.NewGuid().ToString();

			ISound clonedSound = MockRepository.GenerateStub<ISound>();
			clonedSound.Name = theSound.Name;

			theSound.Stub(sound => sound.Clone()).Return(clonedSound);

			return theSound;
		}

		public static ISound StubClonableSound(string name = null)
		{
			ISound theSound = MockRepository.GenerateStub<ISound>();
			theSound.Name = name;

			ISound clonedSound = MockRepository.GenerateStub<ISound>();
			clonedSound.Name = name;

			theSound.Stub(sound => sound.Clone()).Return(clonedSound);

			return theSound;
		}

		public static ISoundFactory StubSoundFactory(string[] supportedExtensions = null)
		{
			ISoundFactory soundFactory = MockRepository.GenerateStub<ISoundFactory>();
			soundFactory.Stub(factory => factory.Create()).Return(null)
				.WhenCalled(invocation => invocation.ReturnValue = MockRepository.GenerateStub<ISound>());
			soundFactory.Stub(factory => factory.SupportedExtensions).Return(supportedExtensions);
			return soundFactory;
		}

		public static IMainWindowViewModel StubMainWindowViewModel(IObservableSoundService observableSoundService = null, ObservableCollection<ISound> initialSounds = null)
		{
			IMainWindowViewModel mainWindowViewModel = MockRepository.GenerateStub<IMainWindowViewModel>();

			mainWindowViewModel.Stub(model => model.SoundService)
				.Return(observableSoundService ?? StubObservableSoundService(initialSounds));

			return mainWindowViewModel;
		}
	}
}
