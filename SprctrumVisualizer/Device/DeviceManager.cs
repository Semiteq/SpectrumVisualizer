using Device.ATR.Devices;

internal class DeviceManager
{
    private readonly DeviceService _deviceService;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DeviceManager(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    public async Task<bool> ConnectAsync(string portName)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _deviceService.Open(portName);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Disconnect()
    {
        _deviceService.Close();
    }
}
