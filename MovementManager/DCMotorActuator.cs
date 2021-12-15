using System;
using System.Threading.Tasks;
using MovementManager.Model;
using MovementManager.Service;
using serial_communication_client;

namespace MovementManager
{
    public class DCMotorActuator : BaseActuator
    {
        // DC Motor A
        private DCMotor motorWheel;
        private DCMotor motorSteer;
        const int input1Wheel = 8, input2Wheel = 7;
        const int input1Steer = 4, input2Steer = 5;

        private readonly IService _service;

        public DCMotorActuator(ISetting setting) : base(setting)
        {
            _service = CreateService(true);
            motorWheel = new DCMotor(HardwarePin.DC_MOTOR_A, _service, input1Wheel, input2Wheel);
            motorSteer = new DCMotor(HardwarePin.DC_MOTOR_B, _service, input1Steer, input2Steer);
        }
        public void Turn(byte speed, bool forward, int waitDuration = 0)
        {
            motorSteer?.Move((byte)speed, forward, waitDuration);
            Task.Delay(1000).Wait();
            motorSteer?.Stop();
        }

        public void MoveMotor(int speed, bool forward, int waitDuration = 0)
        {
            motorWheel?.Move((byte)speed, forward, waitDuration);
        }

        public void StopMotor(int waitDuration = 0)
        {
            motorWheel?.Stop();
        }

        internal void Disconnect()
        {
            _service.Close();
        }
    }
}