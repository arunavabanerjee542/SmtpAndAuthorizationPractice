using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PracticeAuthorizationAndAuthentication.DataMapper;
using PracticeAuthorizationAndAuthentication.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PracticeAuthorizationAndAuthentication.Controllers
{
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenDataMapper _tokenDataMapper;

        public TokenController(ITokenDataMapper tokenDataMapper)
        {
            _tokenDataMapper = tokenDataMapper;
        }

        [HttpGet("CreateToken")]
        public string CreateToken()
        {
            var securityKey = "vagshadjad-abkbkajdl-adjdkaj-anldnaklkaladj-anldkn";

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var signingCred = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var filteredClaimModels = _tokenDataMapper
                .GetAllClaims().Where(x => x.Name.Equals("Mail") || x.Name.Equals("admin", StringComparison.InvariantCultureIgnoreCase));

            var requiredClaim = filteredClaimModels
                                 .Select(x => new Claim(x.Name, x.value));

            var allClaims = new List<Claim>();
            allClaims.AddRange(requiredClaim);

            var token = new JwtSecurityToken(
                                issuer: "Arunava",
                                audience: "all",
                                claims: allClaims,
                                signingCredentials: signingCred,
                                expires: DateTime.Now.AddHours(1)
                                         );
            var jwtHandler = new JwtSecurityTokenHandler();

            var x = jwtHandler.WriteToken(token);

            return x;
        }

        [HttpPost("InsertClaim")]
        public ClaimModel InsertClaim([FromBody] ClaimModel claimRequest)
        {
            var response = _tokenDataMapper.InsertClaim(claimRequest.Name, claimRequest.value);

            return response;
        }

        [HttpPost("InsertClaims")]
        public List<ClaimModel> InsertListOfClaims([FromBody] List<ClaimModel> claimsRequest)
        {
            return _tokenDataMapper.InsertClaims(claimsRequest);
        }

        [HttpPost("DeleteClaim")]
        [Authorize("Staff")]
        public string DeleteClaim([FromQuery] string name)
        {
            var result = _tokenDataMapper.DeleteClaim(name);

            return result;
        }

        [HttpGet("GetAllClaims")]
        public List<ClaimModel> GetAllClaims()
        {
            return _tokenDataMapper.GetAllClaims();
        }

        [HttpGet("SendMail")]
        public async Task SendMail()
        {
            MailMessage mail = new MailMessage();
            mail.To.Add("arunavabanerjee541@gmail.com");
            mail.From = new MailAddress("arunroy542@gmail.com");
            mail.Subject = "Sub";
            string Body = "body";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("arunroy542@gmail.com", "9474839954"); // Enter seders User name and password
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(mail);
        }

        [HttpPost("SendClaimsMail")]
        [Authorize("SendMailPolicy")]
        public async Task SendClaimsMail([FromBody] MailModel model)
        {
            var message = GetMailMessage(model);

            await SendMailAsync(message);
        }

        #region private methods

        private MailMessage GetMailMessage(MailModel model)
        {
            var message = new MailMessage();

            message.To.Add(model.To);

            message.From = new MailAddress("arunroy542@gmail.com");

            message.Subject = "Claims";

            var allClaims = _tokenDataMapper.GetAllClaims();

            var stringClaims = JoinAllClaims(allClaims);

            message.Body = stringClaims;

            return message;
        }

        private async Task SendMailAsync(MailMessage message)
        {
            var client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("arunroy542@gmail.com", "9474839954"),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }

        private string JoinAllClaims(List<ClaimModel> claims)
        {
            var result = "";
            foreach (var claim in claims)
            {
                result = result + claim.value + " ";
            }

            return result;
        }

        #endregion private methods
    }
}