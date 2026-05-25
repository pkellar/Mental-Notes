using Microsoft.AspNetCore.Components;
using Resend;
using Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Pages;

public partial class ContactBase : ComponentBase
{
    [Inject] 
    IResend Resend { get; set; } = default!;

    [Inject]
    RateLimiter RateLimiter { get; set; } = default!;

    [Inject]
    IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        
        await base.OnInitializedAsync();
    }

    protected ContactFormModel formModel = new();
    protected bool isSubmitting = false;
    protected bool isCooldown = false;
    protected string? submitResult;

    // Cooldown in seconds
    private const int CooldownSeconds = 20;
    private const int MaxAttempts = 5;
    private static readonly TimeSpan Window = TimeSpan.FromHours(24);

    protected async Task HandleValidSubmit()
    {
        if (isSubmitting || isCooldown)
        {
            submitResult = $"Please wait before submitting again.";
            return;
        }

        var ip = GetUserIp();
        if (RateLimiter.IsLimited(ip, MaxAttempts, Window))
        {
            submitResult = $"Please wait before submitting again.";
            return;
        }

        submitResult = null;

        // Basic malicious content check
        if (ContainsMaliciousContent(formModel))
        {
            submitResult = "Your message cannot be sent.";
            return;
        }

        if (!EmailContainsContent(formModel))
        {
            submitResult = "Please fill in all required fields.";
            return;
        }

        isSubmitting = true;
        bool sent = await SendEmailAsync(formModel);

        isSubmitting = false;

        if (sent)
        {
            submitResult = "Thank you for contacting us! We'll get back to you soon.";
            formModel = new ContactFormModel();
            isCooldown = true;
            // Client-side cooldown
            _ = Task.Run(async () =>
            {
                await Task.Delay(CooldownSeconds * 1000);
                isCooldown = false;
                await InvokeAsync(StateHasChanged);
            });
        }
        else
        {
            submitResult = "There was an error sending your message. Please try again later.";
        }
    }

    private string GetUserIp()
    {
        return HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static bool ContainsMaliciousContent(ContactFormModel model)
    {
        var pattern = @"(<script|<iframe|<img|<a\s|href=|src=|onerror=|onload=|javascript:|data:|base64|http:\/\/|https:\/\/)";
        return Regex.IsMatch(model.Name ?? "", pattern, RegexOptions.IgnoreCase)
            || Regex.IsMatch(model.Email ?? "", pattern, RegexOptions.IgnoreCase)
            || Regex.IsMatch(model.Subject ?? "", pattern, RegexOptions.IgnoreCase)
            || Regex.IsMatch(model.Message ?? "", pattern, RegexOptions.IgnoreCase);
    }

    private static bool EmailContainsContent(ContactFormModel model)
    {
        return !string.IsNullOrWhiteSpace(model.Name)
            && !string.IsNullOrWhiteSpace(model.Email)
            && !string.IsNullOrWhiteSpace(model.Subject)
            && !string.IsNullOrWhiteSpace(model.Message);
    }

    private async Task<bool> SendEmailAsync(ContactFormModel model)
    {
        var message = new EmailMessage
        {
            From = "noreply@mental-notes.com",
            To = { "pkellar66@gmail.com" },
            //To = { "mental.notes.pod@gmail.com" },
            Subject = $"Contact Form: {model.Subject}",
            TextBody = $"Name: {model.Name}\nEmail: {model.Email}\nMessage:\n{model.Message}"
        };

        var resp = await Resend.EmailSendAsync(message);
        return !resp.Content.ToString().Equals(null, StringComparison.InvariantCultureIgnoreCase);
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(64, ErrorMessage = "Name is too long")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(128, ErrorMessage = "Subject is too long")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message is too long")]
        public string? Message { get; set; }
    }
}
