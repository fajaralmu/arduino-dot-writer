using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace DotWriterServer.Services
{
    public interface IActuatorService
    {
        void ExecuteImageWriter(Bitmap bitmap);
        void Disconnect();
    }
}
