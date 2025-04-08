using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

#pragma warning disable
namespace HMZ_rt.Controllers
{
    [Route("Newsletter")]
    [ApiController]
    public class NewSletter_controller : Controller
    {
        private readonly HmzRtContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IOptions<SmtpSettings> _smtpSettings;

        public NewSletter_controller(
            HmzRtContext context,
            IConfiguration configuration,
            IWebHostEnvironment env,
            IOptions<SmtpSettings> smtpOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _smtpSettings = smtpOptions ?? throw new ArgumentNullException(nameof(smtpOptions));
        }

        [HttpPost("SendNewsletter")]
        public async Task<ActionResult> SendNewsletter([FromBody] NewsletterDto newsletterDto)
        {
            try
            {
                
                var subscribers = await _context.Newsletters
                    .Select(x => x.Email)
                    .ToListAsync();

                if (subscribers == null || !subscribers.Any())
                {
                    return StatusCode(404, "Nincsenek feliratkozott email címek");
                }

                
                foreach (var recipientEmail in subscribers)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("HMZ RT", _smtpSettings.Value.FromEmail));
                    message.To.Add(MailboxAddress.Parse(recipientEmail));
                    message.Subject = newsletterDto.Subject;

                    
                    var body = new TextPart(TextFormat.Html)
                    {
                        Text = newsletterDto.HtmlBody
                    };

                    var multipart = new Multipart("mixed");
                    multipart.Add(body);

                    message.Body = multipart;

                    
                    using var client = new SmtpClient();
                    await client.ConnectAsync(_smtpSettings.Value.Server,
                                         _smtpSettings.Value.Port,
                                         SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpSettings.Value.Username,
                                               _smtpSettings.Value.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return StatusCode(200, new
                {
                    status = "success",
                    message = $"Sikeresen elküldve {subscribers.Count} címnek"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("SubscribeForNewsLetter/{userid}")]
        public async Task<ActionResult<Newsletter>> Subscribe(int userid, Subscribenewsletter addto)
        {
            var use = await _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == userid);
            if (use != null)
            {
                var news = new Newsletter
                {
                    Userid = userid,
                    Email = addto.Email,

                };
                await _context.Newsletters.AddAsync(news);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Sikeres feliratkozás");
            }
            return StatusCode(404, "Nem található felhasználó");
        }
    }

   
}
