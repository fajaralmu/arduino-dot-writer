namespace  MovementManager
{
    public interface ISetting
    {
      

        public double ArmBaseLength { get; }
        public double ArmSecondaryLength { get; }
        public byte ArmSecondaryAngleAdjustment { get; }
        public byte ArmPenDownAngle { get; }
        public int ArmPenDownWaitDuration { get; }
        public byte ArmPenUpAngle { get; }
        public int ArmPenUpWaitDuration { get; }
        public double Tolerance { get; }
        public int DelayBeforeTogglePen { get; }
        public string PortName { get; }
        public int BaudRate { get;  }
        public bool SimulationMode{ get; set; }
        public bool ResetHardwareMode{ get; }
        public int LedBlinkCount{ get; }
    }
    
}