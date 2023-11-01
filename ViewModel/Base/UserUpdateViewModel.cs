using Domain.Constants;
using Domain.Resources;
using FluentValidation;

namespace DrawchadViewModel.Base
{
    public class UserUpdateViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
    }

    public class UserUpdateRule : AbstractValidator<UserUpdateViewModel>
    {
        private static readonly int minLengthName = int.Parse(DrawchadSettings.Settings["MIN_LENGTH_NAME"].ToString());
        private static readonly int maxLengthName = int.Parse(DrawchadSettings.Settings["MAX_LENGTH_NAME"].ToString());
        private readonly string emailRegex = @"^(?!\s)[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private readonly string phoneRegex = @"^(\\+84|0)?\\d{9,10}$";

        private readonly string nameLengthError = string.Format(Drawchad.DRAWCHAD_SIGNUP_NAME_LENGTH_ERROR, minLengthName, maxLengthName);

        public UserUpdateRule()
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

            RuleFor(s => s.Phone)
                .NotNull()
                .Matches(phoneRegex)
                .WithMessage(Drawchad.DRAWCHAD_PHONE_ERROR)
                .When(s => s.Phone is not null);
        }
    }
}
