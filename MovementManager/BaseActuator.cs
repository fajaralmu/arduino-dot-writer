using arduino_client_hand_writer.Serial;
using MovementManager.Service;
using serial_communication_client.Serial;

namespace MovementManager
{
    public abstract class BaseActuator
    {
        protected ISetting _setting;

        public BaseActuator(ISetting setting)
        {
            _setting = setting;
        }
        protected IService CreateService(bool connect = false)
        {
            IClient client = _setting.SimulationMode ? new MockClient() : SerialClient.Create(_setting.PortName, _setting.BaudRate, false);
            IService service = new ServiceImpl(client);
            if (connect)
            {
                service.Connect();
            }
            return service;
        }

    }
}