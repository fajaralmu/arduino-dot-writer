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
        const int input1Pin = 8, input2Pin = 7;

        public void MoveMotor(int speed, int waitDuration = 0)
        {
            IService service = CreateService(true);
            motorA = new DCMotor(serial_communication_client.HardwarePin.DC_MOTOR_A, service, input1Pin, input2Pin);
            motorA.Move((byte)speed, waitDuration);
            service.Close();
        }

        public void StopMotor(int waitDuration = 0)
        {
            IService service = CreateService(true);
            motorA = new DCMotor(serial_communication_client.HardwarePin.DC_MOTOR_A, service, input1Pin, input2Pin);
            motorA.Stop();
            service.Close();
        }
    }
}