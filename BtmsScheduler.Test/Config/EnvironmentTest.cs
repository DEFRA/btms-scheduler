using Microsoft.AspNetCore.Builder;

namespace BtmsScheduler.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   {
      var _builder = WebApplication.CreateBuilder();

      var isDev = BtmsScheduler.Config.Environment.IsDevMode(_builder);

      Assert.False(isDev);
   }
}
