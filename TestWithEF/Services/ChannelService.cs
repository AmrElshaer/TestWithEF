namespace TestWithEF.Services
{
    public class ChannelService:IChannelService
    {
       public int GetPosition()
        {
            Task.Delay(2000).Wait();
            return 0;
        }
    }
}
