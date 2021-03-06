using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using MovementManager.Helper;
using MovementManager.InputProcess;
using MovementManager.Model;
using MovementManager.Service;
using serial_communication_client;

namespace MovementManager
{
    public class ImageWriterActuator : BaseActuator
    {

        const double PX_PER_CM = 37.795280352161;
        double maxX, maxY, minY, minX = 0;
        double cos45 = MathHelper.Cos(45), sin45 = MathHelper.Sin(45);

        private double _verticalLength, _horizontalLength;

        private Servo       _baseMotorComponent, _secondaryMotorComponent;
        private PenMotor    _penMotorComponent;
        private Led         _ledComponent;

        private readonly INotificationService _notificationService;
        public ImageWriterActuator(
            INotificationService notificationService,
            ISetting setting): base(setting)
        {
            _notificationService = notificationService;
            ConfigureBounds();
        }

        public int ImageWidth => (int) (maxX - minX);
        public int ImageHeight => (int) (maxY - minY);

        public string OutputPath { get; set; } = "Output";

        private IService _service;

        public void ForceStop()
        {
            _service?.Close();
        }

        public void Execute(Bitmap image)
        {
            image               = ImageLoader.ResizeImage(image, ImageWidth, ImageHeight);
            int[][] imageCode   = ImageLoader.GetBlackAndWhiteImageCode(image);
            _service            = CreateService();

            _service.Connect();

            InitComponent(_service);

            // Task.Run(() =>
            // {
            if (_setting.SimulationMode == false)
            {
                ResetHardware();
                if (_setting.ResetHardwareMode == true)
                {
                    return;
                }
            }
            // return;
            ExtractMovementPropertiesFromImageCode(imageCode, out ICollection<MovementProperty> movementProperties);

            SaveToFile(movementProperties, image);
            ExecuteDraw(movementProperties);

            ToggleLedFinishOperation();

            if (_setting.SimulationMode == false)
                ResetHardware();

            _service.Close();

            Debug.WriteLine(" =========== End execution ============ ");

            if (_service.Connected)
            {
                _service.Close();
            }

        }
        private void ConfigureBounds()
        {
            _verticalLength     = _setting.ArmBaseLength + _setting.ArmSecondaryLength;
            _horizontalLength   = _setting.ArmBaseLength + _setting.ArmSecondaryLength;

            // maximum value
            maxX = _horizontalLength - _setting.ArmSecondaryLength;
            maxY = (_setting.ArmBaseLength * sin45 + _setting.ArmBaseLength * sin45);

            // minimum value
            minX = _horizontalLength - (_setting.ArmBaseLength * cos45 + _setting.ArmBaseLength * cos45);
            minY = _setting.ArmBaseLength;

        }

        private void InitComponent(IService service)
        {
            _baseMotorComponent         = new Servo(HardwarePin.SERVO_A_PIN, service) { EnableStepMove = true, AngleStep = 10 };
            _secondaryMotorComponent    = new Servo(HardwarePin.SERVO_B_PIN, service) { EnableStepMove = true, AngleStep = 10 };
            _ledComponent               = new Led(HardwarePin.DEFAULT_LED, service);

            _penMotorComponent          = new PenMotor(HardwarePin.SERVO_PEN_PIN, service, _setting.ArmPenDownAngle, _setting.ArmPenUpAngle)
            {
                PenDownWaitDuration     = _setting.ArmPenDownWaitDuration,
                PenUpWaitDuration       = _setting.ArmPenUpWaitDuration
            };
        }

        private void DrawFullArea()
        {

            Debug.WriteLine($"MAX Horizontal Length: { maxX }, MAX Vertical Length: { maxY } ");

            ICollection<MovementProperty> movementProperties = new LinkedList<MovementProperty>();
            for (double y = minY; y < maxY; y++)
            {
                for (double x = minX; x < maxX; x++)
                {
                    try
                    {
                        MovementProperty prop = GetMovementProperty(x, y);
                        AddIfNotExist(movementProperties, prop);

                        Debug.WriteLine($"[x: { x }, y: { y }] alpha: { prop.Alpha }, beta: { prop.Beta }");
                        Debug.WriteLine($"[x: { (byte)prop.X }, y: { (byte)prop.Y }]");
                    }
                    catch (Exception e)
                    {
                        //   Debug.WriteLine($"[Point Error] {x}, {y}, error: {e.Message}");
                    }
                }
                //  break;
            }

            SaveToFile(movementProperties, null);
            ExecuteDraw(movementProperties);

            ToggleLedFinishOperation();

        }
        private void ExtractMovementPropertiesFromImageCode(int[][] _imageCode, out ICollection<MovementProperty> movementProperties)
        {

            Debug.WriteLine($"MAX Horizontal Length: { maxX }, MAX Vertical Length: { maxY } ");

            movementProperties = new LinkedList<MovementProperty>();
            for (int x = 0; x < _imageCode.Length; x++)
            {
                for (int y = 0; y < _imageCode[x].Length; y++)
                {
                    // Debug.WriteLine($"imageCode[x][y] : { imageCode[x][y]  }");
                    if (_imageCode[x][y] != 1) continue;
                    try
                    {
                        MovementProperty prop = GetMovementProperty(x + minX, maxY - (y));
                        AddIfNotExist(movementProperties, prop);

                        Debug.WriteLine($"[x: { x }, y: { y }] alpha: { prop.Alpha }, beta: { prop.Beta }");
                        Debug.WriteLine($"[x: { (byte)prop.X }, y: { (byte)prop.Y }]");
                    }
                    catch (Exception e)
                    {
                        //   Debug.WriteLine($"[Point Error] {x}, {y}, error: {e.Message}");
                    }
                }
                //  break;
            }

        }

        private void AddIfNotExist(ICollection<MovementProperty> movementProperties, MovementProperty prop)
        {
            foreach (MovementProperty propItem in movementProperties)
            {
                if (propItem.AngleEquals(prop))
                {
                    return;
                }
            }
            movementProperties.Add(prop);
        }

        private void SaveToFile(ICollection<MovementProperty> movementProperties, Bitmap image = null)
        {

            try
            {
                string jsonMovements    = JsonHelper.ToJson(movementProperties);
                string jsonSetting      = JsonHelper.ToJson(_setting);
                string content          = $" const calculatedPaths = {jsonMovements};\n " +
                                          $" const appSettings = {jsonSetting};";
                if ( image != null )
                {
                    using (FileStream fileStream = new FileStream( OutputPath + $"/img_{DateTime.Now:HHmmss}.bmp", FileMode.CreateNew))
                    {
                        image.Save(fileStream, ImageFormat.Bmp);
                    }
                }
                File.WriteAllText( OutputPath + $"/path_{DateTime.Now:HHmmss}.json", jsonMovements);
                File.WriteAllText( OutputPath + $"/data.js", content);

                Debug.WriteLine("Success saving to file");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to save paths to file: "+e.Message);
            }
        }

        private void ExecuteDraw(ICollection<MovementProperty> movementProperties)
        {

            int step        = 0;
            int totalStep   = movementProperties.Count;

            _notificationService.Start();

            foreach (MovementProperty prop in movementProperties)
            {
                step++;
                _notificationService.NotifyProgress(step, totalStep, prop);
                if (_setting.SimulationMode == true)
                {
                    Thread.Sleep(50);
                    Debug.WriteLine($"setting.ResetHardwareMode is true. continue : {step}/{totalStep} ");
                    continue;
                }
                Debug.WriteLine($"[STEP] { step }/{ movementProperties.Count }");
                
                MoveArm(prop);
                Thread.Sleep(_setting.DelayBeforeTogglePen);
                TogglePen();
            }

            _notificationService.NotifyComplete();
        }

        private void ToggleLedFinishOperation()
        {
            int delay = 5;
            for (var i = 0; i < 10; i++)
            {
                ToggleLed(true, delay);
                ToggleLed(false, delay / 2);
            }

        }

        bool ValidateX(double x)
        {
            bool result = x >= minX && x <= maxX;
            //   Debug.WriteLine($"X = { x } min: { 0 } max: { maxX } -valid = { result }");
            if (!result)
            {
                throw new ArgumentException($"Invalid X: { x }. Range = {minX} - {maxX}");
            }

            return result;
        }
        bool ValidateY(double y)
        {
            bool result = y >= minY && y <= maxY;
            //    Debug.WriteLine($"Y = { y } min: { B1_LENGTH } max: { verticalLength } -valid = { result }");
            if (!result)
            {
                throw new ArgumentException($"Invalid Y: { y }. Range: {minY} - {maxY}");
            }

            return result;
        }

        ///////////////////////////// Movement Model //////////////////////////////

        MovementProperty GetMovementProperty(double x, double y)
        {
            ValidateX(x);
            ValidateY(y);
            MovementProperty prop = CalculateMovement(x, y);
            if (null == prop)
            {
                throw new ArgumentException($"X,Y valid. but not feasible: {x}, {y}");
            }

            return prop;
        }

        private void MoveArm(MovementProperty prop)
        {
            Debug.WriteLine($"Move motor ({prop.XString}, {prop.YString})");

            // Move arms
            _baseMotorComponent.Move((byte)prop.Alpha);
            // secondaryMotorComponent.Move((byte) prop.Theta);
            int secondaryArmMoveAngle = _setting.ArmSecondaryAngleAdjustment + prop.Beta + prop.Omega;
            _secondaryMotorComponent.Move((byte)secondaryArmMoveAngle);
        }

        private void TogglePen()
        {
            Blink();
            _penMotorComponent.PenDown();
            Blink(_setting.LedBlinkCount);
            _penMotorComponent.PenUp();
            Blink();
        }

        private void Blink(int count = 0)
        {
            if (count < 0) count = 0;
            for (var i = 0; i <= count; i++)
            {
                ToggleLed(true);
                ToggleLed(false);
            }
        }

        private void ToggleLed(bool on, int waitTime = 0) => _ledComponent.Toggle(on, waitTime);

        void ResetHardware()
        {
            Debug.WriteLine(" ======= Start Reset Hardware ======= ");
            ToggleLed(true, 1000);

            // Reset ARM
            _baseMotorComponent.Move(0, 1000);
            _secondaryMotorComponent.Move(_setting.ArmSecondaryAngleAdjustment, 1000);
            // Reset PEN 
            _penMotorComponent.PenUp();

            ToggleLed(false, 1000);
            Debug.WriteLine(" ======= End Reset Hardware ======= ");
        }

        MovementProperty CalculateMovement(double x, double y)
        {

            for (double alpha = 0; alpha < 90; alpha++)
            {
                for (double beta = 0; beta < 45; beta++)
                {
                    double calX = CalculateHorizontalPositionFromGivenAngle(alpha, beta);
                    if (MathHelper.InRange(calX, x, _setting.Tolerance))
                    {
                        double calY = CalculateVerticalPositionFromGivenAngle(alpha, beta);
                        if (MathHelper.InRange(calY, y, _setting.Tolerance))
                        {
                            //     Debug.WriteLine($"<!> [{ x }, { y }] Trial => x: { calX }, y: { calY }");
                            double theta = CalculateTetha(alpha, beta);
                            double omega = CalculateOmega(alpha);
                            return new MovementProperty(calX, calY, alpha, beta, theta, omega);
                        }
                    }
                }
            }
            // Debug.WriteLine($" not found : {x}, {y}");
            return null;
        }

        double CalculateHorizontalPositionFromGivenAngle(double alpha, double beta)
        {
            double baseArmLengthHorizontal      = _setting.ArmBaseLength * MathHelper.Cos(alpha);
            double secondaryArmLengthHorizontal = _setting.ArmSecondaryLength * MathHelper.Cos(beta);
            double totalArmLengthHorizontal     = baseArmLengthHorizontal + secondaryArmLengthHorizontal;

            return _horizontalLength - totalArmLengthHorizontal;
        }
        private double CalculateVerticalPositionFromGivenAngle(double alpha, double beta)
        {
            double baseArmLengthVertical        = _setting.ArmBaseLength * MathHelper.Sin(alpha);
            double secondaryArmLengthVertical   = _setting.ArmSecondaryLength * MathHelper.Sin(beta);
            double totalArmLengthVertical       = baseArmLengthVertical + secondaryArmLengthVertical;

            return totalArmLengthVertical;
        }

        private byte CalculateTetha(double alpha, double beta)
        {
            double lambda = MathHelper.CosAngle(MathHelper.Sin(alpha));
            return (byte)(lambda + beta);//+ 90;
        }
        // relative angle from base arm latest position against x axis
        private byte CalculateOmega(double alpha)
        {
            double baseArmPependicular      = _setting.ArmBaseLength * MathHelper.Tan(alpha);
            double baseArmLengthVertical    = _setting.ArmBaseLength * MathHelper.Sin(alpha);
            return (byte)MathHelper.SinAngle(baseArmLengthVertical / baseArmPependicular);
        }



    }


}
