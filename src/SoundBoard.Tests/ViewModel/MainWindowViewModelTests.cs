using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using FluentAssertions;
using GongSolutions.Wpf.DragDrop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Rhino.Mocks;
using SoundBoard.Model;
using SoundBoard.Services;
using SoundBoard.Services.SoundImplementation;
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
		public void AddSoundCommandExecute__CallsDialogServiceOpenFileDialogWithCorrectArguments()
		{
			//Arrange
			IDialogService dialogServiceMock = MockRepository.GenerateMock<IDialogService>();

			string[] supportedExtensions = { "*.a", "*.b", "*.c"};

			Dictionary<string, string> expectedFilters = new Dictionary<string, string>
			{
				{
					Properties.Resources.MainWindowViewModel_AddSounds_Supported_Files,
					string.Join(Properties.Resources.MainWindowViewModel_AddSounds__Supported_Files_Separator, supportedExtensions)
				}
			};

			Target = CreateTarget(dialogService: dialogServiceMock, soundFactory: CommonStubsFactory.StubSoundFactory(supportedExtensions));
			Target.SelectedSoundBoard = new SoundBoard.Model.SoundBoard();

			dialogServiceMock.Expect(
				service =>
					service.OpenFileDialog(Properties.Resources.MainWindowViewModel_AddSounds_Choose_sound_files_to_add,
						expectedFilters))
				.Return(Enumerable.Empty<string>());

			//Act
			Target.Commands.AddSoundCommand.Execute(null);

			//Assert
			dialogServiceMock.VerifyAllExpectations();
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


		[TestMethod]
		public void DragOver__SetsDropTargetAdornerToInsert()
		{
			//Arrange
			Target = CreateTarget();

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.DropTargetAdorner = typeof (int);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.DropTargetAdorner.ShouldBeEquivalentTo(DropTargetAdorners.Insert);
		}

		[TestMethod]
		public void DragOver_TargetCollectionIsNull_SetsEffectsToNone()
		{
			//Arrange
			Target = CreateTarget();

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.All;
			dropInfo.Stub(info => info.Data).Return(CommonStubsFactory.StubClonableSoundWithRandomName());
			dropInfo.Stub(info => info.TargetCollection).Return(null);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.None);
		}

		[TestMethod]
		public void DragOver_DataIsASingleSoundAndTargetCollectionDoesNotContainSound_SetsEffectsToCopy()
		{
			//Arrange
			Target = CreateTarget();

			ISound droppedSound = CommonStubsFactory.StubClonableSoundWithRandomName();
			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>();
			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSound);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.Copy);
		}

		[TestMethod]
		public void DragOver_DataIsASingleSoundAndTargetCollectionContainsSound_SetsEffectsToMove()
		{
			//Arrange
			Target = CreateTarget();

			ISound droppedSound = CommonStubsFactory.StubClonableSoundWithRandomName();
			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>
			{
				droppedSound
			};
			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSound);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.Move);
		}

		[TestMethod]
		public void DragOver_DataIsASingleSoundAndTargetCollectionContainsSoundAndIsReadOnly_SetsEffectsToNone()
		{
			//Arrange
			Target = CreateTarget();

			ISound droppedSound = CommonStubsFactory.StubClonableSoundWithRandomName();
			ReadOnlyObservableCollection<ISound> targetCollection =
				new ReadOnlyObservableCollection<ISound>(new ObservableCollection<ISound>
				{
					droppedSound
				});
			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSound);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.None);
		}

		[TestMethod]
		public void DragOver_DataIsAnEnumerableOfISoundsAndTargetCollectionDoesNotContainThoseSounds_SetsEffectsToCopy()
		{
			//Arrange
			Target = CreateTarget();

			IEnumerable<ISound> droppedSounds = new ISound[]
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName()
			};
			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>();

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSounds);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.Copy);
		}

		[TestMethod]
		public void DragOver_DataIsAnEnumerableOfISoundsAndTargetCollectionContainsThoseSounds_SetsEffectsToMove()
		{
			//Arrange
			Target = CreateTarget();

			IEnumerable<ISound> droppedSounds = new ISound[]
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName()
			};
			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>(droppedSounds);

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSounds);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.Move);
		}

		[TestMethod]
		public void DragOver_DataIsAnEnumerableOfISoundsAndTargetCollectionContainsThoseSoundsAndIsReadOnly_SetsEffectsToNone()
		{
			//Arrange
			Target = CreateTarget();

			IEnumerable<ISound> droppedSounds = new ISound[]
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName()
			};
			ReadOnlyObservableCollection<ISound> targetCollection =
				new ReadOnlyObservableCollection<ISound>(
					new ObservableCollection<ISound>(droppedSounds));

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSounds);

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.None);
		}

		[TestMethod]
		public void DragOver_DataIsADataObject_SetsEffectsToCopy()
		{
			//Arrange
			Target = CreateTarget();

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Stub(info => info.TargetCollection).Return(new ObservableCollection<ISound>());
			dropInfo.Stub(info => info.Data).Return(new DataObject());

			//Act
			Target.DragOver(dropInfo);

			//Assert
			dropInfo.Effects.ShouldBeEquivalentTo(DragDropEffects.Copy);
		}


		[TestMethod]
		public void Drop_EffectIsCopyAndDataIsASingleISound_InsertsSoundIntoTargetCollection()
		{
			//Arrange
			Target = CreateTarget();

			ISound droppedSound = CommonStubsFactory.StubClonableSoundWithRandomName();
			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
			};

			ISound[] expectedCollection =
			{
				targetCollection[0],
				droppedSound,
				targetCollection[1],
				targetCollection[2],
			};


			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Copy;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSound);
			dropInfo.Stub(info => info.InsertIndex).Return(1);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.ShouldBeEquivalentTo(expectedCollection);
		}

		[TestMethod]
		public void Drop_EffectIsCopyAndTargetCollectionIsActiveSoundsAndDataIsASingleISound_AddsSoundToSoundService()
		{
			//Arrange
			Target = CreateTarget();

			ICollection<ISound> targetCollection = Target.SoundService.ActiveSounds;
			ISound droppedSound = CommonStubsFactory.StubClonableSoundWithRandomName();

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Copy;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSound);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.Single().ShouldBeEquivalentTo(droppedSound);
		}

		[TestMethod]
		public void Drop_EffectIsCopyAndDataIsAnEnumerableOfISounds_InsertsAllSoundsIntoTargetCollection()
		{
			//Arrange
			Target = CreateTarget();

			ISound[] droppedSounds = new ISound[]
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName()
			};

			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
			};

			ISound[] expectedCollection =
			{
				targetCollection[0],
				droppedSounds[0],
				droppedSounds[1],
				droppedSounds[2],
				targetCollection[1],
				targetCollection[2],
			};

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Copy;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSounds);
			dropInfo.Stub(info => info.InsertIndex).Return(1);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.ShouldBeEquivalentTo(expectedCollection);
		}

		[TestMethod]
		public void Drop_EffectIsCopyAndTargetCollectionIsActiveSoundsAndDataIsAnEnumerableOfISounds_AddsAllSoundsToSoundService()
		{
			//Arrange
			Target = CreateTarget();

			ICollection<ISound> targetCollection = Target.SoundService.ActiveSounds;
			IEnumerable<ISound> droppedSounds = new ISound[]
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName()
			};

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Copy;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSounds);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.ShouldBeEquivalentTo(droppedSounds);
		}

		[TestMethod]
		public void Drop_EffectIsCopyAndDataIsADataObjectContainingMultipleFiles_AddsASoundForEachSupportedFileToTargetCollection()
		{
			//Arrange
			ISoundFactory soundFactory = CommonStubsFactory.StubSoundFactory(new string[]
			{
				"*.mp3",
				"*.m4a",
				"*.ogg"
			});
			Target = CreateTarget(soundFactory: soundFactory);

			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>();
			DataObject droppeDataObject = new DataObject();

			string[] supportedFiles =
			{
				"Taking the hobbits to isengard.mp3",
				"The way i tend to be.m4a",
				"-Human.ogg"
			};

			StringCollection fileDropList = new StringCollection
			{
				"Unsupported File.wav",
				"Unsupported File2.agg",
				"Unsupported File2.bleurrg",
				"Unsupported File2.idk"
			};
			fileDropList.AddRange(supportedFiles);
			droppeDataObject.SetFileDropList(fileDropList);

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Copy;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppeDataObject);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.Should().OnlyContain(sound => supportedFiles.Any(s => s == sound.FileName));
		}

		[TestMethod]
		public void Drop_EffectIsCopyAndTargetCollectionIsActiveSoundsDataIsADataObjectContainingMultipleFiles_AddsASoundForEachSupportedFileToSoundService()
		{
			//Arrange
			ISoundFactory soundFactory = CommonStubsFactory.StubSoundFactory(new string[]
			{
				"*.mp3",
				"*.m4a",
				"*.ogg"
			});
			Target = CreateTarget(soundFactory: soundFactory);

			ICollection<ISound> targetCollection = Target.SoundService.ActiveSounds;
			DataObject droppeDataObject = new DataObject();

			string[] supportedFiles =
			{
				"Taking the hobbits to isengard.mp3",
				"The way i tend to be.m4a",
				"-Human.ogg"
			};

			StringCollection fileDropList = new StringCollection
			{
				"Unsupported File.wav",
				"Unsupported File2.agg",
				"Unsupported File2.bleurrg",
				"Unsupported File2.idk"
			};
			fileDropList.AddRange(supportedFiles);
			droppeDataObject.SetFileDropList(fileDropList);

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Copy;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppeDataObject);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.Should().OnlyContain(sound => supportedFiles.Any(s => s == sound.FileName));
		}


		[TestMethod]
		public void Drop_EffectIsMoveAndDataIsASingleISound_MovesSoundToCorrectPosition()
		{
			//Arrange
			const int moveTargetIndex = 2;
			Target = CreateTarget();

			ISound movedSound = CommonStubsFactory.StubClonableSound("Index2");
			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				movedSound,
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName()
			};

			IEnumerable<ISound> expectedCollection = new ObservableCollection<ISound>
			{
				targetCollection[0],
				targetCollection[2],
				movedSound, // index 2
				targetCollection[3]
			};

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Move;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(movedSound);
			dropInfo.Stub(info => info.InsertIndex).Return(moveTargetIndex);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.ShouldBeEquivalentTo(expectedCollection);
		}

		[TestMethod]
		public void Drop_EffectIsMoveAndDataIsAnEnumerableOfISounds_MovesAllSoundsToCorrectPositionInExistingOrder()
		{
			//Arrange
			Target = CreateTarget();

			ISound[] droppedSounds = {
				CommonStubsFactory.StubClonableSound("FirstMovedSound"),
				CommonStubsFactory.StubClonableSound("SecondMovedSound"),
				CommonStubsFactory.StubClonableSound("ThirdMovedSound"),
			};

			ObservableCollection<ISound> targetCollection = new ObservableCollection<ISound>
			{
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				CommonStubsFactory.StubClonableSoundWithRandomName(),
				droppedSounds[0],
				droppedSounds[1],
				droppedSounds[2],
			};

			ISound[] expectedCollection =
			{
				targetCollection[0],
				droppedSounds[0],
				droppedSounds[1],
				droppedSounds[2],
				targetCollection[1],
				targetCollection[2]
			};

			IDropInfo dropInfo = MockRepository.GenerateStub<IDropInfo>();
			dropInfo.Effects = DragDropEffects.Move;
			dropInfo.Stub(info => info.TargetCollection).Return(targetCollection);
			dropInfo.Stub(info => info.Data).Return(droppedSounds);
			dropInfo.Stub(info => info.InsertIndex).Return(1);

			//Act
			Target.Drop(dropInfo);

			//Assert
			targetCollection.ShouldBeEquivalentTo(expectedCollection, options => options.WithStrictOrdering());
		}

		private static MainWindowViewModel CreateTarget(ISoundBoardRepository soundBoardRepository = null,
			IDialogService dialogService = null, IKernel container = null, IObservableSoundService soundService = null,
			ISoundFactory soundFactory = null)
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
				soundFactory ?? CommonStubsFactory.StubSoundFactory(new string[] { "*.a", "*.b"}));
		}
	}
}