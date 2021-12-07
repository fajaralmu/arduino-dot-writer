namespace serial_communication_client.Commands
{
    public class CommandStopMotorPayload : CommandPayload
    {
        public CommandStopMotorPayload(HardwarePin envPin) : 
        base(CommandName.STOP_MOTOR, envPin )
        {
        }


    }
}