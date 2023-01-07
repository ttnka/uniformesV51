using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using uniformesV51.Model;
using uniformesV51.Data;
using Radzen;
using Microsoft.Net.Http.Headers;
using uniformesV51.Pages.Sistema;
using System.Runtime.InteropServices;

namespace uniformesV51.Pages.Admin
{
    public class OrgBase : ComponentBase 
    {
        [Inject]
        public Repo<Z100_Org, ApplicationDbContext> OrgRepo { get; set; } = default!;
        [CascadingParameter(Name = "LasOrgsAll")]
        public List<Z100_Org> LasOrgs { get; set; } = new List<Z100_Org>();
        protected bool Editando = false;
        public RadzenDataGrid<Z100_Org>? OrgGrid { get; set; } =
            new RadzenDataGrid<Z100_Org>();
        protected Dictionary<string, string> DicOrg { get; set; } =
            new Dictionary<string, string>();
        protected string ErrorMsn = string.Empty;
        [Parameter]
        public EventCallback LeerOrgsC { get; set; }
        [Parameter]
        public EventCallback<Z190_Bitacora> BitacoraC { get; set; }
        protected bool Arranco = false;
        protected override async Task OnParametersSetAsync()
        {
            if(!Arranco)
            {
                Arranco = true;
                await Arranque();
                if (LasOrgs.Count() > 0) DicAdd(LasOrgs); 
            }
        }

        protected async Task Arranque()
        {
            var bitaTemp = MyFunc.MakeBitacora(ElUser.UserId, ElUser.OrgId,
                            "Organizaciones, Consulto listado de organizaciones", false);
            await BitacoraC.InvokeAsync(bitaTemp);
        }
        public async Task<bool> Servicio(string tipo, Z100_Org org)
        {
            if (org == null) return false;    
            switch (tipo) {
                case "Insert":
                    var orgInsert = await OrgRepo.Insert(org);
                        return orgInsert != null ? true : false;
                    
                case "Update":
                    var orgUpdate = await OrgRepo.Update(org);
                        return orgUpdate != null ? true : false;
                 
                default: 
                    return false;         
            }
        }
        protected async Task LeerOrgs()
        {
            await LeerOrgsC.InvokeAsync();
            DicAdd(LasOrgs);
            Editando = false;
        }
        protected void DicAdd(List<Z100_Org> LasO)
        {
            foreach (var L in LasO)
            {
                if (!DicOrg.ContainsKey(L.Rfc.ToUpper()))
                {
                    DicOrg.Add(L.Rfc.ToUpper(), L.OrgId);
                }
            }
        }

        protected MyFunc MyFunc { get; set; } = new();

        [CascadingParameter (Name="ElUserAll")]
        public Z110_Usuarios ElUser { get; set; } = new();

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
