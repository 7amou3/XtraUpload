using System.Threading.Tasks;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    public class StartableServices
    {
        private readonly ICheckClientCommand _checkClientCommand;
        public StartableServices(ICheckClientProxy checkProxy)
        {
            _checkClientCommand = checkProxy as ICheckClientCommand;
        }

        public void Start()
        {
            // Free the caller and Queu this task to run on background thread
            Task.Run(() =>
            {
                _checkClientCommand.Initialize();
            });
        }
    }
}
