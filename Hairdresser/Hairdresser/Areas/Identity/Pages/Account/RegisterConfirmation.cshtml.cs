using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Hairdresser.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Hairdresser.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _sender;
        private readonly IOptions<ReCAPTCHA> _keys;

        public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender, IOptions<ReCAPTCHA> keys)
        {
            _userManager = userManager;
            _sender = sender;
            _keys = keys;
        }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnPostAsync(string email, string returnUrl = null)
        {
            string recaptchaResponse = this.Request.Form["g-recaptcha-response"];
            var client = new HttpClient();

            var parameters = new Dictionary<string, string>
            {
                {"secret", _keys.Value.ReCAPTCHA_Secret_Key},
                {"response", recaptchaResponse},
                {"remoteip", this.HttpContext.Connection.RemoteIpAddress.ToString()}
            };

            HttpResponseMessage response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(parameters));
            response.EnsureSuccessStatusCode();

            string apiResponse = await response.Content.ReadAsStringAsync();
            dynamic apiJson = JObject.Parse(apiResponse);
            if (apiJson.success != true)
            {
                this.ModelState.AddModelError(string.Empty, "Potwierdź, że nie jesteś robotem");
            } 
            else
            {
                var user = await _userManager.FindByEmailAsync(email);
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                return RedirectToPage("/Account/ConfirmEmail", new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl });
            }

            return Page();
        }
        
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            return Page();
        }
    }
}
