using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor;
using uniformesV51.Data;
using uniformesV51.Model;

namespace uniformesV51.Pages.Sistema
{
    public class RegistroBase : ComponentBase 
    {
        [CascadingParameter(Name = "ElUserAll")]
        public Z110_Usuarios ElUser { get; set; } = new();
        [CascadingParameter(Name = "LasOrgsAll")]
        public List<Z100_Org> LasOrgs { get; set; } = new List<Z100_Org>();
        public List<KeyValuePair<int, string>> LosNiveles { get; set; } =
            new List<KeyValuePair<int, string>>();
        public AddUser NewAddUser { get; set; } = new();
        
        public RadzenTemplateForm<AddUser>? AddUserForm { get; set; } = new();

        [Inject]
        public Repo<Z100_Org, ApplicationDbContext> OrgRepo { get; set; } = default!;
        [Inject]
        public IAddUser AddUserRepo { get; set; } = default!;
        [Parameter]
        public EventCallback LeerOrgsC { get; set; }
        [Parameter]
        public EventCallback<Z190_Bitacora> BitacoraC { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            if (LasOrgs.Count() > 0 && LosNiveles.Count() < 1 && 
                string.IsNullOrEmpty(NewAddUser.OrgId)) 
                await Arranque();
            
        }
        protected async Task Arranque()
        {
            LeerNiveles();
            LlenarNewAddUser();
            var bitaTemp = MyFunc.MakeBitacora(ElUser.UserId, ElUser.OrgId, 
                "Registro, Entro a registro de nuevos usuarios", false);
            await BitacoraC.InvokeAsync(bitaTemp);
        }
        protected void LlenarNewAddUser()
        {
            if (!string.IsNullOrEmpty(ElUser.OrgId) && LasOrgs.Count() > 0)
            {
                NewAddUser.UsuarioId = ElUser.UserId;
                NewAddUser.UsuarioOrg = ElUser.OrgId;
                NewAddUser.UsuarioMail = ElUser.OldEmail;
                var rs = LasOrgs.FirstOrDefault(x => x.OrgId == ElUser.OrgId).RazonSocial;
                NewAddUser.UsuarioOrgName = rs ?? "Sin Razon Social";
            }
        }
        protected void LeerNiveles()
        {
            string[] NomNiveles = UserNivel.Titulos.Split(",");

            switch (ElUser.OldEmail)
            {
                case Constantes.UserNameMailPublico:
                    LosNiveles.Add(new KeyValuePair<int, string>(1, NomNiveles[0].ToString()));
                    break;

                default:
                    for (int i = 0; i < NomNiveles.Length; i++)
                    {
                        if (ElUser.Nivel > i)
                            LosNiveles.Add(new KeyValuePair<int, string>
                                (i + 1, NomNiveles[i]));
                    }
                    break;
            }
        }
        
        public async Task SaveNewUsuario()
        {
            await AddUserRepo.InsertNew(NewAddUser);
        }
        
        public MyFunc MyFunc { get; set; } = new MyFunc();
        
        public NotificationMessage ElMsn(string tipo, string titulo, string mensaje, int duracion)
        {
            NotificationMessage respuesta = new();
            switch (tipo.ToLower())
            {
                case "info":
                    respuesta.Severity = NotificationSeverity.Info;
                    break;
                case "error":
                    respuesta.Severity = NotificationSeverity.Error;
                    break;
                case "warning":
                    respuesta.Severity = NotificationSeverity.Warning;
                    break;
                default:
                    respuesta.Severity = NotificationSeverity.Success;
                    break;
            }
            respuesta.Summary = titulo;
            respuesta.Detail = mensaje;
            respuesta.Duration = 4000 + duracion;
            return respuesta;
        }

        [Inject]
        public NavigationManager NM { get; set; } = default!;
        public bool Mayuscula { get; set; } = false;
        protected string MayArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public bool Minuscula { get; set; } = false;
        protected string MinArray = "abcdefghijklmnopqrstuvwxyz";
        public bool Numero { get; set; } = false;
        protected string NumArray = "0123456789";
        public bool Diferente { get; set; } = true;
        public bool Punto { get; set; } = false;
        protected string PuntoArray = "-._@+";
        protected string Todos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        public bool IsValido = false;
        public bool IsLargo = false;
        public bool IsRegistro = false;
        public string borrar = string.Empty;
        public void Valido()
        {
            int sigL = 0;
            foreach (var s in NewAddUser.Pass)
            {
                sigL = 0;
                if (IsValido = Todos.Contains(s))
                {
                    if (!Mayuscula && sigL == 0)
                    {
                        if (Mayuscula = MayArray.Any(a => a == s)) sigL = 1;
                    }
                    if (!Minuscula && sigL == 0)
                    {
                        if (Minuscula = MinArray.Any(i => i == s)) sigL = 1;
                    }
                    if (!Numero && sigL == 0)
                    {
                        if (Numero = NumArray.Any(n => n == s)) sigL = 1;
                    }
                    Diferente = NewAddUser.Pass != NewAddUser.ConfirmPass ? true : false;

                    /*
                    if(!Punto)
                    {
                        if(Punto = PuntoArray.Any(p => p==s)) sigL = 1;
                    }
                    */
                }
            }
            IsLargo = NewAddUser.Pass.Length > 5;
            IsRegistro = IsValido && IsLargo && Mayuscula && Minuscula && Numero 
                            && !Diferente;

        }
        
    }
}
