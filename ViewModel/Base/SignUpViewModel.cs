using DrawchadDomain.Constants;
using DrawchadDomain.Resources;
using FluentValidation;

namespace DrawchadViewModel.Base
{
    public class SignUpViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SignUpViewModelRule : AbstractValidator<SignUpViewModel>
    {
        private static readonly int minLengthName = int.Parse(DrawchadSettings.Settings["MIN_LENGTH_NAME"].ToString());
        private static readonly int maxLengthName = int.Parse(DrawchadSettings.Settings["MAX_LENGTH_NAME"].ToString());
        private static readonly int minLengthPassword = int.Parse(DrawchadSettings.Settings["MIN_LENGTH_PASSWORD"].ToString());
        private static readonly int maxLengthPassword = int.Parse(DrawchadSettings.Settings["MAX_LENGTH_PASSWORD"].ToString());
        private readonly string emailRegex = @"^(?!\s)[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        private readonly string nameLengthError = string.Format(Drawchad.DRAWCHAD_SIGNUP_NAME_LENGTH_ERROR, minLengthName, maxLengthName);
        private readonly string passwordLengthError = string.Format(Drawchad.DRAWCHAD_PASSWORD_LENGTH_ERROR, minLengthPassword, maxLengthPassword);

        public SignUpViewModelRule()
        {
            RuleFor(s => s.Name)
                .NotNull()
                .Length(minLengthName, maxLengthName)
                .WithMessage(nameLengthError);

            RuleFor(s => s.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage(Drawchad.DRAWCHAD_EMAIL_EMPTY_ERROR)
                .Matches(emailRegex)
                .WithMessage(Drawchad.DRAWCHAD_EMAIL_INVALID_FORMAT_ERROR);

            RuleFor(s => s.Password)
                .NotNull()
                .Length(minLengthPassword, maxLengthPassword)
                .WithMessage(passwordLengthError);
        }
    }
}
