using Microsoft.AspNetCore.Components;
using uniformesV51.Data;
using uniformesV51.Model;

namespace uniformesV51.Pages.Sistema
{
    public class ArranqueBase : ComponentBase
    {
        
        [Inject]
        public Repo<Z100_Org, ApplicationDbContext> OrgsRepo { get; set; } = default!;

        [Inject]
        public IAddUser AddUserRepo { get; set; } = default!;  
        [Inject]
        public Repo<Z110_Usuarios, ApplicationDbContext> UserRepo { get; set; } = default!;
        [Inject]
        public Repo<Z190_Bitacora, ApplicationDbContext> BitacoraRepo { get; set; } = default!;
        [Inject]
        public NavigationManager NM { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            var emp1 = await OrgsRepo.Get(x => x.Rfc == Constantes.PgRfc);
            var emp2 = await OrgsRepo.Get(x => x.Rfc == Constantes.SyRfc);
            if((emp1 != null && emp1.Count() > 0) && (emp2 != null && emp2.Count()>0)) { NM.NavigateTo("/", true); }
        }

        protected async Task RunInicio()
        {
            if (Clave.Pass == Constantes.Arranque)
                await Creacion();
            
            Clave.Pass = "";
            NM.NavigateTo("/");
        }

        protected async Task<bool> Creacion()
        {
            var resultado = await OrgsRepo.GetAll();
            if (resultado == null || resultado.Count() > 1) return false;
            try
            {
                // Genera una nueva organizacion con datos sistema 
                Z100_Org SysOrg = new();
                SysOrg.Rfc = Constantes.SyRfc;
                SysOrg.Comercial = Constantes.SyRazonSocial;
                SysOrg.RazonSocial = Constantes.SyRazonSocial;
                SysOrg.Estado = Constantes.SyEstado;
                SysOrg.Status = Constantes.SyStatus;
                var newSysOrg = await OrgsRepo.Insert(SysOrg);

                Console.WriteLine(newSysOrg.ToString());
                
                // Genera un nuevo acceso al sistema con un usuario
                AddUser eAddUsuario = new();
                eAddUsuario.Mail = Constantes.SyMail;
                eAddUsuario.Pass = Constantes.SysPassword;
                eAddUsuario.OrgId = newSysOrg.OrgId;
                eAddUsuario.Nivel = 7; 

                var userNew = await AddUserRepo.InsertNew(eAddUsuario);

                Console.WriteLine(userNew.ToString());

                // Actualiza los datos del usuario al que se le genero el acceso 
                Z110_Usuarios SyUser = new();
                SyUser.UserId = userNew.Data.UsuarioId!;
                SyUser.OrgId = userNew.Data.OrgId!;
                SyUser.Nombre = Constantes.SyRazonSocial;
                SyUser.Paterno = Constantes.ElDominio;
                SyUser.OldEmail = Constantes.SyMail;
                SyUser.Nivel = eAddUsuario.Nivel;
                SyUser.Estado = Constantes.SyEstado;
                SyUser.Status = true;
                var pruba = await UserRepo.Update(SyUser);

                Console.WriteLine(pruba.ToString());

                // Genera una organizacion nueva para publico en general
                Z100_Org PgOrg = new();
                PgOrg.Rfc = Constantes.PgRfc;
                PgOrg.Comercial = Constantes.PgRazonSocial;
                PgOrg.RazonSocial = Constantes.PgRazonSocial;
                PgOrg.Estado = Constantes.PgEstado;
                PgOrg.Status = Constantes.PgStatus;
                var newPgOrg = await OrgsRepo.Insert(PgOrg);

                Console.WriteLine(newPgOrg.ToString());

                // Genera acceso para publico en general todos 
                AddUser eAddUsuarioPublico = new();
                eAddUsuarioPublico.Mail = Constantes.DeMailPublico;
                eAddUsuarioPublico.Pass = Constantes.PasswordMailPublico;
                eAddUsuarioPublico.OrgId = newPgOrg.OrgId;
                eAddUsuarioPublico.Nivel = Constantes.NivelPublico;
                var userNewPublico = await AddUserRepo.InsertNew(eAddUsuarioPublico);

                Console.WriteLine(userNewPublico.ToString());

                // Genera el usuario al que se le genero el acceso 
                Z110_Usuarios PgUser = new();
                PgUser.UserId = userNewPublico.Data.UsuarioId!;
                PgUser.OrgId = userNewPublico.Data.OrgId!;
                PgUser.Nombre = Constantes.PgRazonSocial;
                PgUser.Paterno = Constantes.ElDominio;
                PgUser.OldEmail = Constantes.DeMailPublico;
                PgUser.Nivel = Constantes.NivelPublico;
                PgUser.Estado = Constantes.NivelPublico;
                PgUser.Status = true;
                var pu2 = await UserRepo.Update(PgUser);

                var bitaTemp = MyFunc.MakeBitacora(
                    "Vacio", "Vacio", "Se las tablas por primera vez", true);
                await BitacoraRepo.Insert(bitaTemp);
                return true;
            }
            catch (Exception ex)
            {
                var msn = ex.Message;
                return false;

            }
        }

        public class LaClave
        {
            public string Pass { get; set; } = "";
        }
        public LaClave Clave { get; set; } = new();
        public MyFunc MyFunc { get; set; } = new();
    }
}
