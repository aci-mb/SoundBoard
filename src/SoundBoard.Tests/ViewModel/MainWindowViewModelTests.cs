using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
	public class MainWindowViewModelTests
	{
		public MainWindowViewModel Target { get; set; }

		[TestMethod]
		public void AddSoundCommandCanExecute_SelectedSoundBoardIsNull_ReturnsFalse()
		{
			Target = CreateTarget();

			Target.Commands.AddSoundCommand.CanExecute(null).Should().BeFalse();
		}

		[TestMethod]
		public void AddSoundCommandCanExecute_SelectedSoundBoardIsNotNull_ReturnsTrue()
		{
			Target = CreateTarget();

			Target.SelectedSoundBoard = new SoundBoard.Model.SoundBoard();

			Target.Commands.AddSoundCommand.CanExecute(null).Should().BeTrue();
		}

		[TestMethod]
		public void AddSoundCommandExecute__AddsSoundToSoundsCollection()
		{
			//Arrange
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			string[] returnedFiles =
			{
				@"C:\Creeping Death.mp3",
				@"C:\Out of Control.wma",
				@"C:\Bell.ogg"
			};

			Target = CreateTarget(dialogService: stub);
			Target.SelectedSoundBoard = new SoundBoard.Model.SoundBoard();

			stub.Stub(service => service.OpenFileDialog(null, null)).IgnoreArguments().Return(returnedFiles);

			//Act
			Target.Commands.AddSoundCommand.Execute(null);

			//Assert
			Target.SelectedSoundBoard.Sounds.Should()
				.Contain(sound => sound.FileName == returnedFiles[0])
				.And.Contain(sound => sound.FileName == returnedFiles[1])
				.And.Contain(sound => sound.FileName == returnedFiles[2]);
		}

		[TestMethod]
		public void AddSoundCommandExecute__AddedSoundIsSelected()
		{
			//Arrange
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			IEnumerable<string> returnedFiles = new[] {@"C:\SomeNewFile.mp3"};

			stub.Stub(service => service.OpenFileDialog(null, null)).IgnoreArguments().Return(returnedFiles);

			Target = CreateTarget(dialogService: stub);

			Target.SelectedSoundBoard = new SoundBoard.Model.SoundBoard();

			//Act
			Target.Commands.AddSoundCommand.Execute(null);

			//Assert
			Target.SelectedSound.FileName.Should().Be(returnedFiles.First());
		}


		[TestMethod]
		public void RemoveSoundCommandCanExecute_SelectedSoundIsNull_ReturnsFalse()
		{
			Target = CreateTarget();

			Target.Commands.RemoveSoundCommand.CanExecute(null).Should().BeFalse();
		}

		[TestMethod]
		public void RemoveSoundCommandCanExecute_SelectedSoundIsNotNull_ReturnsTrue()
		{
			Target = CreateTarget();

			Target.SelectedSound = MockRepository.GenerateStub<ISound>();


			Target.Commands.RemoveSoundCommand.CanExecute(null).Should().BeTrue();
		}

		[TestMethod]
		public void RemoveSoundCommandExecute__RemovesSelectedSoundFromSounds()
		{
			//Arrange
			ISound soundToRemove = MockRepository.GenerateStub<ISound>();

			Target = CreateTarget();
			Target.SelectedSoundBoard = new SoundBoard.Model.SoundBoard
			{
				Sounds = new ObservableCollection<ISound> {soundToRemove}
			};

			Target.SelectedSound = soundToRemove;

			//Act
			Target.Commands.RemoveSoundCommand.Execute(soundToRemove);

			//Assert
			Target.SelectedSoundBoard.Sounds.Should().NotContain(soundToRemove);
		}


		[TestMethod]
		public void AddSoundBoardCommandCanExecute__ReturnsTrue()
		{
			Target = CreateTarget();

			Target.Commands.AddSoundBoardCommand.CanExecute(null).Should().BeTrue();
		}

		[TestMethod]
		public void AddSoundBoardCommandExecute__CallsNameDialogWithMainWindowAsParentParameter()
		{
			//Arrange
			MainWindow expectedParent = new MainWindow();
			IDialogService mock = MockRepository.GenerateMock<IDialogService>();
			IKernel container = new StandardKernel();
			container.Bind<MainWindow>().ToConstant(expectedParent);

			Target = CreateTarget(dialogService: mock, container: container);

			mock.Expect(service => service.NameDialog(
				Arg<Window>.Is.Same(expectedParent),
				Arg<string>.Is.Anything,
				Arg<string>.Is.Anything,
				Arg<string>.Is.Anything)).Return(string.Empty);

			//Act
			Target.Commands.AddSoundBoardCommand.Execute(null);

			//Assert
			mock.VerifyAllExpectations();
		}

		[TestMethod]
		public void AddSoundBoardCommandExecute_NameDialogReturnsNotNull_AddsSoundBoardToSoundBoardsCollection()
		{
			//Arrange
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			const string returnedSoundBoardName = "My new soundboard";

			Target = CreateTarget(dialogService: stub);

			stub.Stub(service => service.NameDialog(null, null, null, null)).IgnoreArguments().Return(returnedSoundBoardName);

			//Act
			Target.Commands.AddSoundBoardCommand.Execute(null);

			//Assert
			Target.SoundBoards.Should().Contain(board => board.Name == returnedSoundBoardName);
		}

		[TestMethod]
		public void AddSoundBoardCommandExecute_NameDialogReturnsNull_DoesNotAddSoundBoardToSoundBoardsCollection()
		{
			//Arrange
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();

			Target = CreateTarget(dialogService: stub);

			stub.Stub(service => service.NameDialog(null, null, null, null)).IgnoreArguments().Return(null);

			//Act
			Target.Commands.AddSoundBoardCommand.Execute(null);

			//Assert
			Target.SoundBoards.Should().BeEmpty();
		}

		[TestMethod]
		public void AddSoundBoardCommandExecute_NameDialogReturnsNotNull_AddedSoundBoardIsSelected()
		{
			//Arrange
			const string nameOfNewSoundBoard = "My new sound board";

			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			stub.Stub(service => stub.NameDialog(null, null, null, null)).IgnoreArguments().Return(nameOfNewSoundBoard);

			Target = CreateTarget(dialogService: stub);

			//Act
			Target.Commands.AddSoundBoardCommand.Execute(null);

			//Assert
			Target.SelectedSoundBoard.Name.Should().Be(nameOfNewSoundBoard);
		}

		[TestMethod]
		public void RemoveSoundBoardCommandCanExecute_SelectedSoundBoardIsNull_ReturnsFalse()
		{
			Target = CreateTarget();

			Target.Commands.RemoveSoundBoardCommand.CanExecute(null).Should().BeFalse();
		}

		[TestMethod]
		public void RemoveSoundBoardCommandCanExecute_SelectedSoundBoardIsNotNull_ReturnsTrue()
		{
			Target = CreateTarget();

			Target.SelectedSoundBoard = new SoundBoard.Model.SoundBoard();

			Target.Commands.RemoveSoundBoardCommand.CanExecute(null).Should().BeTrue();
		}

		[TestMethod]
		public void RemoveSoundBoardCommandExecute__RemovesSelectedSoundBoardFromSoundsBoard()
		{
			//Arrange
			SoundBoard.Model.SoundBoard soundBoardToRemove = new SoundBoard.Model.SoundBoard();

			Target = CreateTarget();
			Target.SoundBoards.Add(soundBoardToRemove);

			Target.SelectedSoundBoard = soundBoardToRemove;

			//Act
			Target.Commands.RemoveSoundBoardCommand.Execute(soundBoardToRemove);

			//Assert
			Target.SoundBoards.Should().NotContain(soundBoardToRemove);
		}


		[TestMethod]
		public void ActivateSingleSoundCommand_ParameterIsNull_ThrowsArgumentNullException()
		{
			Target = CreateTarget();

			var execute = new Action(() => Target.Commands.ActivateSingleSoundCommand.Execute(null));

			execute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void ActivateSingleSoundCommand_ParameterIsNotOfTypeSound_ThrowsArgumentException()
		{
			Target = CreateTarget();

			var execute = new Action(() => Target.Commands.ActivateSingleSoundCommand.Execute("Not a sound"));

			execute.ShouldThrow<ArgumentException>()
				.WithMessage("Parameter must be of type " + typeof (ISound) + "*")
				.And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void ActivateSingleSoundCommand_ParameterIsNotNull_AddsCloneOfSoundToActiveSounds()
		{
			//Arrange
			ObservableCollection<ISound> activeSounds = new ObservableCollection<ISound>();
			IObservableSoundService observableSoundService = CommonStubsFactory.StubObservableSoundService(activeSounds);

			Target = CreateTarget(soundService: observableSoundService);
			ISound sound = CommonStubsFactory.StubClonableSoundWithRandomName();

			//Act
			Target.Commands.ActivateSingleSoundCommand.Execute(sound);

			//Assert
			activeSounds.Should().NotContain(s => ReferenceEquals(s, sound));
			activeSounds.Single().ShouldBeEquivalentTo(sound);
		}

		[TestMethod]
		public void ActivateSingleSoundCommand_SelectedSoundIsNotNull_AddedSoundIsSelected()
		{
			//Arrange
			Target = CreateTarget();
			ISound sound = CommonStubsFactory.StubClonableSoundWithRandomName();

			//Act
			Target.Commands.ActivateSingleSoundCommand.Execute(sound);

			//Assert
			Target.SelectedActiveSound.ShouldBeEquivalentTo(sound);
		}


		[TestMethod]
		public void DeactivateSingleSoundCommand_ParameterIsNull_ThrowsArgumentNullException()
		{
			Target = CreateTarget();

			var execute = new Action(() => Target.Commands.DeactivateSingleSoundCommand.Execute(null));

			execute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void DeactivateSingleSoundCommand_ParameterIsNotOfTypeSound_ThrowsArgumentException()
		{
			Target = CreateTarget();

			var execute = new Action(() => Target.Commands.DeactivateSingleSoundCommand.Execute("Not a sound"));

			execute.ShouldThrow<ArgumentException>()
				.WithMessage("Parameter must be of type " + typeof (ISound) + "*")
				.And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void DeactivateSoundCommandExecute_SelectedActiveSoundIsNotNull_RemovesSoundFromActiveSounds()
		{
			//Arrange
			Target = CreateTarget();
			ISound sound = MockRepository.GenerateStub<ISound>();
			Target.SoundService.Add(sound);

			//Act
			Target.Commands.DeactivateSingleSoundCommand.Execute(sound);

			//Assert
			Target.SoundService.ActiveSounds.Should().NotContain(sound);
		}


		[TestMethod]
		public void ToggleSoundIsLoopedCommandExecute_ParameterIsNull_ThrowsArgumentNullException()
		{
			Target = CreateTarget();

			var actionName = new Action(() => Target.Commands.ToggleSoundIsLoopedCommand.Execute(null));

			actionName.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void ToggleSoundIsLoopedCommandExecute_ParameterIsNotOfTypeSound_ThrowsArgumentException()
		{
			Target = CreateTarget();

			var execute = new Action(() => Target.Commands.ToggleSoundIsLoopedCommand.Execute(string.Empty));

			execute.ShouldThrow<ArgumentException>()
				.WithMessage("Parameter must be of type " + typeof (ISound) + "*")
				.And.ParamName.ShouldBeEquivalentTo("parameter");
		}

		[TestMethod]
		public void ToggleSoundIsLoopedCommandExecute_IsLoopedWasTrue_IsLoopedIsNowFalse()
		{
			ISound sound = CommonStubsFactory.StubClonableSoundWithRandomName();
			sound.IsLooped = true;
			Target = CreateTarget();

			Target.Commands.ToggleSoundIsLoopedCommand.Execute(sound);

			sound.IsLooped.Should().BeFalse();
		}

		[TestMethod]
		public void ToggleSoundIsLoopedCommandExecute_IsLoopedWasFalse_IsLoopedIsNowTrue()
		{
			ISound sound = CommonStubsFactory.StubClonableSoundWithRandomName();
			sound.IsLooped = false;
			Target = CreateTarget();

			Target.Commands.ToggleSoundIsLoopedCommand.Execute(sound);

			sound.IsLooped.Should().BeTrue();
		}

		[TestMethod]
		public void SaveDataCommandExecute_QuestionDialogResultIsFalse_DoesNotCallSaveSoundBoards()
		{
			//Arrange
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			stub.Stub(service => service.QuestionDialog(null, null, null)).IgnoreArguments().Return(false);

			ISoundBoardRepository mock = MockRepository.GenerateMock<ISoundBoardRepository>();
			mock.Expect(repository => repository.SetSoundBoards(null)).IgnoreArguments()
				.Repeat.Never();

			Target = CreateTarget(dialogService: stub, soundBoardRepository: mock);

			//Act
			Target.Commands.ShutdownAppCommand.Execute(null);

			//Assert
			mock.VerifyAllExpectations();
		}

		[TestMethod]
		public void SaveDataCommandExecute_QuestionDialogResultIsTrue_DoesNotCallSaveSoundBoards()
		{
			//Arrange
			IDialogService stub = MockRepository.GenerateStub<IDialogService>();
			stub.Stub(service => service.QuestionDialog(null, null, null)).IgnoreArguments().Return(true);

			ISoundBoardRepository mock = MockRepository.GenerateMock<ISoundBoardRepository>();
			mock.Stub(repository => repository.AreSoundBoardsDifferent(null)).IgnoreArguments().Return(true);
			mock.Expect(repository => repository.SetSoundBoards(null)).IgnoreArguments()
				.Repeat.Once();

			Target = CreateTarget(dialogService: stub, soundBoardRepository: mock);

			//Act
			Target.Commands.ShutdownAppCommand.Execute(null);

			//Assert
			mock.VerifyAllExpectations();
		}

		[TestMethod]
		public void SaveDataCommandExecute_AreSoundBoardsChangedReturnsTrue_QuestionDialogIsCalled()
		{
			//Arrange
			ISoundBoardRepository stub = MockRepository.GenerateStub<ISoundBoardRepository>();
			stub.Stub(repository => repository.AreSoundBoardsDifferent(null)).IgnoreArguments().Return(true);

			IDialogService mock = MockRepository.GenerateMock<IDialogService>();
			mock.Expect(service => service.QuestionDialog(null, null, null))
				.IgnoreArguments()
				.Return(false);

			Target = CreateTarget(dialogService: mock, soundBoardRepository: stub);

			//Act
			Target.Commands.ShutdownAppCommand.Execute(null);

			//Assert
			mock.VerifyAllExpectations();
		}

		[TestMethod]
		public void SaveDataCommandExecute_AreSoundBoardsChangedReturnsFalse_QuestionDialogIsNotCalled()
		{
			//Arrange
			ISoundBoardRepository stub = MockRepository.GenerateStub<ISoundBoardRepository>();
			stub.Stub(repository => repository.AreSoundBoardsDifferent(null)).IgnoreArguments().Return(false);

			IDialogService mock = MockRepository.GenerateMock<IDialogService>();
			mock.Expect(service => service.QuestionDialog(null, null, null))
				.IgnoreArguments()
				.Return(false)
				.Repeat.Never();

			Target = CreateTarget(dialogService: mock, soundBoardRepository: stub);

			//Act
			Target.Commands.ShutdownAppCommand.Execute(null);

			//Assert
			mock.VerifyAllExpectations();
		}

		private static MainWindowViewModel CreateTarget(ISoundBoardRepository soundBoardRepository = null,
			IDialogService dialogService = null, IKernel container = null, IObservableSoundService soundService = null)
		{
			if (container == null)
			{
				container = new StandardKernel();
				container.Bind<MainWindow>().ToMethod(context => new MainWindow());
			}

			return new MainWindowViewModel(
				soundBoardRepository ?? MockRepository.GenerateStub<ISoundBoardRepository>(),
				dialogService ?? MockRepository.GenerateStub<IDialogService>(),
				soundService ?? CommonStubsFactory.StubObservableSoundService(),
				container,
				CommonStubsFactory.StubSoundFactory());
		}
	}
}