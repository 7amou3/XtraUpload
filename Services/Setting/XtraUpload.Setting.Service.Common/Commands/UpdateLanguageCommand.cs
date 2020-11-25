using MediatR;
using System.ComponentModel.DataAnnotations;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    /// <summary>
    /// Update the language of the current user
    /// </summary>
    public class UpdateLanguageCommand : IRequest<OperationResult>
    {
        public UpdateLanguageCommand(string culture)
        {
            Culture = culture;
        }

        [MaxLength(6)]
        public string Culture { get; }
    }
}
