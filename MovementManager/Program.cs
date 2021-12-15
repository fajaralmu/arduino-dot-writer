using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using MovementManager.InputProcess;
using MovementManager.Service;

namespace MovementManager
{
    public class Program
    {
        
        const string settingFile = "Resources/settings.json";
        public static void Main(string[] args)
        {
            ConfigureLogger();

            ISetting setting                            = Setting.FromFile(settingFile);
            // setting.SimulationMode                      = true;
            INotificationService notificationService    = new NotificationService( "movementnotif", 2000000 );
            ImageWriterActuator writer                  = new ImageWriterActuator(notificationService, setting);
            Bitmap image                                = ImageLoader.LoadImageBitmap("Input/SampleFont.bmp");

            DCMotorActuator motorActuator = new DCMotorActuator(setting);
            // Test steering
            Task.Run(()=>{
                for (var i = 0; i < 100; i++)
                {
                    motorActuator.Turn(250, i % 2 == 0);
                }
            });

            // Test wheel
            for (var i = 0; i < 100; i++)
            {
                motorActuator.MoveMotor(250, i % 2 == 0, 0);
                Task.Delay(20000).Wait();
                motorActuator.StopMotor();
                Task.Delay(3000).Wait();
               
            }
            motorActuator.Disconnect();
            // writer.Execute(image);

        }

        private static void ConfigureLogger()
        {
            //File
            // _logFile = File.Create($"Logs/RoverSimulation-Log-{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")}.log");
            // TextWriterTraceListener myTextListener = new TextWriterTraceListener(_logFile);
            // Trace.Listeners.Add(myTextListener);

            //Debug
            TextWriterTraceListener writer = new TextWriterTraceListener(System.Console.Out);
            Trace.Listeners.Add(writer);
        }
    }
}