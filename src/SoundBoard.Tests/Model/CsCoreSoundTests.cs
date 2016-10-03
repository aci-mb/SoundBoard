﻿using System;
using System.ComponentModel;
using AcillatemSoundBoard.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcillatemSoundBoard.Tests.Model
{
    [TestClass]
    public class CsCoreSoundTests
    {
        protected CsCoreSound Target { get; set; }

        [TestMethod]
        public void Class__ImplementsINotifyPropertyChanged()
        {
            new CsCoreSound().Should().BeAssignableTo<INotifyPropertyChanged>();
        }

        [TestMethod]
        public void Clone__ReturnsEqualCloneOfSelf()
        {
            Target = new CsCoreSound
			{
                FileName = @"C:\SomeSound.mp3",
                IsLooped = true,
                Name = "Some Name",
                VolumeInPercent = 42
            };

            var clone = Target.Clone() as CsCoreSound;

            clone.ShouldBeEquivalentTo(Target);
        }
    }
}