using System.Net;
using System.Net.Mail;
using System.Text;

namespace Chirp.Server.Services.Mail; 

public class SmtpMailService : IMailService {

  private readonly SmtpClient _smtpClient;
  private readonly MailAddress _from;

  public SmtpMailService(SmtpMailConfig smtpConfig) {
    _smtpClient = new SmtpClient(smtpConfig.Host, smtpConfig.Port) {
      Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password, smtpConfig.Domain),
      EnableSsl = smtpConfig.EnableSsl,
      Timeout = smtpConfig.Timeout,
    };
    _from = new MailAddress(smtpConfig.FromAddress, smtpConfig.FromDisplayName, Encoding.UTF8);
  }

  public Task SendEmailAsync(MailMessage message, CancellationToken cancellationToken) {
    message.From = _from;
    return _smtpClient.SendMailAsync(message, cancellationToken);
  }

  public class SmtpMailConfig {
    public string Host { get; set; } = "domain.com";
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Domain { get; set; }
    public bool EnableSsl { get; set; } = true;
    public int Timeout { get; set; } = 100000;
    public string FromAddress { get; set; } = "no-reply@domain.com";
    public string FromDisplayName { get; set; } = "no-reply";
  }
}