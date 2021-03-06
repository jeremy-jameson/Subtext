using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtext.Framework.Util;

namespace UnitTests.Subtext.Framework.Util
{
    [TestClass]
    public class MathHelperTests
    {
        [TestMethod]
        public void ScaleToFit_WithImageAlreadyFitting_ReturnsOriginalImage()
        {
            // arrange
            var original = new Size(8, 4); // aspect = 2.00

            // act
            Size resized = original.ScaleToFit(new Size(12, 15));

            // assert
            Assert.AreEqual(new Size(8, 4), resized);
        }

        [TestMethod]
        public void ScaleToFit_ScaledToSameAspectRatio_ScalesExactlyToMaxSize()
        {
            // arrange
            var original = new Size(8, 4); // aspect = 2.00

            // act
            Size resized = original.ScaleToFit(new Size(4, 2));

            // assert
            Assert.AreEqual(new Size(4, 2), resized);
        }

        [TestMethod]
        public void ScaleToFit_WithImageHavingAspectRatioGreaterThanOneScaledToAspectRatioLessThanOne_ScalesCorrectly()
        {
            // arrange
            var original = new Size(7, 5); // aspect = 1.40
            var maxSize = new Size(2, 3); // aspect = 0.67

            // act
            Size resized = original.ScaleToFit(maxSize);

            // assert
            Assert.AreEqual(new Size(2, 1), resized); // aspect = 2
        }

        [TestMethod]
        public void ScaleToFit_WithRealisticImageSizes_ScalesProperly()
        {
            // arrange
            var original = new Size(1352, 879);

            // act
            Size resized = original.ScaleToFit(640, 480);

            // assert
            Assert.AreEqual(new Size(640, 416), resized); // aspect = 2
        }

        [TestMethod]
        public void ScaleToFit_WithImageHavingAspectRatioLessThanOneScaledToAspectRatioGreaterThanOne_ScalesCorrectly()
        {
            // arrange
            var original = new Size(5, 7); // aspect = 0.71
            var maxSize = new Size(4, 3); // aspect = 1.33

            // act
            Size resized = original.ScaleToFit(maxSize);

            // assert
            Assert.AreEqual(new Size(2, 3), resized); // aspect = 0.67
        }

        [TestMethod]
        public void GetAspectRatio_WithLargeRatio_ReturnsCorrectValue() { 
            // arrange
            int width = 111;
            int height = 31;

            // act
            var result = MathHelper.GetAspectRatio(width, height);

            // assert
            Assert.AreEqual(
                3.5806451612903225806451612902,
                (double)result,
                0.0000000000000000000000000001);
        }
    }
}