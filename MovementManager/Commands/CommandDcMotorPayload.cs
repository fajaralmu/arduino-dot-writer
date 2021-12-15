namespace serial_communication_client.Commands
{
    public class CommandDcMotorPayload : CommandPayload
    {
        public CommandDcMotorPayload(
            HardwarePin envPin, 
            byte in1, 
            byte in2, 
            byte speed, 
            bool forward) : 
        base(
            CommandName.MOVE_MOTOR, 
            envPin, 
            in1, 
            in2, 
            speed, 
            (byte)( forward? 1 : 0 ) )
        {
            
        }


    }
}