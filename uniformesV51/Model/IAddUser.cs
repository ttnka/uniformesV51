
namespace uniformesV51.Model
{
    public interface IAddUser
    {
        Task<ApiRespuesta<AddUser>> FirmaIn(AddUser addUsuario);
        Task<ApiRespuesta<AddUser>> InsertNew(AddUser NewUser);
        //Task<ERecupera> Recupera(ERecupera recupera);
        //Task<EAddUsuario> ResetPassword(EAddUsuario eAddUsuario);
        
    }
}
