using MovementManager.Model;
using MovementManager.Service;

namespace MovementManager
{
    public class DCMotorActuator : BaseActuator
    {
        public DCMotorActuator(ISetting setting) : base(setting)
        {
        }

        // DC Motor A
        private DCMotor motorA;
        private DCMotor motorB;
        const int input1Pin = 8, input2Pin = 7;
        const int input1PinB = 4, input2PinB = 5;

        public void MoveMotor(int speed, int waitDuration = 0)
        {
            IService service = CreateService(true);
            motorA = new DCMotor(serial_communication_client.HardwarePin.DC_MOTOR_A, service, input1Pin, input2Pin);
            motorA.Move((byte)speed, waitDuration);
            motorB = new DCMotor(serial_communication_client.HardwarePin.DC_MOTOR_B, service, input1PinB, input2PinB);
            motorB.Move((byte)speed, waitDuration);
            service.Close();
        }

        public void StopMotor(int waitDuration = 0)
        {
            IService service = CreateService(true);
            motorA = new DCMotor(serial_communication_client.HardwarePin.DC_MOTOR_A, service, input1Pin, input2Pin);
            motorB = new DCMotor(serial_communication_client.HardwarePin.DC_MOTOR_B, service, input1PinB, input2PinB);
            motorA.Stop();
            motorB.Stop();
            service.Close();
        }
    }
}