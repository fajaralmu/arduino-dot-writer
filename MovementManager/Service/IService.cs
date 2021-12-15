using serial_communication_client;

namespace MovementManager.Service
{
    public interface IService
    {
        bool Connected { get; }

        void MoveServo( HardwarePin pin, byte angle, int waitDuration = 0 );
        int ReadMotorAngle( HardwarePin pin );
        void ToggleLed( HardwarePin pin, bool turnOn = true, int waitDuration = 0 );

        void MoveMotor( HardwarePin pin, byte in1, byte in2, byte speed, bool forward, int waitDuration = 0 );
        void StopMotor( HardwarePin pin, int waitDuration = 0 );

        void Connect();
        void Close();

    }
    
}