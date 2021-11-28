using Microsoft.Extensions.Options;
using MovementManager;
using MovementManager.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace DotWriterServer.Services
{
    public class ActuatorService : IActuatorService
    {
        private readonly INotificationService _notificationService;
        private readonly ImageWriterActuator _imageWriterActuator;
        public ActuatorService(
            INotificationService notificationService,
            IOptions<Setting> setting
        ) {
            _notificationService = notificationService;
            _imageWriterActuator = new ImageWriterActuator(_notificationService, setting.Value)
            {
                OutputPath = "../MovementManager/Output"
            };
        }
        public void ExecuteImageWriter(Bitmap bitmap)
        {
            Debug.WriteLine("ExecuteImageWriter::Execute");
            _imageWriterActuator.Execute(bitmap);
        }
    }
}
