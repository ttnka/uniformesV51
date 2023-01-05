using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using uniformesV51.Data;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using uniformesV51.Pages.Sistema;
using MailKit.Security;
using MimeKit;
using System.Text.RegularExpressions;

namespace uniformesV51.Model
{
    public class RAddUser : IAddUser
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly NavigationManager _navigationManager;

        public RAddUser(ApplicationDbContext appDbContext,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            NavigationManager navigationManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _emailSender = emailSender;
            _navigationManager = navigationManager;
        }
        public Z110_Usuarios NewUsuario { get; set; } = new();        
        public async Task<ApiRespuesta<AddUser>> FirmaIn(AddUser addUsuario)
        {
            ApiRespuesta<AddUser> apiRespuesta = new() 
                { Exito = false, Data = addUsuario };
            string texto = "";
            
            {
                try
                {
                    
                    var resultado = await _signInManager.PasswordSignInAsync(
                        addUsuario.Mail, addUsuario.Pass, addUsuario.RecordarMe,
                        lockoutOnFailure: false);

                    addUsuario.Positivo = resultado.Succeeded;
                    
                    apiRespuesta.Exito = true;
                    texto = $"Firmo con exito {addUsuario.Mail}";
                    await WriteBitacora("Vacio", "Vacio", texto, true);
                    return apiRespuesta;
                }
                catch (Exception ex)
                {
                    apiRespuesta.MsnError = new List<string> { ex.Message };
                    texto = $"Intento ingresar {addUsuario.Mail} pass XXX ";
                    await WriteBitacora("Vacio", "Vacio", texto, false);
                    return apiRespuesta;
                }
            }
            
        }

        public async Task<ApiRespuesta<AddUser>> InsertNew(AddUser NewUser)
        {
            ApiRespuesta<AddUser> apirespuesta = new() { Exito = false, Data = NewUser };
            //string txt = "";
            
            try
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, NewUser.Mail, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, NewUser.Mail, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, NewUser.Pass);
                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    NewUser.UsuarioId = NewUser.UsuarioId ?? userId;
                    NewUser.UsuarioOrg = NewUser.UsuarioOrg ?? NewUser.OrgId;

                    await WriteBitacora(NewUser.UsuarioId, NewUser.UsuarioOrg,
                        $"Se creo nuevo acceso {NewUser.Mail} y password", true);
                    
                    NewUser.UsuarioId = userId;

                    NewUsuario.UserId = userId;
                    NewUsuario.Nombre = NewUser.Mail;
                    NewUsuario.Paterno = NewUser.Mail;
                    NewUsuario.Nivel = NewUser.Nivel;
                    NewUsuario.OrgId = NewUser.OrgId;
                    NewUsuario.OldEmail = NewUser.Mail;

                    await _appDbContext.Usuarios.AddAsync(NewUsuario);
                    await _appDbContext.SaveChangesAsync();

                    await WriteBitacora(NewUser.UsuarioId, NewUser.UsuarioOrg,
                        $"Se creo nuevo usuario {NewUser.Mail} de la organizacion {NewUser.UsuarioOrgName}", false);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var url = ($"https://localhost:7209/confirmamail/userId={userId}&code={code}");

                    MailCampos mailCampos = new MailCampos();
                    string elCuerpo = "<label>Hola !</label> <br /><br />";
                    elCuerpo += $"<label>Te escribimos de {Constantes.ElDominio} para enviarte un correo de confirmacion de cuenta </label><br />";
                    elCuerpo += $"<label>por favor confirma tu Cuenta de correo ingresando al siguiente enlace:</label><br />";
                    elCuerpo += $"<a href={url}>Confirma tu Cuenta</a> <br />";

                    MailCampos datos = new MailCampos();
                    datos = mailCampos.PoblarMail(NewUser.Mail, "Confirma Tu correo!", elCuerpo,
                        NewUser.Mail, userId, NewUsuario.OrgId, Constantes.DeNombreMail01,
                        Constantes.DeMail01, Constantes.ServerMail01, Constantes.PortMail01,
                        Constantes.UserNameMail01, Constantes.PasswordMail01);

                   var mails = await EnviarMail(datos);
                }
                else
                {
                    await WriteBitacora(NewUser.UsuarioId, NewUser.UsuarioOrg,
                        $"No creo nuevo usuario!!! {NewUser.Mail}", true);
                }
                apirespuesta.Exito = true;
                return apirespuesta;
            }
            catch (Exception ex)
            {
                apirespuesta.MsnError = new List<string>() { ex.Message };
                return apirespuesta;
            }
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"No pude creear un nuevo usuario de '{nameof(IdentityUser)}'. ");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Se requiere soporte de correo electronico!.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        public MyFunc MyFunc { get; set; } = new();
        public async Task WriteBitacora (string uId, string oId, string d, bool s)
        {
            var bitaTemp = MyFunc.MakeBitacora(uId, oId, d,s);
            await _appDbContext.Bitacora.AddAsync(bitaTemp);
            await _appDbContext.SaveChangesAsync();    
            
        }

        public async Task<ApiRespuesta<MailCampos>> EnviarMail(MailCampos mailCampos)
        {
            ApiRespuesta<MailCampos> apiRespuesta = new()
            {
                Exito = false,
                MsnError = new List<string>(),
                Data = mailCampos
            };
            #region EVALUAR Info de envio y cuentas
            if (mailCampos == null)
            {
                apiRespuesta.Exito = false;
                apiRespuesta.MsnError.Add("No hay datos para enviar mail!");
                apiRespuesta.Data = mailCampos;
                await WriteBitacora("vacio", "vacio", "No hay datos para enviar mail", true);
                return apiRespuesta;
            }
            if (string.IsNullOrEmpty(mailCampos.SenderEmail))
                apiRespuesta.MsnError.Add("No hay direccion de envio Sender");

            // revisamos si hay mail del receptor y si cumple con el formato
            if (string.IsNullOrEmpty(mailCampos.Para) ||
                Regex.IsMatch(mailCampos.Para,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase)
                )
                apiRespuesta.MsnError.Add("No hay direccion del receptor");
            if (string.IsNullOrEmpty(mailCampos.Titulo)
                )
                apiRespuesta.MsnError.Add("No hay titulo del mail!");

            if (string.IsNullOrEmpty(mailCampos.Cuerpo))
                apiRespuesta.MsnError.Add("No hay cuerpo del mail");

            if (apiRespuesta.MsnError.Count > 0)
            {
                var texto = "";
                foreach (var me in apiRespuesta.MsnError)
                {
                    texto += me.ToString() + " ";
                }
                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId, texto, true);
                return apiRespuesta;
            }

            #endregion
            #region Evaluar si es correo de pruebas
            if (mailCampos.Para.EndsWith(".com1") ||
                mailCampos.Para == Constantes.DeMail01)
            {
                apiRespuesta.Exito = true;
                apiRespuesta.MsnError.Add("Email de prueba exitos!");
                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId,
                    "Se supespendio el envio de mail ya que es un correo de prueba!", true);
                return apiRespuesta;
            }
            #endregion
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(mailCampos.SenderEmail));
                email.To.Add(MailboxAddress.Parse(mailCampos.Para));
                email.Subject = mailCampos.Titulo;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailCampos.Cuerpo };
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect(mailCampos.Server, mailCampos.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailCampos.UserName, mailCampos.Password);
                smtp.Send(email);
                smtp.Disconnect(true);

                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId,
                    $"Se envio un Email a {mailCampos.Para} Titulo {mailCampos.Titulo}", true);
                apiRespuesta.Exito = true;
                return apiRespuesta;
            }
            catch (Exception ex)
            {
                apiRespuesta.MsnError.Add(ex.Message);
                var text = $"Hubo un error al enviar MAIL {ex.Message} Para {mailCampos.Para} ";
                text += $"Titulo {mailCampos.Titulo} ";
                await WriteBitacora(mailCampos.UserId, mailCampos.OrgId, text, true);
                return apiRespuesta;
            }
        }


    }
}
