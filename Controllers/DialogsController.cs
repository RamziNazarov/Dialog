using System;
using System.Collections.Generic;
using System.Linq;
using Dialogs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dialogs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DialogsController : ControllerBase
    {
        [HttpGet]
        public Guid SearchClientsDialogId([FromQuery]List<Guid> idClients)
        {
            var rgDialogsClientsList = new RGDialogsClients().Init();
            
            var rgDialogsClientsDictionary = rgDialogsClientsList
                //выборка диалогов из списка
                .Select(x=>x.IDRGDialog)
                //удаление повторений диалогов
                .Distinct()
                //гуппировка всех клиентов по диалогам
                .ToDictionary(dialog => dialog,//х - диалог 
                    //выборка клиентов из списка по их диалогу
                    dialog => rgDialogsClientsList
                    .Where(rgDialogClient => rgDialogClient.IDRGDialog == dialog)
                    .Select(rgDialogClient => rgDialogClient.IDClient));
            
            var rgDialogId = rgDialogsClientsDictionary
                //поиск того диалога у которого список клиентов совпадает со списком принятным в параметре метода
                .Where(dialogWithClients => 
                    //сравнение, присутствуют ли все элементы из принятого в параметрах метода списка в список клиентов диалога
                    dialogWithClients.Value
                    .All(idClients.Contains) && 
                    //сравнение количества клиентов в принятом списке и в списке клиентов диалога, на случай если в принятом списке есть повторения
                    dialogWithClients.Value.Count() == idClients.Count)
                //выборка диалога
                .Select(dialogWithClients => dialogWithClients.Key)
                //возвращение диалога при нахождении, иначе пустой guid
                .FirstOrDefault();
            
            return rgDialogId;
        }
    }
}