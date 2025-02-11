using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Vent.Shared.ResponsesSec;
using Response = Vent.Shared.Responses.Response;

namespace Vent.Helpers;

public class EmailHelper : IEmailHelper
{
    private readonly SendGridSettings _sendGridOption;

    public EmailHelper(IOptions<SendGridSettings> sendGridOption)
    {
        _sendGridOption = sendGridOption.Value;
    }

    public async Task<bool> EnviarAsync(ContactViewDTO contacto)
    {
        //En este punto tomamos los calore de los AppSetting.Json
        //y lo igualamos a variables para poderlos manipular
        var apiKey = _sendGridOption.SendGridApiKey;
        var email = _sendGridOption.SendGridFrom;
        var nombre = _sendGridOption.SendGridNombre;

        //Cargamos la Utilidad de SendGrid, que es el sistema de envio de datos.
        var cliente = new SendGridClient(apiKey);
        var from = new EmailAddress(email, nombre);
        var subject = $"El Cliente {contacto.Nombre} quiere contactarte";
        var to = new EmailAddress(email, nombre);
        var mensajeTextoPlano = contacto.Mensaje;
        var contenidoHtml = $@"De: {contacto.Nombre}
            <p>
            Email: {contacto.Email}
            <p/>
            <p>
            Mensaje: {contacto.Mensaje}
            <p/>";
        var singleEmail = MailHelper.CreateSingleEmail(from, to, subject, mensajeTextoPlano, contenidoHtml);

        var respuesta = await cliente.SendEmailAsync(singleEmail);

        return respuesta.IsSuccessStatusCode;
    }

    public async Task<Response> ConfirmarCuenta(string to,
        string NameCliente, string subject, string body)
    {
        //En este punto tomamos los calore de los AppSetting.Json
        //y lo igualamos a variables para poderlos manipular
        var apiKey = _sendGridOption.SendGridApiKey;
        var email = _sendGridOption.SendGridFrom;
        var nombre = _sendGridOption.SendGridNombre;

        //Cargamos la Utilidad de SendGrid, que es el sistema de envio de datos.
        var cliente = new SendGridClient(apiKey);
        var from = new EmailAddress(email, nombre);
        var tO = new EmailAddress(to, NameCliente);
        var mensajeTextoPlano = "Sistema de Acticacion de Cuenta";
        var singleEmail = MailHelper.CreateSingleEmail(from, tO, subject,
            mensajeTextoPlano, body);

        var respuesta = await cliente.SendEmailAsync(singleEmail);
        if (respuesta.IsSuccessStatusCode)
        {
            return new Response { IsSuccess = true };
        }
        else
        {
            return new Response { IsSuccess = false };
        }
    }
}