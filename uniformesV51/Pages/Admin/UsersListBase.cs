using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Radzen;
using Radzen.Blazor;
using System.Text.RegularExpressions;
using uniformesV51.Data;
using uniformesV51.Model;

namespace uniformesV51.Pages.Admin
{
    public class UsersListBase : ComponentBase 
    {
        [Inject]
        public Repo<Z110_Usuarios, ApplicationDbContext> UserRepo { get; set; } = default!;
        [CascadingParameter(Name = "ElUserAll")]
        public Z110_Usuarios ElUser { get; set; } = new();
        [CascadingParameter(Name = "LasOrgsAll")]
        public List<Z100_Org> LasOrgs { get; set; } = new List<Z100_Org>();
        [CascadingParameter(Name = "LosNivelesAll")]
        public List<KeyValuePair<int, string>> LosNiveles { get; set; } =
            new List<KeyValuePair<int, string>>();
        [CascadingParameter(Name = "LosNivelesTituloAll")]
        public List<KeyValuePair<int, string>> LosNivelesTitulos { get; set; } =
            new List<KeyValuePair<int, string>>();
        public RadzenDataGrid<Z110_Usuarios>? UsersGrid { get; set; } =
            new RadzenDataGrid<Z110_Usuarios>();
        [Parameter]
        public EventCallback LeerUsersC { get; set; }
        [Parameter]
        public EventCallback<Z190_Bitacora> BitacoraC { get; set; }
        [CascadingParameter (Name="LosUsersAll")]
        public List<Z110_Usuarios> LosUsers { get; set; } = new List<Z110_Usuarios>();
        public bool Primeravez = true;
        protected override async Task OnParametersSetAsync()
        {
            if (Primeravez)
            {
                Primeravez = false;
                await Arranque();
            }
        }
        protected async Task Arranque()
        {
            var bitaTemp = MyFunc.MakeBitacora(ElUser.UserId, ElUser.OrgId,
                "Usuarios, Se consulto el listado de usuarios", false);
            await BitacoraC.InvokeAsync(bitaTemp);
        }
        protected async Task LeerUsers()
        {
            await LeerUsersC.InvokeAsync();
            Editando = false;
        }
        
        public async Task<bool> Servicio(string tipo, Z110_Usuarios user)
        {
            if (user == null) return false;
            switch (tipo)
            {
                case "Insert":
                    var userInsert = await UserRepo.Insert(user);
                    return userInsert != null ? true : false;

                case "Update":
                    var userUpdate = await UserRepo.Update(user);
                    return userUpdate != null ? true : false;

                default:
                    return false;
            }
        }

        protected MyFunc MyFunc { get; set; } = new();
        protected bool Editando = false;
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
    }
}
