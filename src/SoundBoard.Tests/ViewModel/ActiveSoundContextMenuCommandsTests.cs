using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SoundBoard.Model;
using SoundBoard.ViewModel;

namespace SoundBoard.Tests.ViewModel
{
    [TestClass]
    public class ActiveSoundContextMenuCommandsTests
    {
        public ActiveSoundContextMenuCommands Target { get; set; }

        [TestInitialize]
        public void InitializeBeforeEachTest()
        {
	        Target = CreateTarget(MockRepository.GenerateStub<IMainWindowViewModel>());
        }

        [TestMethod]
        public void Constructor_MainWindowViewModelIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () => new ActiveSoundContextMenuCommands(null);

            constructor.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("mainWindowViewModel");
        }

        [TestMethod]
        public void Commands__ContainsItemForRemoveSoundsCommand()
        {
			Target.Commands.First().Command.ShouldBeEquivalentTo(Target.RemoveSoundsCommand);
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
        public void RemoveSoundsCommandCanExecute_ParameterIsIListWith3Sounds_ReturnsTrue()
        {
	        Target = CreateTarget();

	        Target.RemoveSoundsCommand.CanExecute(new Collection<ISound>
	        {
		        MockRepository.GenerateStub<ISound>(),
		        MockRepository.GenerateStub<ISound>(),
		        MockRepository.GenerateStub<ISound>()
	        }).Should().BeTrue();
        }

        [TestMethod]
        public void RemoveSoundsCommandCanExecute_ParameterIsIListWith1Sound_ReturnsTrue()
        {
	        Target = CreateTarget();

	        Target.RemoveSoundsCommand.CanExecute(new Collection<ISound> {MockRepository.GenerateStub<ISound>()})
		        .Should().BeTrue();
        }

        [TestMethod]
        public void RemoveSoundsCommandExecute_ParameterIsNull_ThrowsArgumentNullException()
        {
            Action removeSoundsCommandExecute = () => Target.RemoveSoundsCommand.Execute(null);

            removeSoundsCommandExecute.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("parameter");
        }

        [TestMethod]
        public void RemoveSoundsCommandExecute_ParameterIsNotOfTypeIList_ThrowsArgumentException()
        {
            Action removeSoundsCommandExecute = () => Target.RemoveSoundsCommand.Execute("Not an IList");

            removeSoundsCommandExecute.ShouldThrow<ArgumentException>()
                .WithMessage("Parameter must be of type " + typeof(IList) + "*")
                .And.ParamName.ShouldBeEquivalentTo("parameter");
        }
        
        [TestMethod]
        public void RemoveSoundsCommandExecute_SoundBoardIsNotNullAndParameterIsIListWith3Sounds_RemovesOnlyThoseSoundsFromSelectedSoundboard()
        {
			//Arrange
	        ObservableCollection<ISound> selectedSounds = new ObservableCollection<ISound>
	        {
		        MockRepository.GenerateStub<ISound>(),
		        MockRepository.GenerateStub<ISound>(),
		        MockRepository.GenerateStub<ISound>(),
	        };
	        ObservableCollection<ISound> activeSounds = new ObservableCollection<ISound>(selectedSounds)
	        {
		        MockRepository.GenerateStub<ISound>(),
		        MockRepository.GenerateStub<ISound>()
	        };

	        IMainWindowViewModel mainWindowViewModel = CommonStubsFactory.StubMainWindowViewModel(initialSounds: activeSounds);

			Target = CreateTarget(mainWindowViewModel);

            //Act
            Target.RemoveSoundsCommand.Execute(selectedSounds);

            //Assert
            mainWindowViewModel.SoundService.ActiveSounds
                .Should().NotContain(selectedSounds)
                .And.HaveCount(2);
        }

	    private static ActiveSoundContextMenuCommands CreateTarget(IMainWindowViewModel mainWindowViewModel = null)
	    {
		    IMainWindowViewModel theMainWindowViewModel = mainWindowViewModel ?? CommonStubsFactory.StubMainWindowViewModel();

		    return new ActiveSoundContextMenuCommands(theMainWindowViewModel);
	    }
    }
}