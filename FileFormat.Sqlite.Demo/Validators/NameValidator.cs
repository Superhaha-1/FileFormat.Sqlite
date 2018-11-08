using FluentValidation;
using System.Linq;

namespace FileFormat.Sqlite.Demo.Validators
{
    public sealed class NameValidator : AbstractValidator<string>
    {
        public static NameValidator Instance { get; } = new NameValidator();

        private NameValidator()
        {
            RuleFor(name => name).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().Matches("^[A-z0-9\u4e00-\u9fa5_-]{1,30}$").WithName("名称");
        }
    }
}
