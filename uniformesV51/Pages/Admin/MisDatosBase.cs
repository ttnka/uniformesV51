using Microsoft.AspNetCore.Components;
using Radzen;
using uniformesV51.Data;
using uniformesV51.Model;
using uniformesV51.Pages.Sistema;

namespace uniformesV51.Pages.Admin
{
    public class MisDatosBase : ComponentBase
    {
        [Inject]
        public Repo<Z110_Usuarios, ApplicationDbContext> UsersRepo { get; set; } = default!;
        [CascadingParameter(Name = "ElUserAll")]
        public Z110_Usuarios ElUser { get; set; } = new();
        
        [CascadingParameter(Name = "LosNivelesTituloAll")]
        public List<KeyValuePair<int, string>> LosNivelesTitulos { get; set; } =
            new List<KeyValuePair<int, string>>();
        [Parameter]
        public EventCallback<Z190_Bitacora> BitacoraC { get; set; }
        
        public bool Primeravez = true;
        protected async override Task OnParametersSetAsync()
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
                "Mis datos, Se consulto sus datos", false);
            await BitacoraC.InvokeAsync(bitaTemp);
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
