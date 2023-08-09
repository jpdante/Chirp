using System.Net.Mail;

namespace Chirp.Server.Services; 

public interface IMailService {
  
  public Task SendEmailAsync(MailMessage mailMessage, CancellationToken cancellationToken);

}