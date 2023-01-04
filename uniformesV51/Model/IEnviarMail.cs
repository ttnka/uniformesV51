namespace uniformesV51.Model
{
    public interface IEnviarMail
    {
        Task<ApiRespuesta<MailCampos>> EnviarMail(MailCampos mailCampos);
    }
}
