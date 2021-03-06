﻿using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Subtext.Web.Controllers;
using Subtext.Web.Infrastructure;

namespace UnitTests.Subtext.SubtextWeb.Infrastructure
{
    [TestClass]
    public class SubtextControllerActivatorTests
    {
        [TestMethod]
        public void Create_WithControllerType_CanCreateControllerOfThatType()
        {
            // arrange
            var serviceLocator = new Mock<IDependencyResolver>();
            serviceLocator.Setup(l => l.GetService(typeof(CommentApiController))).Returns(new CommentApiController(null, null));
            var factory = new SubtextControllerActivator(serviceLocator.Object);

            // act
            var controller = factory.Create(null, typeof(CommentApiController));

            // assert
            Assert.AreEqual(typeof(CommentApiController), controller.GetType());
        }

        [TestMethod]
        public void Create_SetsEmptyTempDataProvider()
        {
            // arrange
            var serviceLocator = new Mock<IDependencyResolver>();
            serviceLocator.Setup(l => l.GetService(typeof(CommentApiController))).Returns(new CommentApiController(null, null));
            var factory = new SubtextControllerActivator(serviceLocator.Object);

            // act
            var controller = factory.Create(null, typeof(CommentApiController)) as Controller;

            // assert
            Assert.AreEqual(typeof(EmptyTempDataProvider), controller.TempDataProvider.GetType());
        }

    }
}
