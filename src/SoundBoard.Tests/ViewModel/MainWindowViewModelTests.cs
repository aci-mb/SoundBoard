using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AcillatemSoundBoard.Model;
using AcillatemSoundBoard.Services;
using AcillatemSoundBoard.View;
using AcillatemSoundBoard.ViewModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Rhino.Mocks;

namespace AcillatemSoundBoard.Tests.ViewModel
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        public MainWindowViewModel Target { get; set; }

        [TestMethod]
        public void AddSoundCommandCanExecute_SelectedSoundBoardIsNull_ReturnsFalse()
        {
            Target = CreateTargetWithStubs();

            Target.Commands.AddSoundCommand.CanExecute(null).Should().BeFalse();
        }

        [TestMethod]
        public void AddSoundCommandCanExecute_SelectedSoundBoardIsNotNull_ReturnsTrue()
        {
            Target = CreateTargetWithStubs();
            
            Target.SelectedSoundBoard = new SoundBoard();

            Target.Commands.AddSoundCommand.CanExecute(null).Should().BeTrue();
        }

        [TestMethod]
        public void AddSoundCommandExecute__AddsSoundToSoundsCollection()
        {
            //Arrange
            IDialogService stub = MockRepository.GenerateStub<IDialogService>();
            var returnedFiles = new string[]
            {
                @"C:\Creeping Death.mp3",
                @"C:\Out of Control.wma",
                @"C:\Bell.ogg"
            };

            Target = CreateTargetWithStubs(dialogService: stub);
            Target.SelectedSoundBoard = new SoundBoard();

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
            Target = CreateTargetWithStubs(dialogService: stub);
            Target.SelectedSoundBoard = new SoundBoard();

            //Act
            Target.Commands.AddSoundCommand.Execute(null);

            //Assert
            Target.SelectedSound.FileName.Should().Be(returnedFiles.First());
        }


        

        [TestMethod]
        public void RemoveSoundCommandCanExecute_SelectedSoundIsNull_ReturnsFalse()
        {
            Target = CreateTargetWithStubs();

            Target.Commands.RemoveSoundCommand.CanExecute(null).Should().BeFalse();
        }

        [TestMethod]
        public void RemoveSoundCommandCanExecute_SelectedSoundIsNotNull_ReturnsTrue()
        {
            Target = CreateTargetWithStubs();

	        Target.SelectedSound = MockRepository.GenerateStub<ISound>();


			Target.Commands.RemoveSoundCommand.CanExecute(null).Should().BeTrue();
        }

        [TestMethod]
        public void RemoveSoundCommandExecute__RemovesSelectedSoundFromSounds()
        {
            //Arrange
            ISound soundToRemove = MockRepository.GenerateStub<ISound>();

            Target = CreateTargetWithStubs();
            Target.SelectedSoundBoard = new SoundBoard
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
            Target = CreateTargetWithStubs();

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

            Target = CreateTargetWithStubs(dialogService: mock, container: container);

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

            Target = CreateTargetWithStubs(dialogService: stub);

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

            Target = CreateTargetWithStubs(dialogService: stub);

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

            Target = CreateTargetWithStubs(dialogService: stub);

            //Act
            Target.Commands.AddSoundBoardCommand.Execute(null);

            //Assert
            Target.SelectedSoundBoard.Name.Should().Be(nameOfNewSoundBoard);
        }

        [TestMethod]
        public void RemoveSoundBoardCommandCanExecute_SelectedSoundBoardIsNull_ReturnsFalse()
        {
            Target = CreateTargetWithStubs();

            Target.Commands.RemoveSoundBoardCommand.CanExecute(null).Should().BeFalse();
        }

        [TestMethod]
        public void RemoveSoundBoardCommandCanExecute_SelectedSoundBoardIsNotNull_ReturnsTrue()
        {
            Target = CreateTargetWithStubs();

            Target.SelectedSoundBoard = new SoundBoard();

            Target.Commands.RemoveSoundBoardCommand.CanExecute(null).Should().BeTrue();
        }

        [TestMethod]
        public void RemoveSoundBoardCommandExecute__RemovesSelectedSoundBoardFromSoundsBoard()
        {
            //Arrange
            SoundBoard soundBoardToRemove = new SoundBoard();

            Target = CreateTargetWithStubs();
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
            Target = CreateTargetWithStubs();

            var execute = new Action(() => Target.Commands.ActivateSingleSoundCommand.Execute(null));

            execute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
        }

        [TestMethod]
        public void ActivateSingleSoundCommand_ParameterIsNotOfTypeSound_ThrowsArgumentException()
        {
            Target = CreateTargetWithStubs();

            var execute = new Action(() => Target.Commands.ActivateSingleSoundCommand.Execute("Not a sound"));

            execute.ShouldThrow<ArgumentException>()
                .WithMessage("Parameter must be of type " + typeof(ISound) + "*")
                .And.ParamName.ShouldBeEquivalentTo("parameter");
        }

        [TestMethod]
        public void ActivateSingleSoundCommand_ParameterIsNotNull_AddsCloneOfSoundToActiveSounds()
        {
            //Arrange
            Target = CreateTargetWithStubs();
            ISound sound = StubSoundWithRandomName();

            //Act
            Target.Commands.ActivateSingleSoundCommand.Execute(sound);

            //Assert
            Target.ActiveSounds.Should().NotContain(s => ReferenceEquals(s, sound));
            Target.ActiveSounds.First().ShouldBeEquivalentTo(sound);
        }

        [TestMethod]
        public void ActivateSingleSoundCommand_SelectedSoundIsNotNull_AddedSoundIsSelected()
        {
            //Arrange
            Target = CreateTargetWithStubs();
            ISound sound = StubSoundWithRandomName();

            //Act
            Target.Commands.ActivateSingleSoundCommand.Execute(sound);

            //Assert
            Target.SelectedActiveSound.ShouldBeEquivalentTo(sound);
        }


        [TestMethod]
        public void DeactivateSingleSoundCommand_ParameterIsNull_ThrowsArgumentNullException()
        {
            Target = CreateTargetWithStubs();

            var execute = new Action(() => Target.Commands.DeactivateSingleSoundCommand.Execute(null));

            execute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
        }

        [TestMethod]
        public void DeactivateSingleSoundCommand_ParameterIsNotOfTypeSound_ThrowsArgumentException()
        {
            Target = CreateTargetWithStubs();

            var execute = new Action(() => Target.Commands.DeactivateSingleSoundCommand.Execute("Not a sound"));

            execute.ShouldThrow<ArgumentException>()
                .WithMessage("Parameter must be of type " + typeof(ISound) + "*")
                .And.ParamName.ShouldBeEquivalentTo("parameter");
        }

        [TestMethod]
        public void DeactivateSoundCommandExecute_SelectedActiveSoundIsNotNull_RemovesSoundFromActiveSounds()
        {
            //Arrange
            Target = CreateTargetWithStubs();
            ISound sound = MockRepository.GenerateStub<ISound>();
            Target.ActiveSounds.Add(sound);

            //Act
            Target.Commands.DeactivateSingleSoundCommand.Execute(sound);

            //Assert
            Target.ActiveSounds.Should().NotContain(sound);
        }


        [TestMethod]
        public void ToggleSoundIsLoopedCommandExecute_ParameterIsNull_ThrowsArgumentNullException()
        {
            Target = CreateTargetWithStubs();

            var actionName = new Action(() => Target.Commands.ToggleSoundIsLoopedCommand.Execute(null));

            actionName.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
        }

        [TestMethod]
        public void ToggleSoundIsLoopedCommandExecute_ParameterIsNotOfTypeSound_ThrowsArgumentException()
        {
            Target = CreateTargetWithStubs();

            var execute = new Action(() => Target.Commands.ToggleSoundIsLoopedCommand.Execute(string.Empty));

            execute.ShouldThrow<ArgumentException>()
                .WithMessage("Parameter must be of type " + typeof(ISound) + "*")
                .And.ParamName.ShouldBeEquivalentTo("parameter");
        }

        [TestMethod]
        public void ToggleSoundIsLoopedCommandExecute_IsLoopedWasTrue_IsLoopedIsNowFalse()
        {
	        ISound sound = StubSoundWithRandomName();
	        sound.IsLooped = true;
            Target = CreateTargetWithStubs();
            
            Target.Commands.ToggleSoundIsLoopedCommand.Execute(sound);

            sound.IsLooped.Should().BeFalse();
        }

        [TestMethod]
        public void ToggleSoundIsLoopedCommandExecute_IsLoopedWasFalse_IsLoopedIsNowTrue()
        {
			ISound sound = StubSoundWithRandomName();
			sound.IsLooped = true;
			Target = CreateTargetWithStubs();

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
            
            Target = CreateTargetWithStubs(dialogService: stub, soundBoardRepository: mock);

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

            Target = CreateTargetWithStubs(dialogService: stub, soundBoardRepository: mock);

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

            Target = CreateTargetWithStubs(dialogService: mock, soundBoardRepository: stub);

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

            Target = CreateTargetWithStubs(dialogService: mock, soundBoardRepository: stub);

            //Act
            Target.Commands.ShutdownAppCommand.Execute(null);

            //Assert
            mock.VerifyAllExpectations();
        }

        private static MainWindowViewModel CreateTargetWithStubs(ISoundBoardRepository soundBoardRepository = null, IDialogService dialogService = null, IKernel container = null)
        {
            if (container == null)
            {
                container = new StandardKernel();
                container.Bind<MainWindow>().ToMethod(context => new MainWindow());
            }

            return new MainWindowViewModel(
                soundBoardRepository ?? MockRepository.GenerateStub<ISoundBoardRepository>(),
                dialogService ?? MockRepository.GenerateStub<IDialogService>(),
                sounds => null,
                container,
				() => MockRepository.GenerateStub<ISound>());
        }

	    private ISound StubSoundWithRandomName()
	    {
		    return StubSoundWithName(Guid.NewGuid().ToString());
	    }

		private ISound StubSoundWithName(string name)
		{
			ISound sound = MockRepository.GenerateStub<ISound>();
			sound.Name = name;
			return sound;
		}
	}
}