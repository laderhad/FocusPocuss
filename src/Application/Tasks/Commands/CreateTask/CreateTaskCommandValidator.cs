namespace FocusPocuss.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(v => v.OriginalInput)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(input => !string.IsNullOrEmpty(input.Trim()))
            .WithMessage("Original input must not be empty.")
            .MaximumLength(1000);
    }
}
