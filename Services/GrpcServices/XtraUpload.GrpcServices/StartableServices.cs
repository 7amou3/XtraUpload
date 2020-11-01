using System.Threading.Tasks;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    public class StartableServices
    {
        private readonly ICheckClientCommand _checkClientCommand;
        private readonly IStorageHealthClientCommand _storageHealthCommand;

        public StartableServices(ICheckClientProxy checkProxy, IStorageHealthClientProxy healthProxy)
        {
            _checkClientCommand = checkProxy as ICheckClientCommand;
            _storageHealthCommand = healthProxy as IStorageHealthClientCommand;
        }

        public void Start()
        {
            // Free the caller and Queu this task to run on background thread
            Task.Run(() =>
            {
                _checkClientCommand.Initialize();
                _storageHealthCommand.Initialize();
            });
        }
    }
}
