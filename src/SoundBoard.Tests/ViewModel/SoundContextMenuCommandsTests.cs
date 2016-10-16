using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Rhino.Mocks;
using SoundBoard.Model;
using SoundBoard.Services;
using SoundBoard.View;
using SoundBoard.ViewModel;

namespace SoundBoard.Tests.ViewModel
{
	[TestClass]
	public class SoundContextMenuCommandsTests
	{
		public SoundContextMenuCommands Target { get; set; }

		[TestInitialize]
		public void InitializeBeforeEachTest()
		{
			Target = CreateTargetWithDefaultStubs();
		}

		private SoundContextMenuCommands CreateTargetWithDefaultStubs(
			IMainWindowViewModel mainWindowViewModel = null,
			IDialogService dialogService = null,
			IKernel container = null)
		{
			if (container == null)
			{
				container = new StandardKernel();
				container.Bind<MainWindow>().ToMethod(context => new MainWindow());
			}

			SoundContextMenuCommands soundContextMenuCommands = new SoundContextMenuCommands(
				mainWindowViewModel ?? MockRepository.GenerateStub<IMainWindowViewModel>(),
				dialogService ?? MockRepository.GenerateStub<IDialogService>(),
				container);

			return soundContextMenuCommands;
		}

		[TestMethod]
		public void Constructor_MainWindowViewModelIsNull_ThrowsArgumentNullException()
		{
			var constructor = new Action(() => new SoundContextMenuCommands(
				null,
				MockRepository.GenerateStub<IDialogService>(),
				MockRepository.GenerateStub<IKernel>()));

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("mainWindowViewModel");
		}

		[TestMethod]
		public void Constructor_DialogServiceIsNull_ThrowsArgumentNullException()
		{
			var constructor = new Action(() => new SoundContextMenuCommands(
				MockRepository.GenerateStub<IMainWindowViewModel>(),
				null,
				MockRepository.GenerateStub<IKernel>()));

			constructor.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("dialogService");
		}

		[TestMethod]
		public void Commands__ContainsItemForActivateSoundsCommand()
		{
			Target.Commands.Should().Contain(command => ReferenceEquals(command.Command, Target.ActivateSoundsCommand));
		}

		[TestMethod]
		public void Commands__ContainsItemForRemoveSoundsCommand()
		{
			Target.Commands.Should().Contain(command => ReferenceEquals(command.Command, Target.RemoveSoundsCommand));
		}

		[TestMethod]
		public void Commands__ContainsItemForRenameSingleSoundCommand()
		{
			Target.Commands.Should().Contain(command => ReferenceEquals(command.Command, Target.RenameSoundCommand));
		}

		[TestMethod]
		public void ActivateSoundsCommandCanExecute_ParameterIsNull_ReturnsFalse()
		{
			Target.ActivateSoundsCommand.CanExecute(null)
				.Should().BeFalse();
		}

		[TestMethod]
		public void ActivateSoundsCommandCanExecute_ParameterIsNotOfTypeIList_ThrowsArgumentException()
		{
			Target.ActivateSoundsCommand.CanExecute("Not an IList")
				.Should().BeFalse();
		}

		[TestMethod]
		public void ActivateSoundsCommandCanExecute_ParameterIsEmptyCollection_ReturnsFalse()
		{
			Target.ActivateSoundsCommand.CanExecute(new Collection<ISound>())
				.Should().BeFalse();
		}

		[TestMethod]
		public void ActivateSoundsCommandCanExecute_ParameterIsCollectionContaining1Item_ReturnsTrue()
		{
			Target.ActivateSoundsCommand.CanExecute(new Collection<ISound> {MockRepository.GenerateStub<ISound>()})
				.Should().BeTrue();
		}

		[TestMethod]
		public void ActivateSoundsCommandCanExecute_ParameterIsCollectionContaining3Items_ReturnsTrue()
		{
			Target.ActivateSoundsCommand.CanExecute(new Collection<ISound>
			{
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>()
			})
				.Should().BeTrue();
		}

		[TestMethod]
		public void ActivateSoundsCommandExecute_ParameterIsNull_ThrowsArgumentNullException()
		{
			var activateSoundsCommandExecute = new Action(() => Target.ActivateSoundsCommand.Execute(null));

			activateSoundsCommandExecute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void ActivateSoundsCommandExecute_ParameterIsNotOfTypeIList_ThrowsArgumentException()
		{
			var activateSoundsCommandExecute = new Action(() => Target.ActivateSoundsCommand.Execute("Not an IList"));

			activateSoundsCommandExecute.ShouldThrow<ArgumentException>()
				.WithMessage("Parameter must be of type " + typeof (IList) + "*")
				.And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void ActivateSoundsCommandExecute_ParameterContainsIListWith3Sounds_Adds3EquivalentClonesToActiveSounds()
		{
			//Arrange
			List<ISound> selectedSounds = new List<ISound>
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName()
			};
			IMainWindowViewModel mainWindowViewModel = CommonStubsFactory.StubMainWindowViewModel();
			Target = CreateTargetWithDefaultStubs(mainWindowViewModel);

			//Act
			Target.ActivateSoundsCommand.Execute(selectedSounds);

			//Assert
			for (var i = 0; i < selectedSounds.Count; i++)
			{
				mainWindowViewModel.SoundService.ActiveSounds[i].ShouldBeEquivalentTo(selectedSounds[i],
					"for each selected sound there must be an identical active sound");
				mainWindowViewModel.SoundService.ActiveSounds[i].Should().NotBeSameAs(selectedSounds[i], "it must be a clone, not the same reference");
			}
		}

		[TestMethod]
		public void RenameSoundCommandCanExecute_ParameterIsNull_ReturnsFalse()
		{
			Target.RenameSoundCommand.CanExecute(null)
				.Should().BeFalse();
		}

		[TestMethod]
		public void RenameSoundCommandCanExecute_ParameterIsNotOfTypeIList_ThrowsArgumentException()
		{
			Target.RenameSoundCommand.CanExecute("Not an IList")
				.Should().BeFalse();
		}

		[TestMethod]
		public void RenameSoundCommandCanExecute_ParameterIsEmptyCollection_ReturnsFalse()
		{
			Target.RenameSoundCommand.CanExecute(new Collection<ISound>())
				.Should().BeFalse();
		}

		[TestMethod]
		public void RenameSoundCommandCanExecute_ParameterIsCollectionContaining1Item_ReturnsTrue()
		{
			Target.RenameSoundCommand.CanExecute(new Collection<ISound> {MockRepository.GenerateStub<ISound>()})
				.Should().BeTrue();
		}

		[TestMethod]
		public void RenameSoundCommandCanExecute_ParameterIsCollectionContaining3Items_ReturnsFalse()
		{
			Target.RenameSoundCommand.CanExecute(new Collection<ISound>
			{
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>()
			})
				.Should().BeFalse("Only one item may be selected for renaming");
		}

		[TestMethod]
		public void RenameSoundCommandExecute_ParameterIsNull_ThrowsArgumentNullException()
		{
			var renameSoundCommandExecute = new Action(() => Target.RenameSoundCommand.Execute(null));

			renameSoundCommandExecute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void RenameSoundCommandExecute_ParameterIsNotOfTypeIList_ThrowsArgumentException()
		{
			var renameSoundCommandExecute = new Action(() => Target.RenameSoundCommand.Execute("Not an IList"));

			renameSoundCommandExecute.ShouldThrow<ArgumentException>()
				.WithMessage("Parameter must be of type " + typeof (IList) + "*")
				.And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void RenameSoundCommandExecute_ParameterIsEmptyCollection_ThrowsArgumentException()
		{
			var renameSoundCommandExecute = new Action(() => Target.RenameSoundCommand.Execute(new Collection<ISound>()));

			renameSoundCommandExecute.ShouldThrow<ArgumentException>()
				.WithMessage("Only a single sound may be selected for renaming*")
				.And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void RenameSoundCommandExecute_DialogReturnsNotNull_NameOfSelectedSoundEqualsGivenName()
		{
			//Arrange
			const string expected = "New Name";

			ISound selectedSound = CommonStubsFactory.StubClonableSound("Old Name");
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			Target = CreateTargetWithDefaultStubs(dialogService: stub);

			stub.Stub(service => service.NameDialog(null, null, null, null)).IgnoreArguments().Return(expected);

			//Act
			Target.RenameSoundCommand.Execute(new Collection<ISound> {selectedSound});

			//Assert
			selectedSound.Name.Should().Be(expected);
		}

		[TestMethod]
		public void RenameSoundCommandExecute_DialogReturnsNull_OldNameIsRetained()
		{
			//Arrange
			const string expected = "Old Name";

			ISound selectedSound = CommonStubsFactory.StubClonableSound("Old Name");
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			Target = CreateTargetWithDefaultStubs(dialogService: stub);

			stub.Stub(service => service.NameDialog(null, null, null, null)).IgnoreArguments().Return(null);

			//Act
			Target.RenameSoundCommand.Execute(new Collection<ISound> {selectedSound});

			//Assert
			selectedSound.Name.Should().Be(expected);
		}

		[TestMethod]
		public void RenameSoundCommandExecute__NameDialogIsCalledWithTitlePromptAndInitialName()
		{
			ISound selectedSound = CommonStubsFactory.StubClonableSound("Old Name");
			IDialogService mock = MockRepository.GenerateMock<IDialogService>();
			Target = CreateTargetWithDefaultStubs(dialogService: mock);

			mock.Expect(service => service.NameDialog(
				Arg<Window>.Is.Anything,
				Arg<string>.Is.Anything,
				Arg<string>.Is.Anything,
				Arg<string>.Is.Equal(selectedSound.Name)))
				.Return(null);

			//Act
			Target.RenameSoundCommand.Execute(new Collection<ISound> {selectedSound});

			//Assert
			mock.VerifyAllExpectations();
		}

		[TestMethod]
		public void RemoveSoundsCommandCanExecute_ParameterIsNull_ReturnsFalse()
		{
			Target.RemoveSoundsCommand.CanExecute(null)
				.Should().BeFalse();
		}

		[TestMethod]
		public void RemoveSoundsCommandCanExecute_ParameterIsNotOfTypeIList_ReturnsFalse()
		{
			Target.RemoveSoundsCommand.CanExecute("Not an IList")
				.Should().BeFalse();
		}

		[TestMethod]
		public void RemoveSoundsCommandCanExecute_ParameterIsEmptyIList_ReturnsFalse()
		{
			Target.RemoveSoundsCommand.CanExecute(new Collection<ISound>())
				.Should().BeFalse();
		}

		[TestMethod]
		public void RemoveSoundsCommandCanExecute_SoundBoardIsNull_ReturnsFalse()
		{
			Target.RemoveSoundsCommand.CanExecute(new Collection<ISound> {MockRepository.GenerateStub<ISound>()})
				.Should().BeFalse();
		}

		[TestMethod]
		public void RemoveSoundsCommandCanExecute_SoundBoardIsNotNullAndParameterIsIListWith3Sounds_ReturnsTrue()
		{
			var stub = MockRepository.GenerateStub<IMainWindowViewModel>();
			stub.SelectedSoundBoard = new SoundBoard.Model.SoundBoard();
			Target = CreateTargetWithDefaultStubs(stub);

			Target.RemoveSoundsCommand.CanExecute(new Collection<ISound>
			{
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>()
			})
				.Should().BeTrue();
		}

		[TestMethod]
		public void RemoveSoundsCommandCanExecute_SoundBoardIsNotNullAndParameterIsIListWith1Sound_ReturnsTrue()
		{
			var stub = MockRepository.GenerateStub<IMainWindowViewModel>();
			stub.SelectedSoundBoard = new SoundBoard.Model.SoundBoard();
			Target = CreateTargetWithDefaultStubs(stub);

			Target.RemoveSoundsCommand.CanExecute(new Collection<ISound> {MockRepository.GenerateStub<ISound>()})
				.Should().BeTrue();
		}

		[TestMethod]
		public void RemoveSoundsCommandExecute_ParameterIsNull_ThrowsArgumentNullException()
		{
			var removeSoundsCommandExecute = new Action(() => Target.RemoveSoundsCommand.Execute(null));

			removeSoundsCommandExecute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void RemoveSoundsCommandExecute_ParameterIsNotOfTypeIList_ThrowsArgumentException()
		{
			var removeSoundsCommandExecute = new Action(() => Target.RemoveSoundsCommand.Execute("Not an IList"));

			removeSoundsCommandExecute.ShouldThrow<ArgumentException>()
				.WithMessage("Parameter must be of type " + typeof (IList) + "*")
				.And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void
			RemoveSoundsCommandExecute_SoundBoardIsNotNullAndParameterIsIListWith3Sounds_RemovesThoseSoundsFromSelectedSoundboard
			()
		{
			//Arrange
			var selectedSounds = new List<ISound>
			{
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>()
			};

			var stub = MockRepository.GenerateStub<IMainWindowViewModel>();
			stub.SelectedSoundBoard = new SoundBoard.Model.SoundBoard
			{
				Sounds = new ObservableCollection<ISound>(selectedSounds)
				{
					MockRepository.GenerateStub<ISound>(),
					MockRepository.GenerateStub<ISound>()
				}
			};

			Target = CreateTargetWithDefaultStubs(stub);

			//Act
			Target.RemoveSoundsCommand.Execute(selectedSounds);

			//Assert
			stub.SelectedSoundBoard.Sounds
				.Should().NotContain(selectedSounds)
				.And.HaveCount(2);
		}
	}
}