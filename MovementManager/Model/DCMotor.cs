using MovementManager.Service;
using serial_communication_client;

namespace MovementManager.Model
{
    public class DCMotor:Component
    {
        public byte Input2 {get;}
        public byte Input1 {get;}
        public DCMotor(
            HardwarePin pin, 
            IService service,
            byte input1,
            byte input2 ) : base(pin, service)
        {
            Input1 = input1;
            Input2 = input2;
        }

        public void Move(byte speed, int waitDuration = 0)
        {
            _service.MoveMotor(Pin, Input1, Input2, speed, waitDuration);
        }

        public void Stop(int waitDuration = 0)
        {
            _service.StopMotor(Pin, waitDuration);
        }

    }
}