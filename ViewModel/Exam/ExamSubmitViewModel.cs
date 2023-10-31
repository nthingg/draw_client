using DrawchadDomain.Resources;
using FluentValidation;

namespace DrawchadViewModel.Exam
{
    public class ExamSubmitViewModel
    {
        public string AnswerUrl { get; set; }
    }

    public class ExamSubmitRule : AbstractValidator<ExamSubmitViewModel>
    {
        public ExamSubmitRule()
        {
            RuleFor(e => e.AnswerUrl)
                .NotEmpty()
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .WithMessage(Drawchad.DRAWCHAD_IMAGE_URL_FORMAT_ERROR);
        }
    }
}
