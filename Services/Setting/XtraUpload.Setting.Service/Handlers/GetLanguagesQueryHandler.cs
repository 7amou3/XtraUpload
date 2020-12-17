using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    public class GetLanguagesQueryHandler : IRequestHandler<GetLanguagesQuery, LanguagesResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetLanguagesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<LanguagesResult> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
        {
            var languages = await _unitOfWork.Languages.GetAll();
            return new LanguagesResult()
            {
                Languages = languages.OrderBy(s => s.Name)
            };
        }
    }
}
