using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System.Globalization;
using uniformesV51.Data;
using uniformesV51.Model;

namespace uniformesV51.Pages.Admin
{
    public class IndexAdminBase : ComponentBase 
    {
        [Inject]
        public Repo<Z100_Org, ApplicationDbContext> OrgRepo { get; set; } = default!;  
        [Inject]
        public Repo<Z110_Usuarios, ApplicationDbContext> UserRepo { get; set; } = default!;
        [Inject]
        public Repo<Z190_Bitacora, ApplicationDbContext> BitaRepo { get; set; } = default!;
        public MyFunc MyFunc { get; set; } = new();
        string ErrorMsn = "";
        protected override async Task OnInitializedAsync()
        {
            await LeerElUser();
        }
        protected async Task Arranque()
        {
            await LeerOrgsAll();
            await LeerUsersAll();
        }
        [Inject]
        public NavigationManager NM { get; set; } = default!;
        
        public Z110_Usuarios ElUser { get; set; } = new Z110_Usuarios();
        [Inject]
        public AuthenticationStateProvider GetAuthenticationStateAsync { get; set; } = default!;
        
        public List<KeyValuePair<int, string>> LosNiveles { get; set; } =
            new List<KeyValuePair<int, string>>();
        
        public List<KeyValuePair<int, string>> LosNivelesTitulos { get; set; } =
            new List<KeyValuePair<int, string>>();
        public async Task LeerElUser()
        {
            try
            {
                var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
                var user = authstate.User;
                if (!user.Identity!.IsAuthenticated)
                    NM.NavigateTo("Identity/Account/Login?ReturnUrl=/admin", true);
                ElUser = (await UserRepo.Get(x => x.OldEmail == user.Identity.Name))
                    .FirstOrDefault() ?? new();
                if (ElUser == null || string.IsNullOrEmpty(ElUser.UserId))
                    NM.NavigateTo("/Bloqueado", true);
                var bitaTemp = MyFunc.MakeBitacora(ElUser!.UserId, ElUser!.OrgId,
                            "Bitacora, Se consulto listado de bitacora!", false);
                await BitaRepo.Insert(bitaTemp);
                await Arranque();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }    
        }
        public List<Z110_Usuarios> LosUsersAll { get; set; } = new List<Z110_Usuarios>();
        public async Task LeerUsersAll()
        {
            ErrorMsn = "";
            try
            {
                var resultado = await UserRepo.Get(x=>x.OrgId == ElUser.OrgId);
                if (resultado != null)
                {
                    LosUsersAll = resultado.ToList<Z110_Usuarios>();
                }
                else
                {
                    ErrorMsn = "No hay registros";
                }
            }
            catch (Exception ex)
            {
                ErrorMsn = ex.Message;
            }
        }
        public List<Z100_Org> LasOrgsAll { get; set; } = new List<Z100_Org>(); 
        public async Task LeerOrgsAll()
        {
            ErrorMsn = "";
            try
            {
                var resultado = await OrgRepo.Get(x => x.OrgId == ElUser.OrgId);
                if (resultado != null)
                {
                    LasOrgsAll = resultado.ToList<Z100_Org>();
                    
                }
                else
                {
                    ErrorMsn = "No hay registros de organizaciones";
                }
            }
            catch (Exception ex)
            {
                ErrorMsn = ex.Message;
                LasOrgsAll = new();
            }
        }
        protected void LeerNiveles()
        {
            string[] NomNiveles = UserNivel.Titulos.Split(",");
            // activa los niveles que puede ver y asignar el usuario
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
            // lee todos los niveles 
            if (LosNivelesTitulos.Count < 1)
            {
                for (int i = 0; i < NomNiveles.Length; i++)
                {
                    LosNivelesTitulos.Add(new KeyValuePair<int, string>
                        (i + 1, NomNiveles[i]));
                }
            }
        }

        public NotificationMessage ElMsn(string tipo, string titulo, 
            string mensaje, int duracion)
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
        public Z190_Bitacora LastBita { get; set; } = new();
        
        public async Task BitacoraAll(Z190_Bitacora bita)
        {
            if (bita.Fecha.Subtract(LastBita.Fecha).TotalSeconds > 15 ||
                LastBita.Desc != bita.Desc || LastBita.Sistema != bita.Sistema ||
                LastBita.UserId != bita.UserId || LastBita.OrgId != bita.OrgId )
            {
                LastBita= bita;
                await BitaRepo.Insert(bita);
            }
        }
        
    }
}
