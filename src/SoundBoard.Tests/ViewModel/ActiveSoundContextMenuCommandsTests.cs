using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using AcillatemSoundBoard.Model;
using AcillatemSoundBoard.ViewModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace AcillatemSoundBoard.Tests.ViewModel
{
    [TestClass]
    public class ActiveSoundContextMenuCommandsTests
    {
        public ActiveSoundContextMenuCommands Target { get; set; }

        [TestInitialize]
        public void InitializeBeforeEachTest()
        {
            Target = new ActiveSoundContextMenuCommands(MockRepository.GenerateStub<IMainWindowViewModel>());
        }

        [TestMethod]
        public void Constructor_MainWindowViewModelIsNull_ThrowsArgumentNullException()
        {
            var constructor = new Action(() => new ActiveSoundContextMenuCommands(null));

            constructor.ShouldThrow<ArgumentNullException>().And.ParamName.ShouldBeEquivalentTo("mainWindowViewModel");
        }

        [TestMethod]
        public void Commands__ContainsItemForRemoveSoundsCommand()
        {
            var menuCommand = Target.Commands.First(command => ReferenceEquals(command.Command, Target.RemoveSoundsCommand));
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
            var stub = MockRepository.GenerateStub<IMainWindowViewModel>();

            Target = new ActiveSoundContextMenuCommands(stub);

	        Target.RemoveSoundsCommand.CanExecute(new Collection<ISound>
	        {
		        MockRepository.GenerateStub<ISound>(),
		        MockRepository.GenerateStub<ISound>(),
		        MockRepository.GenerateStub<ISound>()
	        })
		        .Should().BeTrue();
        }

        [TestMethod]
        public void RemoveSoundsCommandCanExecute_ParameterIsIListWith1Sound_ReturnsTrue()
        {
            var stub = MockRepository.GenerateStub<IMainWindowViewModel>();

            Target = new ActiveSoundContextMenuCommands(stub);

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
                .WithMessage("Parameter must be of type " + typeof(IList) + "*")
                .And.ParamName.ShouldBeEquivalentTo("parameter");
        }
        
        [TestMethod]
        public void RemoveSoundsCommandExecute_SoundBoardIsNotNullAndParameterIsIListWith3Sounds_RemovesThoseSoundsFromSelectedSoundboard()
        {
            //Arrange
            var selectedSounds = new List<ISound>
            {
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>(),
            };
            
            var stub = MockRepository.GenerateStub<IMainWindowViewModel>();
            stub.ActiveSounds = new ObservableCollection<ISound>(selectedSounds)
            {
				MockRepository.GenerateStub<ISound>(),
				MockRepository.GenerateStub<ISound>()
			};

            Target = new ActiveSoundContextMenuCommands(stub);

            //Act
            Target.RemoveSoundsCommand.Execute(selectedSounds);

            //Assert
            stub.ActiveSounds
                .Should().NotContain(selectedSounds)
                .And.HaveCount(2);
        }
    }
}