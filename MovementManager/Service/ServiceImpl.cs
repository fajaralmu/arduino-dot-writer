using System;
using System.Threading;
using arduino_client_hand_writer.Serial;
using serial_communication_client;
using serial_communication_client.Commands;

namespace  MovementManager.Service
{
    public class ServiceImpl : IService
    {
        private readonly IClient _client;

        public ServiceImpl(IClient client)
        {
            _client = client;
        }

        public void Close()
        {
            _client.Close();
        }

        public void Connect()
        {
            _client.Connect();
        }

        public void MoveServo(HardwarePin pin, byte angle, int waitDuration = 0)
        { 
            CommandPayload command = CommandMotorPayload.NewCommand( pin, angle );
            _client.Send( command, 0 );
            
            Thread.Sleep( waitDuration );
        }

        public bool Connected => _client.Connected;

        public int ReadMotorAngle(HardwarePin pin)
        {
            string response = _client.Send( new CommanReadMotorPayload( pin ));

            return int.Parse( response ) - CommandMotorPayload.ANGLE_ADJUSTMENT;;
        }

        public void ToggleLed(HardwarePin pin, bool turnOn = true, int waitDuration = 0)
        {
            CommandPayload cmd = new CommandLedPayload( turnOn ? CommandName.LED_ON : CommandName.LED_OFF, pin );
            _client.Send( cmd, waitDuration );
        }

        public void MoveMotor(HardwarePin pin, byte in1, byte in2, byte speed, bool forward, int waitDuration = 0)
        {
            CommandPayload cmd = new CommandDcMotorPayload(pin, in1, in2, speed, forward);
            _client.Send( cmd, waitDuration );
        }
        public void StopMotor(HardwarePin pin, int waitDuration = 0)
        {
            CommandPayload cmd = new CommandStopMotorPayload(pin);
            _client.Send( cmd, waitDuration );
        }
    }
}