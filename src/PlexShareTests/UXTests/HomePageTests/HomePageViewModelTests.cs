//using static PlexShareTests.UX.HomePageTests.HomePageUtils;
using Xunit;
using PlexShareApp;

namespace PlexShareTests.UXTests.HomePageTests
{
    public class HomePageViewModelUnitTest
    {
        //[SetUp]
        private HomePageViewModel _viewModel;

        public HomePageViewModelUnitTest()
        {
            _viewModel = new HomePageViewModel();
        }


        [Fact]
        public void OnUserLogin_ReturnBool()
        {
            //Assert
            var result = _viewModel.VerifyCredentials("", "-1", "123", "", "");
            Assert.Equal("False",result[0]);
            //Assert.Equal(result[1], )
            //Assert.Equal(_viewModel.SendForAuth("192 168.1 .1", 123, "Jasir"), false);
            //Assert.AreEqual(_viewModel.SendForAuth("192.168.1.1", 123, ""), false);
            //Assert.AreEqual(_viewModel.SendForAuth(" ", 123, ""), false);
            //Assert.AreEqual(_viewModel.SendForAuth("", 123, "Jasir"), false);
        }
    }
}

