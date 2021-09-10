
namespace ThirdPartyService;
public class StartUpTimer
{
    private readonly DateTime? _startTime;
    public StartUpTimer()
    {
        _startTime = DateTime.UtcNow;
    }
    public bool IsReady { get { return DateTime.UtcNow.Subtract(_startTime.Value).TotalSeconds > 150; } }
}
