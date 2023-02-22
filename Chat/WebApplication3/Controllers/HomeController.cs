using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.SecurityUtilities;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        DataBaseContext dbContext;

        public HomeController(DataBaseContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ControllerError()
        {
            return View();
        }

        public IActionResult PreMain(string user)
        {
            var users = GetUserList();

            if (string.IsNullOrEmpty(user)
                || !users.Any(u => u == user))
            {
                return View("ControllerError");
            }

            return View(new DialogueFormContainer
            {
                ownerName = user,
                partnerName = "",
                Users = GetUserList()
            });
        }

        public IActionResult Main(string ownerName, string partnerName)
        {
            var users = GetUserList();

            if (string.IsNullOrEmpty(ownerName)
                || string.IsNullOrEmpty(partnerName)
                || !users.Any(u => u == ownerName)
                || !users.Any(u => u == partnerName))
            {
                return View("ControllerError");
            }

            var story = GetStory(ownerName, partnerName);


            return View(new DialogueFormContainer
            {
                ownerName = ownerName,
                partnerName = partnerName,
                Users = users,
                Story = story
            });
        }

        private IEnumerable<string> GetUserList()
        {
            return dbContext.Users.ToList().Select(u => u.Name);
        }

        [HttpPost]
        public IActionResult Main(string message, string ownerName, string partnerName)
        {
            var users = GetUserList();

            if (!(string.IsNullOrEmpty(message)
                || string.IsNullOrEmpty(ownerName)
                || string.IsNullOrEmpty(partnerName)
                || !users.Any(u => u == ownerName)
                || !users.Any(u => u == partnerName)))
            {
                AddMessageToDialogue(message, ownerName, partnerName, ownerName, partnerName);

                // обменятся диалогами
                AddMessageToDialogue(message, ownerName, partnerName, partnerName, ownerName);

                int succesfulRecords = dbContext.SaveChanges();
                if (succesfulRecords == 0)
                {
                    //Debug.Log
                }
            }

            
            var story = GetStory(ownerName, partnerName);

            return View(new DialogueFormContainer
            {
                ownerName = ownerName,
                partnerName = partnerName,
                Users = users,
                Story = story
            });
        }

        private void AddMessageToDialogue(string message, string messageFromUser, string messageToUser, string dialogueOwner, string dialoguePartner)
        {
            User user = dbContext.Users.Include(u => u.PartnersDictionary).FirstOrDefault(u => u.Name == dialogueOwner);

            // Добавление сообщения
            if (user.PartnersDictionary == null || user.PartnersDictionary.Count == 0)
            {
                // в пустую историю
                user.PartnersDictionary = new List<PartnerDictionary>()
                {
                    new PartnerDictionary()
                    {
                        PartnerName = dialoguePartner,
                        Messages = new List<Message>
                            {
                                new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser }
                            }
                    }
                };
            }
            else
            {
                PartnerDictionary partner = user.PartnersDictionary.FirstOrDefault((u => u.PartnerName == dialoguePartner));
                if (partner == null)
                {
                    // диалога с заданным партнером нет, создать
                    user.PartnersDictionary.Add
                    (
                        new PartnerDictionary()
                        {
                            PartnerName = dialoguePartner,
                            Messages = new List<Message>
                            {
                                new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser }
                            }
                        }
                    );
                }
                else
                {
                    // диалог с заданным партнером есть
                    if (partner.Messages == null || partner.Messages.Count == 0)
                    {
                        // но сообщений не было
                        partner.Messages = new List<Message>
                            {
                                new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser }
                            };
                    }
                    else
                    {
                        // сообщения уже есть
                        partner.Messages.Add(new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser });
                    }
                }
            }
        }

        private List<Message> GetStory(string ownerName, string partnerName)
        {
            var currentUserDialogues = dbContext.Users
                .Include(u => u.PartnersDictionary).ThenInclude(u => u.Messages)
                .ToList()
                .FirstOrDefault(u => u.Name == ownerName)
                .PartnersDictionary;

            if (currentUserDialogues == null 
                || currentUserDialogues.Count == 0 
                || currentUserDialogues.FirstOrDefault((u => u.PartnerName == partnerName)) == null)
            {
                return new List<Message>();
            }
            else
            {
                return GetUserDialogueWithPartner(currentUserDialogues.FirstOrDefault((u => u.PartnerName == partnerName)));
            }
        }

        private static List<Message> GetUserDialogueWithPartner(PartnerDictionary dialogueWithPartner)
        {
            if (dialogueWithPartner.Messages == null)
            {
                return new List<Message>();
            }
            else
            {
                return dialogueWithPartner.Messages.OrderBy(m => m.TimeStamp).ToList();
            }
        }

        [HttpPost]
        public IActionResult Index(string name, string password)
        {
            if (string.IsNullOrEmpty(name)
                || string.IsNullOrEmpty(password))
            {
                return View("ControllerError");
            }

            List<User> usersList = dbContext.Users.ToList();
            User user = usersList.FirstOrDefault(u => u.Name == name);

            if (user != null)
            {
                if (Hash.GetMd5Hash(password) == user.Password)
                {
                    return View("ProcessResult", new ProcessResultObject
                    {
                        Message = $"Добро пожаловать в чат, {name}!",
                        Redirect = $"/Home/PreMain?user={name}"
                    });
                }
            }
            return View("ProcessResult", new ProcessResultObject
            {
                Message = $"Имя пользователя или пароль неверны!",
                Redirect = "/"
            });
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult ProcessResult()
        {
            return View("ProcessResult", new ProcessResultObject { Message = "", Redirect = "/" });
        }

        [HttpPost]
        public IActionResult Registration(string name, string password)
        {
            var usersList = dbContext.Users.ToList();
            if (!usersList.Any(u => u.Name == name))
            {
                dbContext.Users.Add(new User { Name = name, Password = Hash.GetMd5Hash(password) });
                dbContext.SaveChanges();

                return View("ProcessResult", new ProcessResultObject
                {
                    Message = $"Пользователь {name} зарегистрирован, вы можете войти.",
                    Redirect = "/"
                }
                );
            }
            else
            {
                return View("ProcessResult", new ProcessResultObject
                {
                    Message = $"Регистрация невозможна, пользователь {name} уже существует.",
                    Redirect = "/"
                }
                );
            }
        }
    }
}
